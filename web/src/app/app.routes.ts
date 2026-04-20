import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login';
import { RegisterComponent } from './components/register/register';
import { DashboardComponent } from './components/dashboard/dashboard';
import { SessionManagerComponent } from './components/session-manager/session-manager';
import { StudentCheckinComponent } from './components/student-checkin/student-checkin';
import { authGuard } from './guards/auth.guard'; // We will create this next

export const routes: Routes = [
  // Public Routes
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  
  // Mobile Student Route (Public, but requires specific Session ID)
  { path: 'checkin/:sessionId', component: StudentCheckinComponent },

  // Protected Instructor Routes
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'session/:crn', component: SessionManagerComponent, canActivate: [authGuard] },

  // Default Fallbacks
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'login' }
];