import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DragDropModule, CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { JobApplicationService } from '../../core/services/job-application.service';
import { ApplicationStatus, JobApplication } from '../../shared/models/job-application.model';

interface KanbanColumn {
  status: ApplicationStatus;
  label: string;
  headerClass: string;
  accentClass: string;
  emptyText: string;
  items: JobApplication[];
}

@Component({
  selector: 'app-kanban',
  standalone: true,
  imports: [CommonModule, RouterLink, DragDropModule],
  template: `
    <div class="p-6">
      <div class="flex items-center justify-between mb-8">
        <div>
          <h1 class="text-3xl font-bold text-gray-900">Kanban Board</h1>
          <p class="text-gray-500 text-sm mt-1">Drag cards between columns to update status</p>
        </div>
        <a routerLink="/applications/new"
          class="px-4 py-2.5 bg-blue-600 text-white rounded-xl hover:bg-blue-700 text-sm font-medium shadow-sm transition-colors">
          + New Application
        </a>
      </div>

      <div class="flex gap-5 overflow-x-auto pb-4 items-start">
        @for (col of columns; track col.status) {
          <div class="flex-shrink-0 w-72">

            <div class="rounded-t-2xl px-4 py-3 flex items-center justify-between" [class]="col.headerClass">
              <h2 class="font-bold text-sm tracking-wide uppercase">{{ col.label }}</h2>
              <span class="text-xs font-bold bg-white bg-opacity-25 px-2.5 py-0.5 rounded-full">
                {{ col.items.length }}
              </span>
            </div>

            <div
              [id]="col.status"
              cdkDropList
              [cdkDropListData]="col.items"
              [cdkDropListConnectedTo]="connectedLists"
              (cdkDropListDropped)="onDrop($event)"
              class="min-h-40 space-y-3 rounded-b-2xl p-2 bg-gray-100">

              @if (col.items.length === 0) {
                <div class="py-10 flex flex-col items-center justify-center text-center text-gray-400">
                  <p class="text-xs">{{ col.emptyText }}</p>
                  <p class="text-xs mt-0.5 opacity-70">Drop a card here</p>
                </div>
              }

              @for (app of col.items; track app.id) {
                <div cdkDrag
                  class="bg-white rounded-xl shadow-sm border-l-4 hover:shadow-md transition-all cursor-grab active:cursor-grabbing"
                  [class]="col.accentClass">
                  <a [routerLink]="['/applications', app.id]" class="block p-4">
                    <div class="flex items-start justify-between gap-2 mb-2">
                      <div class="w-8 h-8 rounded-lg flex items-center justify-center text-xs font-black flex-shrink-0"
                        [class]="avatarClass(app.status)">
                        {{ app.companyName[0].toUpperCase() }}
                      </div>
                      @if (app.cvFileName) {
                        <span class="text-gray-400 text-xs">CV</span>
                      }
                    </div>
                    <p class="font-bold text-gray-900 text-sm leading-snug">{{ app.companyName }}</p>
                    <p class="text-gray-500 text-xs mt-1">{{ app.jobTitle }}</p>

                    @if (app.reminderDate) {
                      <p class="mt-2 text-xs text-orange-500 font-medium">
                        Reminder: {{ app.reminderDate | date:'MMM d' }}
                      </p>
                    }

                    <div class="mt-3 pt-2.5 border-t border-gray-50 flex items-center justify-between">
                      <span class="text-xs text-gray-400">{{ app.appliedDate | date:'MMM d, y' }}</span>
                      @if (app.interviews && app.interviews.length > 0) {
                        <span class="text-xs text-blue-500 font-medium">
                          {{ app.interviews.length }} interview{{ app.interviews.length !== 1 ? 's' : '' }}
                        </span>
                      }
                    </div>
                  </a>
                </div>
              }
            </div>

          </div>
        }
      </div>
    </div>
  `
})
export class KanbanComponent implements OnInit {
  columns: KanbanColumn[] = [
    {
      status: 'Applied',
      label: 'Applied',
      headerClass: 'bg-gradient-to-r from-blue-500 to-blue-600 text-white',
      accentClass: 'border-blue-400',
      emptyText: 'No applications',
      items: []
    },
    {
      status: 'Interview',
      label: 'Interview',
      headerClass: 'bg-gradient-to-r from-amber-400 to-orange-500 text-white',
      accentClass: 'border-amber-400',
      emptyText: 'No interviews yet',
      items: []
    },
    {
      status: 'Offer',
      label: 'Offer',
      headerClass: 'bg-gradient-to-r from-emerald-500 to-green-600 text-white',
      accentClass: 'border-emerald-400',
      emptyText: 'No offers yet',
      items: []
    },
    {
      status: 'Rejected',
      label: 'Rejected',
      headerClass: 'bg-gradient-to-r from-rose-400 to-red-500 text-white',
      accentClass: 'border-rose-300',
      emptyText: 'None rejected',
      items: []
    },
  ];

  get connectedLists() {
    return this.columns.map(c => c.status);
  }

  constructor(private appService: JobApplicationService) {}

  ngOnInit() {
    this.appService.getAll({ pageSize: 200 }).subscribe(result => {
      result.items.forEach(app => {
        const col = this.columns.find(c => c.status === app.status);
        col?.items.push(app);
      });
    });
  }

  onDrop(event: CdkDragDrop<JobApplication[]>) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      const app = event.previousContainer.data[event.previousIndex];
      const newStatus = event.container.id as ApplicationStatus;

      transferArrayItem(event.previousContainer.data, event.container.data, event.previousIndex, event.currentIndex);
      const originalStatus = app.status;
      (app as any).status = newStatus;

      this.appService.updateStatus(app.id, { status: newStatus }).subscribe({
        error: () => {
          transferArrayItem(event.container.data, event.previousContainer.data, event.currentIndex, event.previousIndex);
          (app as any).status = originalStatus;
        }
      });
    }
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
}
