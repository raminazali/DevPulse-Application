export interface KeyValue {
  label: string;
  value: number;
}

export interface DashboardSummary {
  totalErrors: number;
  todayErrors: number;
  weekErrors: number;
  monthErrors: number;
  activeProjects: number;
  lastErrorAt: string | null;
}

export interface ErrorComparison {
  today: number;
  yesterday: number;
  todayPercentage: number;
  week: number;
  lastWeek: number;
  weekPercentage: number;
  month: number;
  lastMonth: number;
  monthPercentage: number;
}

export interface RecentError {
  errorId: string;
  projectId: string;
  projectName: string;
  exceptionType: string;
  createdAt: string;
}

export interface TopUser {
  userId: string;
  fullName: string;
  projectCount: number;
  errorCount: number;
}

export interface AdminSummary {
  totalUsers: number;
  activeUsers: number;
  totalProjects: number;
  activeProjects: number;
  totalErrors: number;
  todayErrors: number;
}

export interface AdminTopProject {
  projectId: string;
  projectName: string;
  ownerName: string;
  errorCount: number;
}

export interface DashboardReport {
  summary: DashboardSummary;
  comparison: ErrorComparison;
  recentErrors: RecentError[];
  topExceptionTypes: KeyValue[];
  topUsers: TopUser[];
  hourlyErrors: KeyValue[];
  projectErrorDistribution: KeyValue[];
  userErrorDistribution: KeyValue[];
  adminSummary: AdminSummary | null;
  adminTopProjects: AdminTopProject[] | null;
}
