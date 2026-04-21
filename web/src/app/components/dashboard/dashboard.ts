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

  classrooms: any[] = [];
  isLoading = true;
  errorMessage = '';

  // Class Management State
  showCreateModal = false;
  isEditing = false;
  newClass = { classCRN: '', className: '', classDescription: '' };
  
  // Roster Management State
  showRosterModal = false;
  selectedClassCrn = '';
  studentIdsInput = '';
  isEnrolling = false;
  enrolledStudents: string[] = [];
  isLoadingRoster = false;

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

  // --- CLASSROOM CRUD ---

  openEditModal(cls: any) {
    this.isEditing = true;
    this.newClass = { classCRN: cls.classCRN, className: cls.className, classDescription: cls.classDescription };
    this.showCreateModal = true;
  }

  closeModal() {
    this.showCreateModal = false;
    this.isEditing = false;
    this.newClass = { classCRN: '', className: '', classDescription: '' };
  }

  submitClassForm() {
    if (!this.newClass.classCRN || !this.newClass.className) return;

    if (this.isEditing) {
      this.apiService.updateClassroom(this.newClass.classCRN, this.newClass).subscribe({
        next: () => {
          this.closeModal();
          this.loadClasses();
        },
        error: () => alert('Failed to update class.')
      });
    } else {
      this.apiService.createClassroom(this.newClass).subscribe({
        next: () => {
          this.closeModal();
          this.loadClasses(); 
        },
        error: () => alert('Failed to create class. That CRN might already exist in the database.')
      });
    }
  }

  deleteClassroom(crn: string) {
    if (confirm(`Are you sure you want to delete ${crn}? This will delete all attendance history.`)) {
      this.apiService.deleteClassroom(crn).subscribe({
        next: () => this.loadClasses(),
        error: () => alert('Failed to delete. You may need to clear active sessions first.')
      });
    }
  }

  // --- ROSTER MANAGEMENT ---

  openRosterModal(crn: string) {
    this.selectedClassCrn = crn;
    this.studentIdsInput = '';
    this.showRosterModal = true;
    this.loadRoster();
  }

  loadRoster() {
    this.isLoadingRoster = true;
    this.apiService.getEnrolledStudents(this.selectedClassCrn).subscribe({
      next: (res) => {
        this.enrolledStudents = res;
        this.isLoadingRoster = false;
      },
      error: () => {
        this.isLoadingRoster = false;
      }
    });
  }

  enrollStudents() {
    if (!this.studentIdsInput.trim()) return;
    this.isEnrolling = true;
    
    const rawIds = this.studentIdsInput.split(/[\n,]+/);
    const cleanIds = rawIds.map(id => id.trim()).filter(id => id.length > 0);

    if (cleanIds.length === 0) {
      this.isEnrolling = false;
      return;
    }

    this.apiService.enrollStudents(this.selectedClassCrn, cleanIds).subscribe({
      next: () => {
        this.studentIdsInput = ''; 
        this.isEnrolling = false;
        this.loadRoster(); 
      },
      error: (err: any) => { 
        alert(err.error?.message || 'Failed to enroll students.'); 
        this.isEnrolling = false; 
      }
    });
  }

  removeStudent(studentId: string) {
    if(confirm(`Are you sure you want to remove student ${studentId} from the roster?`)) {
      this.apiService.removeStudent(this.selectedClassCrn, studentId).subscribe({
        next: () => this.loadRoster(),
        error: () => alert('Failed to remove student.')
      });
    }
  }

  logout() {
    this.authService.logout();
  }
}