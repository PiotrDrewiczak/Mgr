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

  // Zmieniamy typ danych na List<List<Rectangles>>
  private apiUrl = 'http://localhost:5136/quadrulation/quadrulate';
  private quadrulationDataSubject = new BehaviorSubject<Rectangles[][]>([]); // Lista list prostokątów
  quadrulationData$ = this.quadrulationDataSubject.asObservable();

  constructor(private http: HttpClient) { }

  // Metoda zwracająca List<List<Rectangles>> dla każdej triangulacji
  loadQuadrulationData(triangulation: Triangulation[]): Observable<Rectangles[][]> {
    const url = `${this.apiUrl}`;
    return this.http.post<Rectangles[][]>(url, triangulation).pipe(
      tap(data => {
        console.log('Received data:', data);  // Logowanie odebranych danych
        this.quadrulationDataSubject.next(data);  // Ustawianie pełnych danych
      })
    );
  }

  // Ustawianie danych - lista list prostokątów
  setQuadrangleData(data: Rectangles[][]) {
    this.quadrulationDataSubject.next(data);
  }

  // Pobieranie danych - zwraca pełną listę list prostokątów
  getQuadrangleData(): Rectangles[][] {
    return this.quadrulationDataSubject.value;
  }
}
