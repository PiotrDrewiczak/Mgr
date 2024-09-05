import { Component, ElementRef, OnInit, ViewChild, OnDestroy } from '@angular/core';
import * as d3 from 'd3';
import { QuadrangleService } from './service/quadrangle.service';
import { Triangulation } from '../triangulation/models/triangulation.model';
import { TriangulationService } from '../triangulation/service/triangulation.service';
import { Subscription } from 'rxjs';
import { Rectangles } from './models/rectangles.model';

@Component({
  selector: 'app-quadrangle',
  templateUrl: './quadrangle.component.html',
  styleUrls: ['./quadrangle.component.css']
})
export class QuadrangleComponent implements OnInit, OnDestroy {

  @ViewChild('svg', { static: true }) svg!: ElementRef<SVGElement>;
  quadrangles: Rectangles[] = [];
  triangulations: Triangulation[] = [];
  subscriptions: Subscription = new Subscription();
  selectedQuadrangleIndex: number = 0;
  coveragePercentage: number = 0;

  constructor(private quadrangleService: QuadrangleService,
              private triangulationService: TriangulationService) {}

  ngOnInit(): void {
    this.subscriptions.add(
      this.quadrangleService.quadrulationData$.subscribe(data => {
        this.quadrangles = data;
        this.triangulations = this.triangulationService.getTriangulationData();
        this.selectQuadrangle();
      })
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private calculateCoveragePercentage(): void {
    if (this.quadrangles.length > 0) {
      const quadrangle = this.quadrangles[this.selectedQuadrangleIndex];

      const totalRectangleArea = quadrangle.rectangles.reduce((total, rect) => {
        return total + (rect.width * rect.height);
      }, 0);

      const totalQuadrangleArea = this.triangulations[this.selectedQuadrangleIndex]?.area || 0;

      this.coveragePercentage = (totalRectangleArea / totalQuadrangleArea) * 100;
    } else {
      this.coveragePercentage = 0;
    }
  }

  private draw(): void {
    const svg = d3.select(this.svg.nativeElement);
    svg.selectAll('*').remove(); // Clear previous drawings
  
    if (this.triangulations.length > 0) {
      const triangulation = this.triangulations[this.selectedQuadrangleIndex];
  
      triangulation.triangles.forEach(triangle => {
        const points: { x: number, y: number }[] = [
          { x: triangle.a.x, y: triangle.a.y },
          { x: triangle.b.x, y: triangle.b.y },
          { x: triangle.c.x, y: triangle.c.y }
        ];
  
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
    }
  
    if (this.quadrangles.length > 0) {
      const quadrangle = this.quadrangles[this.selectedQuadrangleIndex];
  
      if (quadrangle && Array.isArray(quadrangle.rectangles)) {
        quadrangle.rectangles.forEach(rectangle => {
          const points = [
            { x: rectangle.x, y: rectangle.y },
            { x: rectangle.x + rectangle.width, y: rectangle.y },
            { x: rectangle.x + rectangle.width, y: rectangle.y + rectangle.height },
            { x: rectangle.x, y: rectangle.y + rectangle.height }
          ];
  
          svg.append("polygon")
            .attr("class", "rectangle")
            .attr("points", points.map(p => `${p.x},${p.y}`).join(' '))
            .attr("stroke", "red") // Kolor obrysu
            .attr("stroke-width", 2) // Szerokość obrysu
            .attr("fill", "red"); // Kolor wypełnienia
        });
      } else {
        console.error('Quadrangle or quadrangle.Rectangles is undefined, null, or not an array.');
      }
    } else {
      console.error('No quadrangles available to draw.');
    }
  }
  

  selectQuadrangle(): void {
    this.draw();
    this.calculateCoveragePercentage();
  }
}
