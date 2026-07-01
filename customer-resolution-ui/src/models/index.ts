export interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  createdDate: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export interface Ticket {
  id: number;
  title: string;
  description: string;
  categoryId: number;
  categoryName: string;
  priority: string;
  status: string;
  createdBy: number;
  creatorName: string;
  assignedTo: number | null;
  assigneeName: string | null;
  createdDate: string;
  updatedDate: string;
}

export interface CreateTicketRequest {
  title: string;
  description: string;
  categoryId: number;
  priority: string;
}

export interface UpdateTicketRequest {
  title?: string;
  description?: string;
  categoryId?: number;
  priority?: string;
  status?: string;
  assignedTo?: number;
}

export interface Category {
  id: number;
  categoryName: string;
}

export interface Comment {
  id: number;
  ticketId: number;
  content: string;
  createdBy: number;
  authorName: string;
  createdDate: string;
}

export interface CreateCommentRequest {
  ticketId: number;
  content: string;
}

export interface DashboardStats {
  totalTickets: number;
  openTickets: number;
  resolvedTickets: number;
  pendingTickets: number;
  todaysTickets: number;
}

export interface TicketFilter {
  title?: string;
  priority?: string;
  status?: string;
  categoryId?: number;
  createdFrom?: string;
  createdTo?: string;
}
