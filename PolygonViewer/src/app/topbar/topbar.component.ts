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
    private quadrangleService: QuadrangleService
  ) {}

  generatePolygon() {
    this.polygonService.loadPolygonData().subscribe();
  }

  generateTriangulation() {
    const polygonsData: Polygon[] = this.polygonService.getPolygonsData(); // Pobierz listę wielokątów
    this.triangulationService.loadTriangulationData(polygonsData).subscribe((data: Triangulation[]) => {
      this.triangulationService.setTriangulationData(data);
    });
  }

  generateRectangles() {
    const triangulationData = this.triangulationService.getTriangulationData();
    this.quadrangleService.loadQuadrulationData(triangulationData).subscribe((data: Rectangles[][]) => {
      this.quadrangleService.setQuadrangleData(data);
    });
  }
}
