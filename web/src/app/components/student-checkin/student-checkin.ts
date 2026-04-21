import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-student-checkin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './student-checkin.html'
})
export class StudentCheckinComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private apiService = inject(ApiService);

  sessionId: string = '';
  studentId: string = '';
  
  status: 'idle' | 'loading' | 'success' | 'error' = 'idle';
  errorMessage: string = '';

  ngOnInit() {
    // Grab the active session ID from the URL that the QR code generated
    this.sessionId = this.route.snapshot.paramMap.get('sessionId') || '';
  }

  submitCheckIn() {
    if (!this.studentId || !this.sessionId) return;
    
    this.status = 'loading';
    this.errorMessage = '';
    
    const payload = {
      sessionID: this.sessionId,
      studentID: this.studentId,
      imageUrl: '' // For the MVP, we will bypass the camera capture
    };

    this.apiService.checkInStudent(payload).subscribe({
      next: (res: any) => {
        this.status = 'success';
      },
      error: (err: any) => {
        this.status = 'error';
        // Display the exact error from your .NET backend (e.g., "Session expired")
        this.errorMessage = err.error?.message || 'Check-in failed. Please try again.';
      }
    });
  }
}