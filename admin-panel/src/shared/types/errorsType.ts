export type ErrorParameters = {
  page: number;
  pageSize: number;
  search?: string;
  ProjectId?: string;
  IsResolved?: boolean;
  StartDate?: Date;
  EndDate?: Date;
  sortField?: string;
  sortDirection?: "asc" | "desc";
};

export interface ErrorGroup {
  id: string;

  projectId: string;

  projectName: string;

  message: string;

  stackTrace: string;

  url: string;

  exceptionType: string;

  method: string;

  browser: string | null;

  userId: string | null;

  ipAddress: string | null;

  createdAt: string;
}

export interface ErrorScreenshot {
  id: string;
  bucketName: string;
  objectKey: string;
  region: string;
  contentType: string;
  sizeInBytes: number;
  checksum: string | null;
  s3Url: string;
  createdAt: string;
}

export interface ErrorDetail {
  id: string;
  projectId: string;
  projectName: string;
  message: string;
  stackTrace: string;
  url: string;
  exceptionType: string;
  method: string;
  requestBody: string;
  queryString: string | null;
  browser: string | null;
  ipAddress: string | null;
  createdAt: string;
  screenshot: ErrorScreenshot | null;
}
