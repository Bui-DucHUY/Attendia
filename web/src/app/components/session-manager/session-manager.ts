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

  ngOnInit() {
    this.classCrn = this.route.snapshot.paramMap.get('crn') || '';
    if (!this.classCrn) {
      this.router.navigate(['/dashboard']);
    }
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
      next: (res: any) => { // <-- Add ': any' here
        this.activeSessionId = res.sessionID; 
        
        // Constructs the exact local network URL for the students' phones
        this.qrCodeUrl = `${window.location.origin}/#/checkin/${this.activeSessionId}`;
        
        this.isSessionActive = true;
        this.isLoading = false;
      },
      error: (err: any) => { 
        this.errorMessage = 'Failed to create session. Ensure the API is running.';
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