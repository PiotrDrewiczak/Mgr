import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Triangulation } from '../../triangulation/models/triangulation.model';
import { BehaviorSubject, tap } from 'rxjs';
import { Rectangles } from '../models/rectangles.model';

@Injectable({
  providedIn: 'root'
})
export class QuadrangleService {

  private apiUrl = 'http://localhost:5136/Quadrangulation/quadrangulate';
  private quadrulationDataSubject = new BehaviorSubject<Rectangles[][]>([]);
  quadrulationData$ = this.quadrulationDataSubject.asObservable();

  constructor(private http: HttpClient) { }

  LoadQuadrangulationData(triangulation: Triangulation[]): Observable<Rectangles[][]> {
    const url = `${this.apiUrl}`;
    return this.http.post<Rectangles[][]>(url, triangulation).pipe(
      tap(data => {
        console.log('Received data:', data);
        this.quadrulationDataSubject.next(data);
      })
    );
  }

  SetQuadrangulationData(data: Rectangles[][]) {
    this.quadrulationDataSubject.next(data);
  }

  GetQuadrangulationData(): Rectangles[][] {
    return this.quadrulationDataSubject.value;
  }
}
