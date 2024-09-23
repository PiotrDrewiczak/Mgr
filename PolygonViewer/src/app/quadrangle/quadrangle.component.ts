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
  quadrangles: Rectangles[][] = [];  // Lista list prostokątów dla każdej triangulacji
  triangulations: Triangulation[] = [];
  subscriptions: Subscription = new Subscription();
  selectedQuadrangleIndex: number = 0;
  coveragePercentage: number = 0;
  maxRectangles: Rectangles | null = null;  // Prostokąty o największej powierzchni dla wybranej triangulacji

  constructor(private quadrangleService: QuadrangleService,
              private triangulationService: TriangulationService) {}

  ngOnInit(): void {
    this.subscriptions.add(
      this.quadrangleService.quadrulationData$.subscribe(data => {
        this.quadrangles = data;  // Otrzymujemy listę list prostokątów
        this.triangulations = this.triangulationService.getTriangulationData();
        this.selectQuadrangle();  // Wybierz pierwszą konfigurację po załadowaniu danych
      })
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private calculateCoveragePercentage(): void {
    if (this.maxRectangles && this.triangulations.length > 0) {
      const totalRectangleArea = this.maxRectangles.totalArea;
      const totalQuadrangleArea = this.triangulations[this.selectedQuadrangleIndex]?.area || 0;
      this.coveragePercentage = (totalRectangleArea / totalQuadrangleArea) * 100;
    } else {
      this.coveragePercentage = 0;
    }
  }

  private findMaxAreaRectangles(rectanglesList: Rectangles[]): Rectangles | null {
    // Sprawdzamy, czy lista nie jest pusta
    if (!rectanglesList || rectanglesList.length === 0) return null;
  
    // Używamy pierwszego elementu jako wartości początkowej w reduce
    return rectanglesList.reduce((max, current) => {
      return current.totalArea > max.totalArea ? current : max;
    }, rectanglesList[0]);  // Używamy pierwszego elementu jako początkowej wartości
  }

  private draw(): void {
    const svg = d3.select(this.svg.nativeElement);
    svg.selectAll('*').remove();  // Czyścimy poprzednie rysunki

    // Rysowanie triangulacji
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

    // Rysowanie prostokątów o największej powierzchni
    if (this.maxRectangles && Array.isArray(this.maxRectangles.rectangles)) {
      this.maxRectangles.rectangles.forEach(rectangle => {
        const points = [
          { x: rectangle.x, y: rectangle.y },
          { x: rectangle.x + rectangle.width, y: rectangle.y },
          { x: rectangle.x + rectangle.width, y: rectangle.y + rectangle.height },
          { x: rectangle.x, y: rectangle.y + rectangle.height }
        ];

        svg.append("polygon")
          .attr("class", "rectangle")
          .attr("points", points.map(p => `${p.x},${p.y}`).join(' '))
          .attr("stroke", "red")  // Kolor obrysu
          .attr("stroke-width", 2)  // Szerokość obrysu
          .attr("fill", "red");  // Kolor wypełnienia
      });
    } else {
      console.error('No rectangles available to draw.');
    }
  }

  selectQuadrangle(): void {
    if (this.quadrangles.length > 0 && this.selectedQuadrangleIndex < this.quadrangles.length) {
      // Znajdź prostokąty o największej powierzchni dla wybranej triangulacji
      this.maxRectangles = this.findMaxAreaRectangles(this.quadrangles[this.selectedQuadrangleIndex]);

      if (this.maxRectangles) {
        this.draw();  // Rysuj prostokąty o największej powierzchni
        this.calculateCoveragePercentage();  // Oblicz procentowe pokrycie powierzchni
      }
    } else {
      console.error('Invalid quadrangles data or selected index.');
    }
  }
  saveAll(): void {
    this.triangulations.forEach((triangulation, triangulationIndex) => {
      const quadrangleList = this.quadrangles[triangulationIndex];  // Lista prostokątów dla danej triangulacji
  
      quadrangleList.forEach((rectangles) => {
        // Tworzenie ukrytego SVG
        const hiddenSvg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
        hiddenSvg.setAttribute('width', '1600');
        hiddenSvg.setAttribute('height', '1500');
        hiddenSvg.setAttribute('xmlns', 'http://www.w3.org/2000/svg');
  
        // Rysowanie triangulacji w ukrytym SVG
        triangulation.triangles.forEach(triangle => {
          const points = [
            `${triangle.a.x},${triangle.a.y}`,
            `${triangle.b.x},${triangle.b.y}`,
            `${triangle.c.x},${triangle.c.y}`
          ].join(' ');
  
          const polygon = document.createElementNS('http://www.w3.org/2000/svg', 'polygon');
          polygon.setAttribute('points', points);
          polygon.setAttribute('stroke', 'black');
          polygon.setAttribute('stroke-width', '2');
          polygon.setAttribute('fill', 'none');
          hiddenSvg.appendChild(polygon);
        });
  
        // Rysowanie prostokątów w ukrytym SVG
        rectangles.rectangles.forEach(rectangle => {
          const rect = document.createElementNS('http://www.w3.org/2000/svg', 'rect');
          rect.setAttribute('x', rectangle.x.toString());
          rect.setAttribute('y', rectangle.y.toString());
          rect.setAttribute('width', rectangle.width.toString());
          rect.setAttribute('height', rectangle.height.toString());
          rect.setAttribute('fill', 'rgba(255, 0, 0, 0.5)');
          rect.setAttribute('stroke', 'red');
          rect.setAttribute('stroke-width', '2');
          hiddenSvg.appendChild(rect);
        });
  
        this.calculateCoveragePercentage();
        const coveragePercentage = this.coveragePercentage.toFixed(0); // Zaokrąglone pokrycie do 0 miejsc po przecinku
  
        // Generowanie nazwy pliku na podstawie numeru i procentu pokrycia
        const fileNumber = triangulationIndex
        const fileName = `${fileNumber}_${coveragePercentage}.svg`;
  
        // Serializacja SVG i zapisanie go jako pliku
        const svgData = new XMLSerializer().serializeToString(hiddenSvg);
        const svgBlob = new Blob([svgData], { type: 'image/svg+xml;charset=utf-8' });
        const svgUrl = URL.createObjectURL(svgBlob);
        const downloadLink = document.createElement('a');
        downloadLink.href = svgUrl;
        downloadLink.download = fileName;
        downloadLink.click();
      });
    });
  }
  }

