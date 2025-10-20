import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuizSolvingComponent } from './quiz-solving';

describe('QuizSolving', () => {
  let component: QuizSolvingComponent;
  let fixture: ComponentFixture<QuizSolvingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuizSolvingComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(QuizSolvingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
