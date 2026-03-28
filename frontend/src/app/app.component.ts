import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    @if (auth.isLoggedIn()) {
      <nav class="bg-white border-b border-gray-200 px-6 py-3 flex items-center justify-between">
        <div class="flex items-center gap-6">
          <span class="font-bold text-gray-900">Job Tracker</span>
          <a routerLink="/dashboard" routerLinkActive="text-blue-600" class="text-sm text-gray-600 hover:text-gray-900">Dashboard</a>
          <a routerLink="/kanban" routerLinkActive="text-blue-600" class="text-sm text-gray-600 hover:text-gray-900">Kanban</a>
          <a routerLink="/applications" routerLinkActive="text-blue-600" class="text-sm text-gray-600 hover:text-gray-900">Applications</a>
        </div>
        <div class="flex items-center gap-4">
          <span class="text-sm text-gray-500">{{ auth.currentUser()?.firstName }}</span>
          <button (click)="auth.logout()" class="text-sm text-red-600 hover:underline">Sign out</button>
        </div>
      </nav>
    }
    <main class="min-h-screen bg-gray-50">
      <router-outlet />
    </main>
  `
})
export class AppComponent {
  constructor(public auth: AuthService) {}
}
