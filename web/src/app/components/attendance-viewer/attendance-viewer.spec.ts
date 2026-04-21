import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AttendanceViewer } from './attendance-viewer';

describe('AttendanceViewer', () => {
  let component: AttendanceViewer;
  let fixture: ComponentFixture<AttendanceViewer>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AttendanceViewer]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AttendanceViewer);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
