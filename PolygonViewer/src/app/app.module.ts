import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { PolygonComponent } from './polygon/polygon.component';
import { HttpClientModule } from '@angular/common/http';
import { PolygonService } from './polygon/service/polygon.service';
import { FormsModule } from '@angular/forms';
import { TopbarComponent } from './topbar/topbar.component';

import { TriangulationComponent } from './triangulation/triangulation.component';
import { TriangulationService } from './triangulation/service/triangulation.service';
import { QuadrangleComponent } from './quadrangle/quadrangle.component';
import { QuadrangleService } from './quadrangle/service/quadrangle.service';
import { ExcelExportService } from './quadrangle/service/excel-export.service';

@NgModule({
  declarations: [
    AppComponent,
    PolygonComponent,
    TopbarComponent,
    TriangulationComponent,
    QuadrangleComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule 
  ],
  providers: [PolygonService,TriangulationService,QuadrangleService,ExcelExportService],
  bootstrap: [AppComponent]
})
export class AppModule { }
