import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Triangulation } from '../models/triangulation.model';
import { Polygon } from '../../polygon/models/polygon.model';

@Injectable({
  providedIn: 'root'
})
export class TriangulationService {
  private apiUrl = 'http://localhost:5136/triangulation/triangulate';
  private triangulationDataSubject = new BehaviorSubject<Triangulation[]>([]);
  triangulationData$ = this.triangulationDataSubject.asObservable();

  constructor(private http: HttpClient) { }

  loadTriangulationData(polygon: Polygon[]): Observable<Triangulation[]> {
     return this.http.post<Triangulation[]>(this.apiUrl, polygon).pipe(
      tap(data => {
        this.triangulationDataSubject.next(data);
      })
    );
  }

  setTriangulationData(data: Triangulation[]) {
    this.triangulationDataSubject.next(data);
  }

  getTriangulationData(): Triangulation[] {
    return this.triangulationDataSubject.value;
  }

}
