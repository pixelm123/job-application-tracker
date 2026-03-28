import { Component, Input, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { JobApplicationService } from '../../../core/services/job-application.service';

@Component({
  selector: 'app-application-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="min-h-screen bg-gray-50 p-6">
      <div class="max-w-2xl mx-auto">

        <div class="flex items-center gap-3 mb-6">
          <a routerLink="/applications"
            class="w-9 h-9 flex items-center justify-center rounded-xl border border-gray-200 bg-white hover:bg-gray-50 text-gray-600 shadow-sm transition-colors">
            &larr;
          </a>
          <div>
            <h1 class="text-2xl font-bold text-gray-900">{{ id ? 'Edit Application' : 'New Application' }}</h1>
            <p class="text-sm text-gray-500 mt-0.5">{{ id ? 'Update the details below' : 'Fill in the details to track a new role' }}</p>
          </div>
        </div>

        <form [formGroup]="form" (ngSubmit)="onSubmit()" class="space-y-5">

          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <h2 class="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-4">Role Details</h2>
            <div class="space-y-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">
                  Company Name <span class="text-red-500">*</span>
                </label>
                <input type="text" formControlName="companyName" placeholder="e.g. Google"
                  class="w-full px-4 py-2.5 rounded-xl border border-gray-200 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                  [class.border-red-300]="form.get('companyName')?.invalid && form.get('companyName')?.touched" />
                @if (form.get('companyName')?.invalid && form.get('companyName')?.touched) {
                  <p class="text-xs text-red-500 mt-1">Company name is required</p>
                }
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">
                  Job Title <span class="text-red-500">*</span>
                </label>
                <input type="text" formControlName="jobTitle" placeholder="e.g. Software Engineer"
                  class="w-full px-4 py-2.5 rounded-xl border border-gray-200 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                  [class.border-red-300]="form.get('jobTitle')?.invalid && form.get('jobTitle')?.touched" />
                @if (form.get('jobTitle')?.invalid && form.get('jobTitle')?.touched) {
                  <p class="text-xs text-red-500 mt-1">Job title is required</p>
                }
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">Job Posting URL</label>
                <input type="url" formControlName="jobUrl" placeholder="https://..."
                  class="w-full px-4 py-2.5 rounded-xl border border-gray-200 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all" />
              </div>
            </div>
          </div>

          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <h2 class="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-4">Dates</h2>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">
                  Applied Date <span class="text-red-500">*</span>
                </label>
                <input type="date" formControlName="appliedDate"
                  class="w-full px-4 py-2.5 rounded-xl border border-gray-200 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all" />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">Reminder Date</label>
                <input type="date" formControlName="reminderDate"
                  class="w-full px-4 py-2.5 rounded-xl border border-gray-200 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all" />
                <p class="text-xs text-gray-400 mt-1">Receive a reminder email on this date</p>
              </div>
            </div>
          </div>

          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <h2 class="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-4">Notes</h2>
            <textarea formControlName="notes" rows="4"
              placeholder="Recruiter contact, salary range, interview notes..."
              class="w-full px-4 py-2.5 rounded-xl border border-gray-200 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all resize-none"></textarea>
          </div>

          @if (error) {
            <div class="flex items-center gap-3 p-4 bg-red-50 border border-red-200 rounded-xl">
              <p class="text-sm text-red-700">{{ error }}</p>
            </div>
          }

          <div class="flex gap-3 pb-6">
            <button type="submit" [disabled]="form.invalid || loading"
              class="flex-1 py-3 bg-blue-600 text-white rounded-xl font-semibold hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors shadow-sm text-sm">
              @if (loading) {
                <span class="flex items-center justify-center gap-2">
                  <span class="inline-block w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></span>
                  Saving...
                </span>
              } @else {
                {{ id ? 'Save Changes' : 'Add Application' }}
              }
            </button>
            <a routerLink="/applications"
              class="px-6 py-3 border border-gray-200 rounded-xl text-sm font-medium text-gray-600 hover:bg-gray-50 transition-colors text-center">
              Cancel
            </a>
          </div>

        </form>
      </div>
    </div>
  `
})
export class ApplicationFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private appService = inject(JobApplicationService);
  private router = inject(Router);

  @Input() id?: string;

  form = this.fb.group({
    companyName: ['', Validators.required],
    jobTitle: ['', Validators.required],
    jobUrl: [''],
    appliedDate: [new Date().toISOString().split('T')[0], Validators.required],
    notes: [''],
    reminderDate: ['']
  });

  loading = false;
  error = '';

  ngOnInit() {
    if (this.id) {
      this.appService.getById(this.id).subscribe(app => {
        this.form.patchValue({
          companyName: app.companyName,
          jobTitle: app.jobTitle,
          jobUrl: app.jobUrl ?? '',
          appliedDate: app.appliedDate.split('T')[0],
          notes: app.notes ?? '',
          reminderDate: app.reminderDate ? app.reminderDate.split('T')[0] : ''
        });
      });
    }
  }

  onSubmit() {
    if (this.form.invalid) return;
    this.loading = true;
    this.error = '';

    const value = this.form.value;
    const request = {
      companyName: value.companyName!,
      jobTitle: value.jobTitle!,
      jobUrl: value.jobUrl || undefined,
      appliedDate: value.appliedDate!,
      notes: value.notes || undefined,
      reminderDate: value.reminderDate || undefined
    };

    const op = this.id
      ? this.appService.update(this.id, request)
      : this.appService.create(request);

    op.subscribe({
      next: (app: any) => this.router.navigate(['/applications', app?.id ?? this.id]),
      error: () => { this.error = 'Save failed. Please try again.'; this.loading = false; }
    });
  }
}
