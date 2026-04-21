import { Component, OnInit, OnDestroy, inject, ViewChild, ElementRef } from '@angular/core';
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
export class StudentCheckinComponent implements OnInit, OnDestroy {
  @ViewChild('videoFeed') videoFeed!: ElementRef<HTMLVideoElement>;
  @ViewChild('canvas') canvas!: ElementRef<HTMLCanvasElement>;

  private route = inject(ActivatedRoute);
  private apiService = inject(ApiService);

  sessionId: string = '';
  studentId: string = '';
  sessionDetails: any = null; // Holds the public rules
  
  status: 'idle' | 'loading' | 'success' | 'error' = 'idle';
  errorMessage: string = '';

  stream: MediaStream | null = null;
  cameraReady: boolean = false;

  ngOnInit() {
    this.sessionId = this.route.snapshot.paramMap.get('sessionId') || '';
    
    // Fetch rules before doing anything
    this.apiService.getPublicSession(this.sessionId).subscribe({
      next: (res) => {
        this.sessionDetails = res;
        if (res.isExpired) {
          this.status = 'error';
          this.errorMessage = 'This session has expired and is closed.';
        } else if (res.requiresImage) {
          this.startCamera(); // Only boot camera if required!
        }
      },
      error: () => {
        this.status = 'error';
        this.errorMessage = 'Invalid or missing session.';
      }
    });
  }

  ngOnDestroy() { this.stopCamera(); }

  async startCamera() {
    try {
      this.stream = await navigator.mediaDevices.getUserMedia({ video: { facingMode: 'user' }, audio: false });
      if (this.videoFeed) {
        this.videoFeed.nativeElement.srcObject = this.stream;
        this.cameraReady = true;
      }
    } catch (err) {
      this.errorMessage = "Camera access is strictly required for this class.";
      this.status = 'error';
    }
  }

  stopCamera() {
    if (this.stream) this.stream.getTracks().forEach(track => track.stop());
  }

  submitCheckIn() {
    if (!this.studentId || !this.sessionId || !this.sessionDetails) return;
    
    this.status = 'loading';
    this.errorMessage = '';
    let base64Image = '';

    // Only capture photo if required
    if (this.sessionDetails.requiresImage) {
      if (!this.cameraReady) return;
      const video = this.videoFeed.nativeElement;
      const canvas = this.canvas.nativeElement;
      canvas.width = video.videoWidth;
      canvas.height = video.videoHeight;
      const ctx = canvas.getContext('2d');
      ctx?.drawImage(video, 0, 0, canvas.width, canvas.height);
      base64Image = canvas.toDataURL('image/jpeg', 0.6);
    }

    const payload = {
      sessionID: this.sessionId,
      studentID: this.studentId,
      imageUrl: base64Image 
    };

    this.apiService.checkInStudent(payload).subscribe({
      next: () => {
        this.status = 'success';
        this.stopCamera(); 
      },
      error: (err: any) => {
        this.status = 'error';
        this.errorMessage = err.error?.message || 'Check-in failed.';
      }
    });
  }
}