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

  // --- CLASSROOM MANAGEMENT ---
  getMyClasses(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Classroom/my-classes`);
  }

  createClassroom(classData: { classCRN: string, className: string, classDescription?: string }): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Classroom/create`, classData);
  }

  updateClassroom(classCrn: string, classData: any): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/Classroom/${classCrn}`, classData);
  }

  deleteClassroom(classCrn: string): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/Classroom/${classCrn}`);
  }

  // --- ROSTER MANAGEMENT ---
  enrollStudents(classCrn: string, studentIds: string[]): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Classroom/${classCrn}/enroll`, studentIds);
  }

  getEnrolledStudents(classCrn: string): Observable<string[]> { 
    return this.http.get<string[]>(`${this.baseUrl}/Classroom/${classCrn}/roster`); 
  }
  
  removeStudent(classCrn: string, studentId: string): Observable<any> { 
    return this.http.delete<any>(`${this.baseUrl}/Classroom/${classCrn}/roster/${studentId}`); 
  }

  // --- SESSION MANAGEMENT ---
  createSession(sessionData: { classCRN: string, startTime: string, expiryTime: string, requiresImage: boolean }): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Session/create`, sessionData);
  }

  getSessions(classCrn: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Session/class/${classCrn}`);
  }

  deleteSession(sessionId: string): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/Session/${sessionId}`);
  }

  // --- NEW: Connects to your backend patch ---
  endSessionEarly(sessionId: string): Observable<any> {
    return this.http.patch<any>(`${this.baseUrl}/Session/${sessionId}/end`, {});
  }

  getPublicSession(sessionId: string): Observable<any> { 
    return this.http.get<any>(`${this.baseUrl}/Session/public/${sessionId}`); 
  }

  // --- ATTENDANCE MANAGEMENT ---
  checkInStudent(payload: { sessionID: string, studentID: string, imageUrl?: string }): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Attendance/checkin`, payload);
  }

  getAttendanceRecords(sessionId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Attendance/session/${sessionId}`);
  }

  approveAttendance(recordId: string, isApproved: boolean): Observable<any> {
    return this.http.patch<any>(`${this.baseUrl}/Attendance/approve/${recordId}`, { isApproved });
  }

  deleteAttendanceRecord(recordId: string): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/Attendance/${recordId}`);
  }
}