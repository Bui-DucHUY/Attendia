import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ApiService } from '../../services/api';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './dashboard.html'
})
export class DashboardComponent implements OnInit {
  private apiService = inject(ApiService);
  public authService = inject(AuthService); // Public so HTML can read the instructor name
  private router = inject(Router);

  classrooms: any[] = [];
  isLoading = true;
  errorMessage = '';

  // Form states
  showCreateModal = false;
  newClass = { classCRN: '', className: '', classDescription: '' };
  
  ngOnInit() {
    this.loadClasses();
  }

  loadClasses() {
    this.isLoading = true;
    this.apiService.getMyClasses().subscribe({
      next: (data: any) => {
        this.classrooms = data;
        this.isLoading = false;
      },
      error: (err: any) => {
        this.errorMessage = 'Failed to load classrooms. Ensure the API is running.';
        this.isLoading = false;
      }
    });
  }

  createClass() {
    if (!this.newClass.classCRN || !this.newClass.className) return;

    this.apiService.createClassroom(this.newClass).subscribe({
      next: () => {
        this.showCreateModal = false;
        this.newClass = { classCRN: '', className: '', classDescription: '' };
        this.loadClasses(); // Refresh the grid
      },
      error: (err: any) => {
        alert('Failed to create class. That CRN might already exist in the database.');
      }
    });
  }

  logout() {
    this.authService.logout();
  }
}