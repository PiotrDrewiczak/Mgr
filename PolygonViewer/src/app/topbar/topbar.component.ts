import { Component } from '@angular/core';
import { PolygonService } from '../polygon/service/polygon.service';
import { TriangulationService } from '../triangulation/service/triangulation.service';
import { QuadrangleService } from '../quadrangle/service/quadrangle.service';
import { Polygon } from '../polygon/models/polygon.model';
import { Triangulation } from '../triangulation/models/triangulation.model';
import { Rectangles } from '../quadrangle/models/rectangles.model';

@Component({
  selector: 'app-topbar',
  templateUrl: './topbar.component.html',
  styleUrls: ['./topbar.component.css']
})
export class TopbarComponent {
  constructor(
    private polygonService: PolygonService,
    private triangulationService: TriangulationService,
    private quadrangleService: QuadrangleService,
  ) {}

  isLoading: boolean = false;  // Zmienna kontrolująca widoczność spinnera

  generatePolygon() {
    this.polygonService.loadPolygonData().subscribe();
  }

  generateTriangulation() {
    const polygonsData: Polygon[] = this.polygonService.getPolygonsData();
    
    if (polygonsData && polygonsData.length > 0) {
      const polygonArea = polygonsData[0].area;

      this.triangulationService.loadTriangulationData(polygonsData).subscribe((data: Triangulation[]) => {
        
        const triangulationsWithArea = data.map(triangulation => {
          triangulation.area = polygonArea;
          return triangulation;
        });

        this.triangulationService.setTriangulationData(triangulationsWithArea);
      });
    } else {
      console.error('Brak danych wielokątów do wygenerowania triangulacji.');
    }
  }

  generateRectangles() {
    const triangulationData = this.triangulationService.getTriangulationData();
    this.isLoading = true;  
    this.quadrangleService.LoadQuadrangulationData(triangulationData).subscribe((data: Rectangles[][]) => {
      this.quadrangleService.SetQuadrangulationData(data);
      this.isLoading = false;
    }, (error) => {
      this.isLoading = false;
      console.error('Error loading quadrangles:', error);
    });
  }
}
