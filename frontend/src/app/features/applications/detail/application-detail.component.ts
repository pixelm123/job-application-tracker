import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { JobApplicationService } from '../../../core/services/job-application.service';
import { ApplicationStatus, JobApplication } from '../../../shared/models/job-application.model';

@Component({
  selector: 'app-application-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="p-6 max-w-3xl mx-auto">
      @if (app) {
        <div class="flex justify-between items-start mb-6">
          <div>
            <h1 class="text-2xl font-bold text-gray-900">{{ app.companyName }}</h1>
            <p class="text-gray-500 mt-1">{{ app.jobTitle }}</p>
          </div>
          <div class="flex gap-2">
            <a [routerLink]="['/applications', app.id, 'edit']"
              class="px-3 py-1.5 border border-gray-300 rounded text-sm hover:bg-gray-50">Edit</a>
            <button (click)="delete()" class="px-3 py-1.5 bg-red-600 text-white rounded text-sm hover:bg-red-700">
              Delete
            </button>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow divide-y">
          <div class="p-5 grid grid-cols-2 gap-4">
            <div>
              <p class="text-xs text-gray-500">Status</p>
              <span class="px-2 py-1 text-xs rounded-full mt-1 inline-block" [class]="statusClass(app.status)">
                {{ app.status }}
              </span>
            </div>
            <div>
              <p class="text-xs text-gray-500">Applied</p>
              <p class="text-sm mt-1">{{ app.appliedDate | date:'mediumDate' }}</p>
            </div>
            @if (app.jobUrl) {
              <div class="col-span-2">
                <p class="text-xs text-gray-500">Job URL</p>
                <a [href]="app.jobUrl" target="_blank" class="text-blue-600 text-sm hover:underline">{{ app.jobUrl }}</a>
              </div>
            }
            @if (app.notes) {
              <div class="col-span-2">
                <p class="text-xs text-gray-500">Notes</p>
                <p class="text-sm mt-1 whitespace-pre-wrap">{{ app.notes }}</p>
              </div>
            }
            @if (app.cvFileName) {
              <div class="col-span-2">
                <p class="text-xs text-gray-500">CV</p>
                <div class="flex items-center gap-3 mt-1">
                  <p class="text-sm">{{ app.cvFileName }}</p>
                  <button (click)="downloadCv()"
                    class="flex items-center gap-1.5 px-3 py-1 bg-blue-50 text-blue-700 text-xs font-medium rounded-lg hover:bg-blue-100 transition-colors">
                    Download
                  </button>
                </div>
              </div>
            }
          </div>

          @if (app.interviews.length > 0) {
            <div class="p-5">
              <h3 class="font-semibold text-gray-700 mb-3">Interviews</h3>
              <div class="space-y-2">
                @for (interview of app.interviews; track interview.id) {
                  <div class="flex gap-4 text-sm">
                    <span class="text-gray-500">{{ interview.scheduledAt | date:'medium' }}</span>
                    <span class="font-medium">{{ interview.type }}</span>
                    @if (interview.notes) { <span class="text-gray-400">{{ interview.notes }}</span> }
                  </div>
                }
              </div>
            </div>
          }
        </div>

        <div class="mt-4">
          <label class="block text-sm font-medium text-gray-700 mb-1">Upload CV (PDF, max 5MB)</label>
          <input type="file" accept=".pdf" (change)="onCvUpload($event)"
            class="text-sm text-gray-500 file:mr-3 file:py-1.5 file:px-4 file:rounded file:border-0 file:bg-blue-50 file:text-blue-700 hover:file:bg-blue-100" />
        </div>
      } @else {
        <p class="text-gray-500 text-center py-8">Loading...</p>
      }
    </div>
  `
})
export class ApplicationDetailComponent implements OnInit {
  @Input() id!: string;
  app: JobApplication | null = null;

  constructor(private appService: JobApplicationService, private router: Router) {}

  ngOnInit() {
    this.appService.getById(this.id).subscribe(app => this.app = app);
  }

  delete() {
    if (!confirm('Delete this application?')) return;
    this.appService.delete(this.id).subscribe(() => this.router.navigate(['/applications']));
  }

  downloadCv() {
    if (!this.app) return;
    this.appService.downloadCv(this.id).subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = this.app!.cvFileName ?? 'cv.pdf';
      a.click();
      URL.revokeObjectURL(url);
    });
  }

  onCvUpload(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;
    if (file.size > 5 * 1024 * 1024) { alert('File must be under 5MB'); return; }
    this.appService.uploadCv(this.id, file).subscribe(() => this.ngOnInit());
  }

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
