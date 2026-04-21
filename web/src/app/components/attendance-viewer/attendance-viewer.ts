import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ApiService } from '../../services/api';
import { Location } from '@angular/common';

@Component({
  selector: 'app-attendance-viewer',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './attendance-viewer.html'
})
export class AttendanceViewerComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private apiService = inject(ApiService);
  public location = inject(Location); 

  sessionId: string = '';
  records: any[] = [];
  isLoading: boolean = true;
  
  // Image Viewer State
  selectedImage: string | null = null;

  ngOnInit() {
    this.sessionId = this.route.snapshot.paramMap.get('sessionId') || '';
    this.loadRecords();
  }

  loadRecords() {
    this.apiService.getAttendanceRecords(this.sessionId).subscribe({
      next: (data: any[]) => {
        this.records = data.map(r => ({
          ...r,
          // Force UTC mapping for correct local time calculation
          checkInTime: r.checkInTime.endsWith('Z') ? r.checkInTime : r.checkInTime + 'Z'
        }));
        this.isLoading = false;
      },
      error: (err: any) => {
        console.error('Failed to load records', err);
        this.isLoading = false;
      }
    });
  }

  toggleApproval(record: any) {
    const newStatus = !record.isApproved;
    record.isApproved = newStatus; 
    
    this.apiService.approveAttendance(record.recordID, newStatus).subscribe({
      error: (err: any) => {
        record.isApproved = !newStatus;
        alert('Failed to update status.');
      }
    });
  }

  viewPhoto(imageUrl: string) {
    this.selectedImage = imageUrl;
  }
}