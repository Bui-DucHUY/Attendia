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
  public authService = inject(AuthService); 
  private router = inject(Router);
  isEditing = false;

  classrooms: any[] = [];
  isLoading = true;
  errorMessage = '';

  // Create Class State
  showCreateModal = false;
  newClass = { classCRN: '', className: '', classDescription: '' };
  
  // Roster Import State
  showRosterModal = false;
  selectedClassCrn = '';
  studentIdsInput = '';
  isEnrolling = false;

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
        this.loadClasses(); 
      },
      error: (err: any) => {
        alert('Failed to create class. That CRN might already exist in the database.');
      }
    });
  }

  openRosterModal(crn: string) {
    this.selectedClassCrn = crn;
    this.studentIdsInput = '';
    this.showRosterModal = true;
  }
  openEditModal(cls: any) {
    this.isEditing = true;
    this.newClass = { classCRN: cls.classCRN, className: cls.className, classDescription: cls.classDescription };
    this.showCreateModal = true;
  }

  submitClassForm() {
    if (!this.newClass.classCRN || !this.newClass.className) return;

    if (this.isEditing) {
      this.apiService.updateClassroom(this.newClass.classCRN, this.newClass).subscribe(() => {
        this.closeModal();
        this.loadClasses();
      });
    } else {
      this.apiService.createClassroom(this.newClass).subscribe(() => {
        this.closeModal();
        this.loadClasses();
      });
    }
  }

  deleteClassroom(crn: string) {
    if (confirm(`Type OK to delete ${crn}. This will delete all history.`)) {
      this.apiService.deleteClassroom(crn).subscribe({
        next: () => this.loadClasses(),
        error: () => alert('Failed to delete. Clear sessions first.')
      });
    }
  }

  closeModal() {
    this.showCreateModal = false;
    this.isEditing = false;
    this.newClass = { classCRN: '', className: '', classDescription: '' };
  }

  enrollStudents() {
    if (!this.studentIdsInput.trim()) return;

    this.isEnrolling = true;
    
    // Parse the input: split by commas or newlines, remove whitespace, and filter out empties
    const rawIds = this.studentIdsInput.split(/[\n,]+/);
    const cleanIds = rawIds.map(id => id.trim()).filter(id => id.length > 0);

    if (cleanIds.length === 0) {
      this.isEnrolling = false;
      return;
    }

    this.apiService.enrollStudents(this.selectedClassCrn, cleanIds).subscribe({
      next: (res: any) => {
        alert(res.message || `Successfully enrolled ${cleanIds.length} students!`);
        this.showRosterModal = false;
        this.isEnrolling = false;
      },
      error: (err: any) => {
        alert(err.error?.message || 'Failed to enroll students.');
        this.isEnrolling = false;
      }
    });
  }

  logout() {
    this.authService.logout();
  }
}