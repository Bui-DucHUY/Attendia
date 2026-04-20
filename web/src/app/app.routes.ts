import { Routes } from '@angular/router';
import { Login } from './components/login/login';
import { Register } from './components/register/register';
import { Dashboard } from './components/dashboard/dashboard';
import { SessionManager } from './components/session-manager/session-manager';
import { StudentCheckin } from './components/student-checkin/student-checkin';
import { authGuard } from './guards/auth-guard';
export const routes: Routes = [
  // Public Routes
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  
  // Mobile Student Route (Public, but requires specific Session ID)
  { path: 'checkin/:sessionId', component: StudentCheckin },

  // Protected Instructor Routes
  { path: 'dashboard', component: Dashboard, canActivate: [authGuard] },
  { path: 'session/:crn', component: SessionManager, canActivate: [authGuard] },

  // Default Fallbacks
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'login' }
];