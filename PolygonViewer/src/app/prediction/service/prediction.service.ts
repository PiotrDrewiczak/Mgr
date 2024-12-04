import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { PolygonWithCenter } from '../models/PolygonWithCenter.model';
import { Polygon } from '../../polygon/models/polygon.model';


@Injectable({
  providedIn: 'root'
})
export class PredictionService {
  private apiUrl = 'http://localhost:5136/prediction/calculatecenter';
  private centerDataSubject = new BehaviorSubject<PolygonWithCenter[]>([]);
  centerData$ = this.centerDataSubject.asObservable();

  constructor(private http: HttpClient) {}

  loadCenterData(polygon: Polygon[]): Observable<PolygonWithCenter[]> {
    return this.http.post<PolygonWithCenter[]>(this.apiUrl, polygon).pipe(
     tap(data => {
       this.centerDataSubject.next(data);
     })
   );
 }
  
}
