import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import * as d3 from 'd3';
import { PredictionService } from './service/prediction.service';
import { Point } from '../polygon/models/point.model';
import { PolygonWithCenter } from './models/PolygonWithCenter.model';

@Component({
  selector: 'app-prediction',
  templateUrl: './prediction.component.html',
  styleUrls: ['./prediction.component.css']
})
export class PredictionComponent implements OnInit, OnDestroy {
  @ViewChild('svg', { static: true }) svg!: ElementRef<SVGElement>;
  private predictionSubscription: Subscription = new Subscription();
  predictionData: PolygonWithCenter[] = [];
  selectedPolygonIndex: number = 0;

  constructor(private predictionService: PredictionService) {}

  ngOnInit(): void {
    this.predictionSubscription = this.predictionService.centerData$.subscribe(data => {
      this.predictionData = data || [];
      this.drawPolygonWithCenter();
    });
  }

  ngOnDestroy(): void {
    this.predictionSubscription.unsubscribe();
  }

  selectPolygon(): void {
    this.drawPolygonWithCenter();
  }

  private drawPolygonWithCenter(): void {
    const svg = d3.select(this.svg.nativeElement);
    svg.selectAll("*").remove();

    if (this.predictionData.length > 0) {
      const polygonWithCenter = this.predictionData[this.selectedPolygonIndex];
      const vertices = polygonWithCenter.vertices;

      // Rysowanie wielokąta
      const line = d3.line<Point>()
        .x(d => d.x)
        .y(d => d.y)
        .curve(d3.curveLinearClosed);

      svg.append('path')
        .datum(vertices)
        .attr('d', line)
        .attr('fill', 'none')
        .attr('stroke', 'black')
        .attr('stroke-width', 2);

      // Rysowanie środka
      svg.append('circle')
        .attr('cx', polygonWithCenter.x * 1000)
        .attr('cy', polygonWithCenter.y * 1000)
        .attr('r', 5) // Wielkość kropki
        .attr('fill', 'blue');
    }
  }
}
