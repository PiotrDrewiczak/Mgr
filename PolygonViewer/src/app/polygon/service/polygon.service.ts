import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Polygon } from '../models/polygon.model';

@Injectable({
  providedIn: 'root'
})
export class PolygonService {
  private polygonsSubject = new BehaviorSubject<Polygon[] | null>(null);
  private apiUrl = 'http://localhost:5136/polygon';
  polygonsData$ = this.polygonsSubject.asObservable();

  constructor(private http: HttpClient) {}

  loadPolygonData(): Observable<Polygon[]> {
    return this.http.get<Polygon[]>(this.apiUrl).pipe(
      tap(data => {
        this.polygonsSubject.next(data);
      })
    );
  }

  getPolygonsData(): Polygon[] {
    return this.polygonsSubject.value || [];
  }
  
}
