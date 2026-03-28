import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { JobApplicationService } from '../../core/services/job-application.service';
import { JobApplication } from '../../shared/models/job-application.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="p-6 max-w-7xl mx-auto">

      <div class="flex justify-between items-start mb-8">
        <div>
          <h1 class="text-3xl font-bold text-gray-900">Dashboard</h1>
          <p class="text-gray-500 mt-1">Track your job search progress</p>
        </div>
        <a routerLink="/applications/new"
          class="px-5 py-2.5 bg-blue-600 text-white rounded-xl hover:bg-blue-700 font-medium shadow-sm transition-colors text-sm">
          + New Application
        </a>
      </div>

      <div class="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
        <div class="bg-gradient-to-br from-blue-500 to-blue-600 rounded-2xl p-6 text-white shadow-md">
          <p class="text-xs font-semibold uppercase tracking-wide text-blue-200 mb-3">Applied</p>
          <p class="text-5xl font-black">{{ stats[0].count }}</p>
          <p class="text-blue-100 text-sm mt-2">Applications sent</p>
        </div>

        <div class="bg-gradient-to-br from-amber-400 to-orange-500 rounded-2xl p-6 text-white shadow-md">
          <p class="text-xs font-semibold uppercase tracking-wide text-amber-100 mb-3">Interview</p>
          <p class="text-5xl font-black">{{ stats[1].count }}</p>
          <p class="text-amber-100 text-sm mt-2">Interviews booked</p>
        </div>

        <div class="bg-gradient-to-br from-emerald-500 to-green-600 rounded-2xl p-6 text-white shadow-md">
          <p class="text-xs font-semibold uppercase tracking-wide text-emerald-100 mb-3">Offer</p>
          <p class="text-5xl font-black">{{ stats[2].count }}</p>
          <p class="text-emerald-100 text-sm mt-2">Offers received</p>
        </div>

        <div class="bg-gradient-to-br from-rose-500 to-red-600 rounded-2xl p-6 text-white shadow-md">
          <p class="text-xs font-semibold uppercase tracking-wide text-rose-100 mb-3">Rejected</p>
          <p class="text-5xl font-black">{{ stats[3].count }}</p>
          <p class="text-rose-100 text-sm mt-2">Not selected</p>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">

        <div class="lg:col-span-2 bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div class="flex items-center justify-between px-6 py-4 border-b border-gray-100">
            <h2 class="font-semibold text-gray-800">Recent Applications</h2>
            <a routerLink="/applications" class="text-sm text-blue-600 hover:text-blue-800 font-medium">View all</a>
          </div>

          @if (recentApps.length === 0) {
            <div class="flex flex-col items-center justify-center py-16 text-center">
              <p class="text-gray-500 font-medium">No applications yet</p>
              <p class="text-gray-400 text-sm mt-1">Add your first to get started</p>
              <a routerLink="/applications/new"
                class="mt-4 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm hover:bg-blue-700">
                + New Application
              </a>
            </div>
          } @else {
            <div class="divide-y divide-gray-50">
              @for (app of recentApps; track app.id) {
                <a [routerLink]="['/applications', app.id]"
                  class="flex items-center gap-4 px-6 py-4 hover:bg-gray-50 transition-colors">
                  <div class="w-10 h-10 rounded-full flex items-center justify-center text-sm font-bold flex-shrink-0"
                    [class]="avatarClass(app.status)">
                    {{ app.companyName[0].toUpperCase() }}
                  </div>
                  <div class="flex-1 min-w-0">
                    <p class="font-semibold text-gray-900 text-sm truncate">{{ app.companyName }}</p>
                    <p class="text-xs text-gray-500 truncate">{{ app.jobTitle }}</p>
                  </div>
                  <div class="flex flex-col items-end gap-1 flex-shrink-0">
                    <span class="text-xs px-2.5 py-0.5 rounded-full font-semibold" [class]="badgeClass(app.status)">
                      {{ app.status }}
                    </span>
                    <span class="text-xs text-gray-400">{{ app.appliedDate | date:'MMM d, y' }}</span>
                  </div>
                </a>
              }
            </div>
          }
        </div>

        <div class="space-y-5">

          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <h2 class="font-semibold text-gray-800 mb-5">Success Rate</h2>
            @if (total === 0) {
              <p class="text-sm text-gray-400 text-center py-4">Apply to see your stats</p>
            } @else {
              <div class="space-y-4">
                <div>
                  <div class="flex justify-between text-xs mb-1.5">
                    <span class="text-gray-600 font-medium">Interview rate</span>
                    <span class="font-bold text-amber-600">{{ interviewRate }}%</span>
                  </div>
                  <div class="h-2.5 bg-gray-100 rounded-full overflow-hidden">
                    <div class="h-full bg-gradient-to-r from-amber-400 to-orange-400 rounded-full transition-all duration-700"
                      [style.width.%]="interviewRate"></div>
                  </div>
                </div>
                <div>
                  <div class="flex justify-between text-xs mb-1.5">
                    <span class="text-gray-600 font-medium">Offer rate</span>
                    <span class="font-bold text-emerald-600">{{ offerRate }}%</span>
                  </div>
                  <div class="h-2.5 bg-gray-100 rounded-full overflow-hidden">
                    <div class="h-full bg-gradient-to-r from-emerald-400 to-green-500 rounded-full transition-all duration-700"
                      [style.width.%]="offerRate"></div>
                  </div>
                </div>
                <p class="text-xs text-gray-400 pt-1 border-t border-gray-50">
                  Based on {{ total }} total application{{ total !== 1 ? 's' : '' }}
                </p>
              </div>
            }
          </div>

          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <h2 class="font-semibold text-gray-800 mb-4">Quick Actions</h2>
            <div class="space-y-1">
              <a routerLink="/applications/new"
                class="flex items-center gap-3 p-3 rounded-xl hover:bg-blue-50 transition-colors group">
                <span class="w-8 h-8 bg-blue-100 rounded-lg flex items-center justify-center text-sm font-bold text-blue-600">+</span>
                <span class="text-sm text-gray-700 group-hover:text-blue-700 font-medium">New Application</span>
              </a>
              <a routerLink="/kanban"
                class="flex items-center gap-3 p-3 rounded-xl hover:bg-purple-50 transition-colors group">
                <span class="w-8 h-8 bg-purple-100 rounded-lg flex items-center justify-center text-sm font-bold text-purple-600">K</span>
                <span class="text-sm text-gray-700 group-hover:text-purple-700 font-medium">Kanban Board</span>
              </a>
              <a routerLink="/applications"
                class="flex items-center gap-3 p-3 rounded-xl hover:bg-gray-50 transition-colors group">
                <span class="w-8 h-8 bg-gray-100 rounded-lg flex items-center justify-center text-sm font-bold text-gray-600">A</span>
                <span class="text-sm text-gray-700 group-hover:text-gray-900 font-medium">All Applications</span>
              </a>
            </div>
          </div>

        </div>
      </div>
    </div>
  `
})
export class DashboardComponent implements OnInit {
  stats = [
    { label: 'Applied', count: 0 },
    { label: 'Interview', count: 0 },
    { label: 'Offer', count: 0 },
    { label: 'Rejected', count: 0 },
  ];
  recentApps: JobApplication[] = [];
  total = 0;

  get interviewRate() {
    return this.total ? Math.round((this.stats[1].count / this.total) * 100) : 0;
  }

  get offerRate() {
    return this.total ? Math.round((this.stats[2].count / this.total) * 100) : 0;
  }

  constructor(private appService: JobApplicationService) {}

  ngOnInit() {
    this.appService.getAll({ pageSize: 100 }).subscribe(result => {
      const apps = result.items;
      this.total = apps.length;
      this.stats[0].count = apps.filter(a => a.status === 'Applied').length;
      this.stats[1].count = apps.filter(a => a.status === 'Interview').length;
      this.stats[2].count = apps.filter(a => a.status === 'Offer').length;
      this.stats[3].count = apps.filter(a => a.status === 'Rejected').length;
      this.recentApps = apps.slice(0, 5);
    });
  }

  avatarClass(status: string): string {
    const map: Record<string, string> = {
      Applied: 'bg-blue-100 text-blue-700',
      Interview: 'bg-amber-100 text-amber-700',
      Offer: 'bg-emerald-100 text-emerald-700',
      Rejected: 'bg-rose-100 text-rose-700',
    };
    return map[status] ?? 'bg-gray-100 text-gray-700';
  }

  badgeClass(status: string): string {
    const map: Record<string, string> = {
      Applied: 'bg-blue-100 text-blue-700',
      Interview: 'bg-amber-100 text-amber-700',
      Offer: 'bg-emerald-100 text-emerald-700',
      Rejected: 'bg-rose-100 text-rose-700',
    };
    return map[status] ?? 'bg-gray-100 text-gray-700';
  }
}
