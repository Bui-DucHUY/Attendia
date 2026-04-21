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
  
  status: 'idle' | 'loading' | 'success' | 'error' = 'idle';
  errorMessage: string = '';

  stream: MediaStream | null = null;
  cameraReady: boolean = false;

  ngOnInit() {
    this.sessionId = this.route.snapshot.paramMap.get('sessionId') || '';
    this.startCamera();
  }

  ngOnDestroy() {
    this.stopCamera();
  }

  async startCamera() {
    try {
      // Request front-facing camera
      this.stream = await navigator.mediaDevices.getUserMedia({ video: { facingMode: 'user' }, audio: false });
      if (this.videoFeed) {
        this.videoFeed.nativeElement.srcObject = this.stream;
        this.cameraReady = true;
      }
    } catch (err) {
      console.error("Camera access denied", err);
      this.errorMessage = "Camera access is strictly required to check in.";
      this.status = 'error';
    }
  }

  stopCamera() {
    if (this.stream) {
      this.stream.getTracks().forEach(track => track.stop());
    }
  }

  submitCheckIn() {
    if (!this.studentId || !this.sessionId || !this.cameraReady) return;
    
    this.status = 'loading';
    this.errorMessage = '';
    
    // Capture the current frame from the video feed to the hidden canvas
    const video = this.videoFeed.nativeElement;
    const canvas = this.canvas.nativeElement;
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    const ctx = canvas.getContext('2d');
    ctx?.drawImage(video, 0, 0, canvas.width, canvas.height);
    
    // Compress it into a lightweight Base64 string
    const base64Image = canvas.toDataURL('image/jpeg', 0.6);

    const payload = {
      sessionID: this.sessionId,
      studentID: this.studentId,
      imageUrl: base64Image 
    };

    this.apiService.checkInStudent(payload).subscribe({
      next: (res: any) => {
        this.status = 'success';
        this.stopCamera(); // Turn off their webcam light immediately
      },
      error: (err: any) => {
        this.status = 'error';
        this.errorMessage = err.error?.message || 'Check-in failed. Please try again.';
      }
    });
  }
}