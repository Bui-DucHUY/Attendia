import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { QRCodeComponent } from 'angularx-qrcode';
import { ApiService } from '../../services/api'; // We will scaffold this service next if you haven't fully built it

@Component({
  selector: 'app-session-manager',
  standalone: true,
  imports: [CommonModule, FormsModule, QRCodeComponent],
  templateUrl: './session-manager.html'
})
export class SessionManagerComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  // private apiService = inject(ApiService); // Uncomment when your API service is wired

  classCrn: string = '';
  
  // Session Configuration
  durationMinutes: number = 15;
  requiresImage: boolean = true;
  
  // Active Session State
  isSessionActive: boolean = false;
  activeSessionId: string | null = null;
  qrCodeUrl: string = '';
  
  errorMessage: string = '';
  isLoading: boolean = false;

  ngOnInit() {
    // Grab the ClassCRN from the URL (e.g., /session/CSC101)
    this.classCrn = this.route.snapshot.paramMap.get('crn') || '';
    if (!this.classCrn) {
      this.router.navigate(['/dashboard']);
    }
  }

  startSession() {
    this.isLoading = true;
    this.errorMessage = '';

    // Calculate expiry time
    const startTime = new Date();
    const expiryTime = new Date(startTime.getTime() + this.durationMinutes * 60000);

    const sessionPayload = {
      ClassCRN: this.classCrn,
      StartTime: startTime.toISOString(),
      ExpiryTime: expiryTime.toISOString(),
      RequiresImage: this.requiresImage
    };

    /* // TODO: Wire this to your actual ApiService
    this.apiService.createSession(sessionPayload).subscribe({
      next: (res) => {
        this.activeSessionId = res.SessionID;
        // Construct the URL the student's phone will actually visit
        // Using window.location.origin dynamically grabs your local IP or production domain
        this.qrCodeUrl = `${window.location.origin}/checkin/${this.activeSessionId}`;
        this.isSessionActive = true;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = 'Failed to create session. Please try again.';
        this.isLoading = false;
      }
    });
    */

    // --- TEMPORARY MOCK FOR UI TESTING ---
    setTimeout(() => {
      this.activeSessionId = 'mock-guid-1234-5678';
      this.qrCodeUrl = `${window.location.origin}/checkin/${this.activeSessionId}`;
      this.isSessionActive = true;
      this.isLoading = false;
    }, 500);
  }

  endSession() {
    // In a full implementation, you might want to call an endpoint here to instantly force-expire the session in the DB
    this.isSessionActive = false;
    this.activeSessionId = null;
    this.qrCodeUrl = '';
  }
}