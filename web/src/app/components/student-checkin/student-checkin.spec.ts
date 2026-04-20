import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentCheckin } from './student-checkin';

describe('StudentCheckin', () => {
  let component: StudentCheckin;
  let fixture: ComponentFixture<StudentCheckin>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StudentCheckin]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentCheckin);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
