import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  
  getMyClasses(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Classroom/my-classes`);
  }

  createClassroom(classData: { classCRN: string, className: string, classDescription?: string }): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Classroom/create`, classData);
  }

  enrollStudents(classCrn: string, studentIds: string[]): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Classroom/${classCrn}/enroll`, studentIds);
  }

  
  createSession(sessionData: { classCRN: string, startTime: string, expiryTime: string, requiresImage: boolean }): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Session/create`, sessionData);
  }

  getSessions(classCrn: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Session/${classCrn}`);
  }

  
  checkInStudent(payload: { sessionID: string, studentID: string, imageUrl?: string }): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Attendance/checkin`, payload);
  }

  getAttendanceRecords(sessionId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Attendance/session/${sessionId}`);
  }

  approveAttendance(recordId: string, isApproved: boolean): Observable<any> {
    return this.http.patch<any>(`${this.baseUrl}/Attendance/approve/${recordId}`, { isApproved });
  }
}