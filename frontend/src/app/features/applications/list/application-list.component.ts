import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { JobApplicationService } from '../../../core/services/job-application.service';
import { ApplicationStatus, JobApplication, PaginatedResult } from '../../../shared/models/job-application.model';

@Component({
  selector: 'app-application-list',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  template: `
    <div class="p-6">
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-2xl font-bold text-gray-900">Applications</h1>
        <a routerLink="/applications/new"
          class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700">
          + New
        </a>
      </div>

      <div class="bg-white rounded-xl shadow p-4 mb-4 flex flex-wrap gap-3">
        <input [formControl]="searchControl" placeholder="Search company or title..."
          class="border-gray-300 rounded-md text-sm flex-1 min-w-48" />
        <select [formControl]="statusControl" class="border-gray-300 rounded-md text-sm">
          <option value="">All statuses</option>
          <option value="Applied">Applied</option>
          <option value="Interview">Interview</option>
          <option value="Offer">Offer</option>
          <option value="Rejected">Rejected</option>
        </select>
      </div>

      @if (loading) {
        <p class="text-gray-500 text-center py-8">Loading...</p>
      } @else {
        <div class="bg-white rounded-xl shadow overflow-hidden">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Company</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Role</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Status</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Applied</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-gray-200">
              @for (app of result?.items; track app.id) {
                <tr class="hover:bg-gray-50 cursor-pointer" [routerLink]="['/applications', app.id]">
                  <td class="px-6 py-4 text-sm font-medium text-gray-900">{{ app.companyName }}</td>
                  <td class="px-6 py-4 text-sm text-gray-500">{{ app.jobTitle }}</td>
                  <td class="px-6 py-4">
                    <span class="px-2 py-1 text-xs rounded-full" [class]="statusClass(app.status)">
                      {{ app.status }}
                    </span>
                  </td>
                  <td class="px-6 py-4 text-sm text-gray-500">{{ app.appliedDate | date:'mediumDate' }}</td>
                </tr>
              }
            </tbody>
          </table>
        </div>
        <div class="flex justify-between items-center mt-4 text-sm text-gray-600">
          <span>{{ result?.totalCount ?? 0 }} total</span>
          <div class="flex gap-2">
            <button (click)="prevPage()" [disabled]="!result?.hasPreviousPage"
              class="px-3 py-1 border rounded disabled:opacity-40">Previous</button>
            <span>Page {{ page }} of {{ result?.totalPages ?? 1 }}</span>
            <button (click)="nextPage()" [disabled]="!result?.hasNextPage"
              class="px-3 py-1 border rounded disabled:opacity-40">Next</button>
          </div>
        </div>
      }
    </div>
  `
})
export class ApplicationListComponent implements OnInit {
  private fb = inject(FormBuilder);
  private appService = inject(JobApplicationService);

  result: PaginatedResult<JobApplication> | null = null;
  loading = false;
  page = 1;

  searchControl = this.fb.control('');
  statusControl = this.fb.control('');

  ngOnInit() {
    this.load();
    this.searchControl.valueChanges.pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(() => { this.page = 1; this.load(); });
    this.statusControl.valueChanges
      .subscribe(() => { this.page = 1; this.load(); });
  }

  load() {
    this.loading = true;
    this.appService.getAll({
      page: this.page,
      pageSize: 20,
      search: this.searchControl.value || undefined,
      status: (this.statusControl.value as ApplicationStatus) || undefined
    }).subscribe({
      next: r => { this.result = r; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  nextPage() { this.page++; this.load(); }
  prevPage() { this.page--; this.load(); }

  statusClass(status: ApplicationStatus): string {
    const map: Record<ApplicationStatus, string> = {
      Applied: 'bg-blue-100 text-blue-800',
      Interview: 'bg-yellow-100 text-yellow-800',
      Offer: 'bg-green-100 text-green-800',
      Rejected: 'bg-red-100 text-red-800'
    };
    return map[status];
  }
}
