import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuizDetailComponent } from './quiz-detail';

describe('QuizDetail', () => {
  let component: QuizDetailComponent;
  let fixture: ComponentFixture<QuizDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuizDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(QuizDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
