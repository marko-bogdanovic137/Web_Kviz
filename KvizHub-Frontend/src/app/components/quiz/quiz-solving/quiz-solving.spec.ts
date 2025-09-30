import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuizSolving } from './quiz-solving';

describe('QuizSolving', () => {
  let component: QuizSolving;
  let fixture: ComponentFixture<QuizSolving>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuizSolving]
    })
    .compileComponents();

    fixture = TestBed.createComponent(QuizSolving);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
