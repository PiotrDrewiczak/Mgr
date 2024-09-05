import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import * as d3 from 'd3';
import { Point } from './models/point.model';
import { Polygon } from './models/polygon.model';
import { PolygonService } from './service/polygon.service';

@Component({
  selector: 'app-polygon',
  templateUrl: './polygon.component.html',
  styleUrls: ['./polygon.component.css']
})
export class PolygonComponent implements OnInit, OnDestroy {
  @ViewChild('svg', { static: true }) svg!: ElementRef<SVGElement>;
  private polygonsSubscription: Subscription = new Subscription();
  polygonsData: Polygon[] = []; 
  selectedPolygonIndex: number = 0;

  constructor(private polygonService: PolygonService) {}

  ngOnInit(): void {
    this.polygonsSubscription = this.polygonService.polygonsData$.subscribe(polygons => {
      this.polygonsData = polygons || []; 
      this.drawPolygon();
    });
  }

  ngOnDestroy(): void {
    this.polygonsSubscription.unsubscribe();
  }

  selectPolygon(): void {
    this.drawPolygon();
  }

  private drawPolygon(): void {
    const svg = d3.select(this.svg.nativeElement);
    svg.selectAll("*").remove();

    if (this.polygonsData.length > 0) {
      const polygon = this.polygonsData[this.selectedPolygonIndex].vertices;

      const line = d3.line<Point>()
        .x(d => d.x)
        .y(d => d.y)
        .curve(d3.curveLinearClosed);

      svg.append('path')
        .datum(polygon)
        .attr('d', line)
        .attr('fill', 'none')
        .attr('stroke', 'black');
    }
  }
}
