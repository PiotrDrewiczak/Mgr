import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import * as d3 from 'd3';
import { Triangulation } from './models/triangulation.model';
import { TriangulationService } from './service/triangulation.service';

@Component({
  selector: 'app-triangulation',
  templateUrl: './triangulation.component.html',
  styleUrls: ['./triangulation.component.css']
})
export class TriangulationComponent implements OnInit, OnDestroy {
  @ViewChild('svg', { static: true }) svg!: ElementRef<SVGElement>;
  triangulations: Triangulation[] = [];
  selectedTriangulationIndex: number = 0;
  private triangulationDataSubscription: Subscription = new Subscription();

  constructor(private triangulationService: TriangulationService) {}

  ngOnInit(): void {
    this.triangulationDataSubscription = this.triangulationService.triangulationData$.subscribe(data => {
      this.triangulations = data;
      this.drawTriangulation();
    });
  }

  ngOnDestroy(): void {
    this.triangulationDataSubscription.unsubscribe();
  }

  selectTriangulation(): void {
    this.drawTriangulation();
  }

  private drawTriangulation(): void {
    const svg = d3.select(this.svg.nativeElement);
    svg.selectAll("*").remove(); // Clear previous drawings

    if (this.triangulations.length > 0) {
      const triangulation = this.triangulations[this.selectedTriangulationIndex];

      // Iterate over each triangle in the triangulation
      triangulation.triangles.forEach(triangle => {
        const points: { x: number, y: number }[] = [
          { x: triangle.a.x, y: triangle.a.y },
          { x: triangle.b.x, y: triangle.b.y },
          { x: triangle.c.x, y: triangle.c.y }
        ];

        // Draw lines between points A, B, and C to form a triangle
        svg.append("line")
          .attr("x1", points[0].x)
          .attr("y1", points[0].y)
          .attr("x2", points[1].x)
          .attr("y2", points[1].y)
          .attr("stroke", "black")
          .attr("stroke-width", 2);

        svg.append("line")
          .attr("x1", points[1].x)
          .attr("y1", points[1].y)
          .attr("x2", points[2].x)
          .attr("y2", points[2].y)
          .attr("stroke", "black")
          .attr("stroke-width", 2);

        svg.append("line")
          .attr("x1", points[2].x)
          .attr("y1", points[2].y)
          .attr("x2", points[0].x)
          .attr("y2", points[0].y)
          .attr("stroke", "black")
          .attr("stroke-width", 2);
      });
    } else {
      console.error('No triangulation data available to draw.');
    }
  }
}
