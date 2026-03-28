import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import {
  CreateInterviewRequest,
  CreateJobApplicationRequest,
  GetJobApplicationsQuery,
  Interview,
  JobApplication,
  PaginatedResult,
  UpdateJobApplicationRequest,
  UpdateStatusRequest
} from '../../shared/models/job-application.model';

@Injectable({ providedIn: 'root' })
export class JobApplicationService {
  private readonly base = `${environment.apiUrl}/applications`;

  constructor(private http: HttpClient) {}

  getAll(query: GetJobApplicationsQuery = {}) {
    let params = new HttpParams();
    if (query.page) params = params.set('page', query.page);
    if (query.pageSize) params = params.set('pageSize', query.pageSize);
    if (query.status) params = params.set('status', query.status);
    if (query.search) params = params.set('search', query.search);
    if (query.fromDate) params = params.set('fromDate', query.fromDate);
    if (query.toDate) params = params.set('toDate', query.toDate);
    return this.http.get<PaginatedResult<JobApplication>>(this.base, { params });
  }

  getById(id: string) {
    return this.http.get<JobApplication>(`${this.base}/${id}`);
  }

  create(request: CreateJobApplicationRequest) {
    return this.http.post<JobApplication>(this.base, request);
  }

  update(id: string, request: UpdateJobApplicationRequest) {
    return this.http.put<JobApplication>(`${this.base}/${id}`, request);
  }

  updateStatus(id: string, request: UpdateStatusRequest) {
    return this.http.patch<void>(`${this.base}/${id}/status`, request);
  }

  delete(id: string) {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  uploadCv(id: string, file: File) {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<void>(`${this.base}/${id}/cv`, form);
  }

  downloadCv(id: string) {
    return this.http.get(`${this.base}/${id}/cv`, { responseType: 'blob' });
  }

  createInterview(applicationId: string, request: CreateInterviewRequest) {
    return this.http.post<Interview>(`${this.base}/${applicationId}/interviews`, request);
  }
}
