import axiosInstance from './axiosInstance';
import type {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  User,
  Ticket,
  CreateTicketRequest,
  UpdateTicketRequest,
  Category,
  Comment,
  CreateCommentRequest,
  DashboardStats,
  TicketFilter,
} from '../models';

// Auth API
export const authApi = {
  login: (data: LoginRequest) =>
    axiosInstance.post<AuthResponse>('/auth/login', data),
  register: (data: RegisterRequest) =>
    axiosInstance.post<AuthResponse>('/auth/register', data),
  getProfile: () => axiosInstance.get<User>('/auth/profile'),
  getUsers: () => axiosInstance.get<User[]>('/auth/users'),
  updateUserRole: (id: number, role: string) =>
    axiosInstance.put<User>(`/auth/users/${id}/role`, { role }),
};

// Tickets API
export const ticketsApi = {
  getAll: (filter?: TicketFilter) =>
    axiosInstance.get<Ticket[]>('/tickets', { params: filter }),
  getById: (id: number) => axiosInstance.get<Ticket>(`/tickets/${id}`),
  create: (data: CreateTicketRequest) =>
    axiosInstance.post<Ticket>('/tickets', data),
  update: (id: number, data: UpdateTicketRequest) =>
    axiosInstance.put<Ticket>(`/tickets/${id}`, data),
  delete: (id: number) => axiosInstance.delete(`/tickets/${id}`),
  getDashboard: () => axiosInstance.get<DashboardStats>('/tickets/dashboard'),
};

// Categories API
export const categoriesApi = {
  getAll: () => axiosInstance.get<Category[]>('/categories'),
  create: (categoryName: string) =>
    axiosInstance.post<Category>('/categories', { categoryName }),
};

// Comments API
export const commentsApi = {
  getByTicket: (ticketId: number) =>
    axiosInstance.get<Comment[]>(`/comments/${ticketId}`),
  create: (data: CreateCommentRequest) =>
    axiosInstance.post<Comment>('/comments', data),
};
