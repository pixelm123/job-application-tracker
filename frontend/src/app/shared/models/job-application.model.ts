export type ApplicationStatus = 'Applied' | 'Interview' | 'Offer' | 'Rejected';
export type InterviewType = 'Phone' | 'Video' | 'OnSite' | 'Technical';

export interface JobApplication {
  id: string;
  userId: string;
  companyName: string;
  jobTitle: string;
  jobUrl?: string;
  status: ApplicationStatus;
  appliedDate: string;
  notes?: string;
  cvFileName?: string;
  cvFilePath?: string;
  reminderDate?: string;
  reminderSent: boolean;
  createdAt: string;
  updatedAt: string;
  interviews: Interview[];
}

export interface Interview {
  id: string;
  jobApplicationId: string;
  scheduledAt: string;
  type: InterviewType;
  notes?: string;
  createdAt: string;
}

export interface CreateJobApplicationRequest {
  companyName: string;
  jobTitle: string;
  jobUrl?: string;
  appliedDate: string;
  notes?: string;
  reminderDate?: string;
}

export interface UpdateJobApplicationRequest extends CreateJobApplicationRequest {}

export interface UpdateStatusRequest {
  status: ApplicationStatus;
}

export interface CreateInterviewRequest {
  scheduledAt: string;
  type: InterviewType;
  notes?: string;
}

export interface PaginatedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface GetJobApplicationsQuery {
  page?: number;
  pageSize?: number;
  status?: ApplicationStatus;
  search?: string;
  fromDate?: string;
  toDate?: string;
}
