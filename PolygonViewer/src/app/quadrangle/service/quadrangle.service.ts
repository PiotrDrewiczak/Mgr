import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';
import { Triangulation } from '../../triangulation/models/triangulation.model';
import { BehaviorSubject, tap } from 'rxjs';

import { Rectangles } from '../models/rectangles.model';


@Injectable({
  providedIn: 'root'
})
export class QuadrangleService {

    private apiUrl = 'http://localhost:5136/quadrulation/quadrulate';
    private quadrulationDataSubject = new BehaviorSubject<Rectangles[]>([]);
    quadrulationData$ = this.quadrulationDataSubject.asObservable();

  constructor(private http: HttpClient) { }

  loadQuadrulationData(triangulation: Triangulation[]): Observable<Rectangles[]> {
    const url = `${this.apiUrl}`;
    return this.http.post<Rectangles[]>(url, triangulation).pipe(
      tap(data => {
        console.log('Received data:', data);  // Log the received data
        this.quadrulationDataSubject.next(data);
      })
    );
  }

  setQuadrangleData(data: Rectangles[]) {
    this.quadrulationDataSubject.next(data);
  }

  getQuadrangleData(): Rectangles[] {
    return this.quadrulationDataSubject.value;
  }
}
  