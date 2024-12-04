import { Component } from '@angular/core';
import { PolygonService } from '../polygon/service/polygon.service';
import { TriangulationService } from '../triangulation/service/triangulation.service';
import { QuadrangleService } from '../quadrangle/service/quadrangle.service';
import { Polygon } from '../polygon/models/polygon.model';
import { Triangulation } from '../triangulation/models/triangulation.model';
import { Rectangles } from '../quadrangle/models/rectangles.model';
import { PredictionService } from '../prediction/service/prediction.service';
import { PolygonWithCenter } from '../prediction/models/PolygonWithCenter.model';

@Component({
  selector: 'app-topbar',
  templateUrl: './topbar.component.html',
  styleUrls: ['./topbar.component.css']
})
export class TopbarComponent {
  isLoading: boolean = false; // Zmienna kontrolująca widoczność spinnera
  centerData: PolygonWithCenter[] = []; // Dane z obliczeniami środka

  constructor(
    private polygonService: PolygonService,
    private triangulationService: TriangulationService,
    private quadrangleService: QuadrangleService,
    private predictionService: PredictionService
  ) {}

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
    this.quadrangleService.LoadQuadrangulationData(triangulationData).subscribe({
      next: (data: Rectangles[][]) => {
        this.quadrangleService.SetQuadrangulationData(data);
        this.isLoading = false;
      },
      error: (error) => {
        this.isLoading = false;
        console.error('Error loading quadrangles:', error);
      }
    });
  }

  calculateCenter() {
    const polygonsData: Polygon[] = this.polygonService.getPolygonsData();

    if (polygonsData && polygonsData.length > 0) {
      this.isLoading = true; 
      this.predictionService.loadCenterData(polygonsData).subscribe({
        next: (data: PolygonWithCenter[]) => {
          console.log('Center data:', data);
          this.centerData = data; 
          this.isLoading = false; 
        },
        error: (error) => {
          console.error('Error calculating center:', error); 
          this.isLoading = false;
        }
      });
    } else {
      console.error('Brak danych wielokątów do obliczenia środka.');
    }
  }
}
