import { Component, ElementRef, OnInit, ViewChild, OnDestroy } from '@angular/core';
import * as d3 from 'd3';
import { QuadrangleService } from './service/quadrangle.service';
import { Triangulation } from '../triangulation/models/triangulation.model';
import { TriangulationService } from '../triangulation/service/triangulation.service';
import { Subscription } from 'rxjs';
import { Rectangles } from './models/rectangles.model';
import { PolygonService } from '../polygon/service/polygon.service';
import { Polygon } from '../polygon/models/polygon.model';
import { ExcelExportService } from './service/excel-export.service';

@Component({
  selector: 'app-quadrangle',
  templateUrl: './quadrangle.component.html',
  styleUrls: ['./quadrangle.component.css']
})
export class QuadrangleComponent implements OnInit, OnDestroy {

  @ViewChild('svg', { static: true }) svg!: ElementRef<SVGElement>;
  quadrangles: Rectangles[][] = [];  
  triangulations: Triangulation[] = [];
  subscriptions: Subscription = new Subscription();
  selectedQuadrangleIndex: number = 0;
  coveragePercentage: number = 0;
  maxRectangles: Rectangles | null = null; 
  totalQuadrangleArea: number = 0;  
  totalRectangleArea: number = 0;  

  constructor(
    private polygonService: PolygonService,
    private quadrangleService: QuadrangleService,
    private triangulationService: TriangulationService,
    private excelService: ExcelExportService
  ) {}

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
    if (this.maxRectangles && this.triangulations.length > 0) {
      const totalRectangleArea = this.maxRectangles.totalArea;
      const totalQuadrangleArea = this.triangulations[this.selectedQuadrangleIndex]?.area || 0;
      this.coveragePercentage = (totalRectangleArea / totalQuadrangleArea) * 100;
    } else {
      this.coveragePercentage = 0;
    }
  }

  private findMaxAreaRectangles(rectanglesList: Rectangles[]): Rectangles | null {
    if (!rectanglesList || rectanglesList.length === 0) return null;
  
    return rectanglesList.reduce((max, current) => {
      return current.totalArea > max.totalArea ? current : max;
    }, rectanglesList[0]); 
  }

  private draw(): void {
    const svg = d3.select(this.svg.nativeElement);
    svg.selectAll('*').remove();  
  
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
  
    if (this.maxRectangles && Array.isArray(this.maxRectangles.rectangles)) {
      // Znajdź prostokąt o największej powierzchni
      const largestRectangle = this.maxRectangles.rectangles.reduce((maxRect, currentRect) => {
        return (currentRect.width * currentRect.height) > (maxRect.width * maxRect.height) ? currentRect : maxRect;
      });
  
      // Rysowanie wszystkich prostokątów
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
          .attr("stroke", "red")
          .attr("stroke-width", 2)
          .attr("fill", "red");
      });
      
      svg.append("circle")
        .attr("cx", largestRectangle.centerX)
        .attr("cy", largestRectangle.centerY)
        .attr("r", 5)  // Wielkość kropki
        .attr("fill", "blue");
  
    } else {
      console.error('No rectangles available to draw.');
    }
  }

  selectQuadrangle(): void {
    if (this.quadrangles.length > 0 && this.selectedQuadrangleIndex < this.quadrangles.length) {
      this.maxRectangles = this.findMaxAreaRectangles(this.quadrangles[this.selectedQuadrangleIndex]);
  
      if (this.maxRectangles) {
        this.draw();
        this.calculateCoveragePercentage();
  
        const selectedTriangulation = this.triangulations[this.selectedQuadrangleIndex];
  
        if (selectedTriangulation) {
          this.totalRectangleArea = this.maxRectangles.totalArea;  
          this.totalQuadrangleArea = selectedTriangulation.area || 0;  
          this.coveragePercentage = this.totalQuadrangleArea > 0 ? (this.totalRectangleArea / this.totalQuadrangleArea) * 100 : 0;
        } else {
          console.error('Selected triangulation not found.');
          this.totalRectangleArea = 0;
          this.totalQuadrangleArea = 0;
          this.coveragePercentage = 0;
        }
      } else {
        console.error('Max rectangles not found.');
      }
    } else {
      console.error('Invalid quadrangles data or selected index.');
    }
  }

  saveAllConfigurations(): void {
    if (!this.triangulations || !this.quadrangles) {
      console.error("Brak danych triangulacji lub czworokątów.");
      return;
    }
  
    this.triangulations.forEach((triangulation, triangulationIndex) => {
      const quadrangleList = this.quadrangles[triangulationIndex];
  
      if (!quadrangleList) {
        console.error(`Brak czworokątów dla triangulacji o indeksie ${triangulationIndex}`);
        return;
      }
  
      quadrangleList.forEach((rectangles) => {
        if (!rectangles || !rectangles.rectangles) {
          console.error("Brak danych prostokątów w bieżącej konfiguracji.");
          return;
        }
  
        const totalRectangleArea = rectangles.totalArea || 0;
        const totalPolygonArea = triangulation.area || 0;
  
        const coveragePercentage = totalPolygonArea > 0
          ? (totalRectangleArea / totalPolygonArea) * 100
          : 0;
  
        // Tworzenie ukrytego SVG
        const hiddenSvg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
        hiddenSvg.setAttribute('width', '1600');
        hiddenSvg.setAttribute('height', '1500');
        hiddenSvg.setAttribute('xmlns', 'http://www.w3.org/2000/svg');
  
        // Dodanie białego tła
        const backgroundRect = document.createElementNS('http://www.w3.org/2000/svg', 'rect');
        backgroundRect.setAttribute('x', '0');
        backgroundRect.setAttribute('y', '0');
        backgroundRect.setAttribute('width', '1600');
        backgroundRect.setAttribute('height', '1500');
        backgroundRect.setAttribute('fill', 'white'); // Ustawienie białego tła
        hiddenSvg.appendChild(backgroundRect);
  
        // Rysowanie triangulacji
        if (triangulation.triangles && triangulation.triangles.length > 0) {
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
        } else {
          console.warn(`Brak trójkątów do rysowania dla triangulacji ${triangulationIndex}`);
        }
  
        // Rysowanie prostokątów
        rectangles.rectangles.forEach(rectangle => {
          if (!rectangle) {
            console.warn("Pominięto pusty prostokąt.");
            return;
          }
  
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
  
        // Dodanie tekstu z Coverage Percentage
        const textElement = document.createElementNS('http://www.w3.org/2000/svg', 'text');
        textElement.setAttribute('x', '1590'); // Pozycja X (blisko prawej krawędzi z 10px marginesem)
        textElement.setAttribute('y', '30');  // Pozycja Y
        textElement.setAttribute('font-size', '44');
        textElement.setAttribute('fill', 'black');
        textElement.setAttribute('text-anchor', 'end'); // Wyrównanie do prawej strony
        textElement.textContent = `Coverage: ${coveragePercentage.toFixed(2)}%`;
        hiddenSvg.appendChild(textElement);
  
        // Konwersja SVG do obrazu PNG
        const svgData = new XMLSerializer().serializeToString(hiddenSvg);
        const svgBlob = new Blob([svgData], { type: 'image/svg+xml;charset=utf-8' });
        const svgUrl = URL.createObjectURL(svgBlob);
  
        const image = new Image();
        image.onload = () => {
          const canvas = document.createElement('canvas');
          canvas.width = 1600;
          canvas.height = 1500;
  
          const context = canvas.getContext('2d');
          if (context) {
            // Rysowanie SVG na canvasie
            context.drawImage(image, 0, 0);
  
            // Eksportowanie canvas do PNG
            canvas.toBlob((blob) => {
              if (blob) {
                const fileNumber = triangulationIndex;
                const fileName = `${fileNumber}_${coveragePercentage.toFixed(0)}.png`;
  
                const downloadLink = document.createElement('a');
                downloadLink.href = URL.createObjectURL(blob);
                downloadLink.download = fileName;
                downloadLink.click();
              }
            }, 'image/png');
          }
        };
  
        image.src = svgUrl;
      });
    });
  }
  
  
saveBestConfigurations(): void {
  this.triangulations.forEach((triangulation, triangulationIndex) => {
    const quadrangleList = this.quadrangles[triangulationIndex]; 

    const maxRectangles = quadrangleList.reduce((maxRectangles, currentRectangles) => {
      return currentRectangles.totalArea > maxRectangles.totalArea ? currentRectangles : maxRectangles;
    });

    this.totalRectangleArea = maxRectangles.totalArea;
    this.totalQuadrangleArea = this.triangulations[triangulationIndex]?.area || 0;
    this.coveragePercentage = this.totalQuadrangleArea > 0 ? (this.totalRectangleArea / this.totalQuadrangleArea) * 100 : 0;

    const hiddenSvg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
    hiddenSvg.setAttribute('width', '1600');
    hiddenSvg.setAttribute('height', '1600'); 
    hiddenSvg.setAttribute('xmlns', 'http://www.w3.org/2000/svg');

    // Dodanie białego tła
    const backgroundRect = document.createElementNS('http://www.w3.org/2000/svg', 'rect');
    backgroundRect.setAttribute('x', '0');
    backgroundRect.setAttribute('y', '0');
    backgroundRect.setAttribute('width', '1600');
    backgroundRect.setAttribute('height', '1600');
    backgroundRect.setAttribute('fill', 'white');
    hiddenSvg.appendChild(backgroundRect);

    const textElement = document.createElementNS('http://www.w3.org/2000/svg', 'text');
    textElement.setAttribute('x', '10');  
    textElement.setAttribute('y', '30');  
    textElement.setAttribute('font-size', '24');
    textElement.setAttribute('fill', 'black');

    const textContent = `Coverage: ${this.coveragePercentage.toFixed(2)}%,  Total Polygon Area: ${this.totalQuadrangleArea.toFixed(2)},Total Rectangle Area: ${this.totalRectangleArea.toFixed(2)}`;
    textElement.textContent = textContent; 
    hiddenSvg.appendChild(textElement);

    const drawingOffset = 80;

    triangulation.triangles.forEach(triangle => {
      const points = [
        `${triangle.a.x},${triangle.a.y + drawingOffset}`, 
        `${triangle.b.x},${triangle.b.y + drawingOffset}`,
        `${triangle.c.x},${triangle.c.y + drawingOffset}`
      ].join(' ');

      const polygon = document.createElementNS('http://www.w3.org/2000/svg', 'polygon');
      polygon.setAttribute('points', points);
      polygon.setAttribute('stroke', 'black');
      polygon.setAttribute('stroke-width', '2');
      polygon.setAttribute('fill', 'none');
      hiddenSvg.appendChild(polygon);
    });

    if (maxRectangles && Array.isArray(maxRectangles.rectangles)) {
      maxRectangles.rectangles.forEach(rectangle => {
        const rect = document.createElementNS('http://www.w3.org/2000/svg', 'rect');
        rect.setAttribute('x', rectangle.x.toString());
        rect.setAttribute('y', (rectangle.y + drawingOffset).toString());  // Przesunięcie Y
        rect.setAttribute('width', rectangle.width.toString());
        rect.setAttribute('height', rectangle.height.toString());
        rect.setAttribute('fill', 'rgba(255, 0, 0, 0.5)');
        rect.setAttribute('stroke', 'red');
        rect.setAttribute('stroke-width', '2');
        hiddenSvg.appendChild(rect);
      });
    } else {
      console.error('No rectangles available to save.');
      return;
    }

    const svgData = new XMLSerializer().serializeToString(hiddenSvg);
    const svgBlob = new Blob([svgData], { type: 'image/svg+xml;charset=utf-8' });
    const svgUrl = URL.createObjectURL(svgBlob);

    const image = new Image();
    image.onload = () => {
      const canvas = document.createElement('canvas');
      canvas.width = 1600;
      canvas.height = 1600;
      const context = canvas.getContext('2d');

      // Rysowanie SVG na canvasie
      context?.drawImage(image, 0, 0);

      // Eksportowanie jako PNG
      canvas.toBlob((blob) => {
        if (blob) {
          const downloadLink = document.createElement('a');
          downloadLink.href = URL.createObjectURL(blob);
          downloadLink.download = `triangulation_${triangulationIndex}_best_rectangles.png`;
          downloadLink.click();
        }
      }, 'image/png');
    };

    image.src = svgUrl;
  });
}
saveToExcel(): void {
  const polygonsData: Polygon[] = this.polygonService.getPolygonsData();

  const rectangleCenters = this.quadrangles.map((rectangles, index) => {
    const maxRectangles = this.findMaxAreaRectangles(rectangles);
    if (maxRectangles && maxRectangles.rectangles.length > 0) {
      const largestRectangle = maxRectangles.rectangles.reduce((maxRect, currentRect) => {
        return (currentRect.width * currentRect.height) > (maxRect.width * maxRect.height) ? currentRect : maxRect;
      });

      return { 
        polygonIndex: index,
        centerX: largestRectangle.x + largestRectangle.width / 2,
        centerY: largestRectangle.y + largestRectangle.height / 2
      };
    }
    return null;
  }).filter(center => center !== null);

  // Sprawdzenie subskrypcji
  this.excelService.exportToExcel(polygonsData, rectangleCenters)
    .subscribe({
      next: response => console.log('Export successful:', response),
      error: error => console.error('Export failed:', error)  // Sprawdzanie błędów w logach
    });
}
}

