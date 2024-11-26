import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Polygon } from '../../polygon/models/polygon.model';


@Injectable({
  providedIn: 'root'
})
export class ExcelExportService {

  private apiUrl = 'http://localhost:5136/excelexport/export'; 
  
  constructor(private http: HttpClient) {}
  
  exportToExcel(polygons: Polygon[], rectangleCenters: any[]): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    const body = { polygons, rectangleCenters };
  
    console.log('Wysyłane dane:', body); // Zalogowanie danych
  
    return this.http.post(this.apiUrl, body, { headers });
  }
}
