import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'auth',
    children: [
      {
        path: 'login',
        loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
      },
      {
        path: 'register',
        loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)
      }
    ]
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
  },
  {
    path: 'kanban',
    canActivate: [authGuard],
    loadComponent: () => import('./features/kanban/kanban.component').then(m => m.KanbanComponent)
  },
  {
    path: 'applications',
    canActivate: [authGuard],
    children: [
      {
        path: '',
        loadComponent: () => import('./features/applications/list/application-list.component').then(m => m.ApplicationListComponent)
      },
      {
        path: 'new',
        loadComponent: () => import('./features/applications/form/application-form.component').then(m => m.ApplicationFormComponent)
      },
      {
        path: ':id',
        loadComponent: () => import('./features/applications/detail/application-detail.component').then(m => m.ApplicationDetailComponent)
      },
      {
        path: ':id/edit',
        loadComponent: () => import('./features/applications/form/application-form.component').then(m => m.ApplicationFormComponent)
      }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
