import { ComponentFixture, TestBed } from '@angular/core/testing';
import { QuadrangleComponent } from './quadrangle.component';



describe('QuadrangleComponent', () => {
  let component: QuadrangleComponent;
  let fixture: ComponentFixture<QuadrangleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [QuadrangleComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(QuadrangleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
