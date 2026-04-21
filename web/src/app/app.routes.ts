import { Routes } from '@angular/router';

import { LoginComponent } from './components/login/login';
import { RegisterComponent } from './components/register/register';
import { DashboardComponent } from './components/dashboard/dashboard';
import { SessionManagerComponent } from './components/session-manager/session-manager';
import { StudentCheckinComponent } from './components/student-checkin/student-checkin';
import { AttendanceViewerComponent } from './components/attendance-viewer/attendance-viewer';
import { authGuard } from './guards/auth-guard'; 

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'checkin/:sessionId', component: StudentCheckinComponent },
  
  // Protected Routes
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'session/:crn', component: SessionManagerComponent, canActivate: [authGuard] },
  { path: 'attendance/:sessionId', component: AttendanceViewerComponent, canActivate: [authGuard] }, // <-- NEW

  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];