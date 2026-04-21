import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { QRCodeComponent } from 'angularx-qrcode';
import { ApiService } from '../../services/api'; 

@Component({
  selector: 'app-session-manager',
  standalone: true,
  imports: [CommonModule, FormsModule, QRCodeComponent, RouterModule],
  templateUrl: './session-manager.html'
})
export class SessionManagerComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private apiService = inject(ApiService); 

  classCrn: string = '';
  durationMinutes: number = 15;
  requiresImage: boolean = true;
  
  isSessionActive: boolean = false;
  activeSessionId: string | null = null;
  qrCodeUrl: string = '';
  errorMessage: string = '';
  isLoading: boolean = false;

  pastSessions: any[] = [];
  isLoadingHistory: boolean = false;

  ngOnInit() {
    this.classCrn = this.route.snapshot.paramMap.get('crn') || '';
    if (!this.classCrn) {
      this.router.navigate(['/dashboard']);
      return;
    }
    this.loadPastSessions();
  }

  loadPastSessions() {
    this.isLoadingHistory = true;
    this.apiService.getSessions(this.classCrn).subscribe({
      next: (res: any[]) => {
        this.pastSessions = res.map(s => ({
          ...s,
          // Force Angular to recognize this as UTC so it automatically converts to local time
          startTime: s.startTime.endsWith('Z') ? s.startTime : s.startTime + 'Z'
        })).sort((a, b) => new Date(b.startTime).getTime() - new Date(a.startTime).getTime());
        this.isLoadingHistory = false;
      },
      error: (err: any) => {
        console.error('Failed to load history', err);
        this.isLoadingHistory = false;
      }
    });
  }

  startSession() {
    this.isLoading = true;
    this.errorMessage = '';

    const startTime = new Date();
    const expiryTime = new Date(startTime.getTime() + this.durationMinutes * 60000);

    const sessionPayload = {
      classCRN: this.classCrn,
      startTime: startTime.toISOString(),
      expiryTime: expiryTime.toISOString(),
      requiresImage: this.requiresImage
    };

    this.apiService.createSession(sessionPayload).subscribe({
      next: (res: any) => {
        this.activeSessionId = res.sessionID; 
        this.qrCodeUrl = `${window.location.origin}/#/checkin/${this.activeSessionId}`;
        this.isSessionActive = true;
        this.isLoading = false;
        this.loadPastSessions(); 
      },
      error: (err: any) => {
        this.errorMessage = err.error?.message || 'Failed to create session.';
        this.isLoading = false;
      }
    });
  }

  endSession() {
    this.isSessionActive = false;
    this.activeSessionId = null;
    this.qrCodeUrl = '';
  }
}