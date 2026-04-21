import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentCheckinComponent } from './student-checkin';

describe('StudentCheckin', () => {
  let component: StudentCheckinComponent;
  let fixture: ComponentFixture<StudentCheckinComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StudentCheckinComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentCheckinComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
