// eslint-disable-next-line @typescript-eslint/no-empty-interface
import {
  ICreateBookRequest,
  IEditBookRequest,
  IForgotPasswordRequest,
  ILoginRequest,
  IResetPasswordRequest,
  ITwoFactorRequest
} from "@/types/requests"
import {PaginatedResponse, SucceededOrNotResponse} from "@/types/responses"
import {Administrator, Book, ChatMessage, Conversation, EquipeConversation, EquipeMessage, Member, User,Equipe} from "@/types/entities"
import {Guid} from "@/types";
import type {AvailableSlot, AvailabilityData, AvailabilitySlot, AvailabilityOverride} from "@/types/entities";
import type { MyEquipeListItem, MyEquipeResponse } from "@/services/equipeService";

export interface IApiService {
  headersWithJsonContentType(): any;

  headersWithFormDataContentType(): any;

  buildEmptyBody(): string;
}

export interface IAdministratorService {
  getAuthenticated(): Promise<Administrator | undefined>;

  updateMyProfile(data: {
    firstName: string;
    lastName: string;
  }): Promise<SucceededOrNotResponse>;
}

export interface IAuthenticationService {
  login(request: ILoginRequest): Promise<SucceededOrNotResponse>;

  twoFactor(request: ITwoFactorRequest): Promise<SucceededOrNotResponse>;

  forgotPassword(
    request: IForgotPasswordRequest,
  ): Promise<SucceededOrNotResponse>;

  resetPassword(
    request: IResetPasswordRequest,
  ): Promise<SucceededOrNotResponse>;

  logout(): Promise<SucceededOrNotResponse>;
}

export interface LoginNotificationQuiz {
  assignmentId: string;
  quizId: string;
  titre: string;
  followUpLabel?: string | null;
  assignedAt: string;
}

export interface LoginNotificationModule {
  moduleId: string;
  name?: string | null;
  assignedAt: string;
}

export interface LoginNotifications {
  quizzes: LoginNotificationQuiz[];
  modules: LoginNotificationModule[];
}

export interface IMemberService {
  getAuthenticated(): Promise<Member | undefined>;

  getLoginNotifications(): Promise<LoginNotifications>;
  dismissLoginNotifications(): Promise<void>;


  search(
    pageIndex: number,
    pageSize: number,
    searchValue: string,
  ): Promise<PaginatedResponse<Member>>;

  getMember(id: string): Promise<Member>;

  createMember(member: Member): Promise<SucceededOrNotResponse>;

  updateMember(member: Member): Promise<SucceededOrNotResponse>;

  updateMyProfile(data: {
    firstName: string;
    lastName: string;
    phoneNumber?: string;
    phoneExtension?: number;
    apartment?: number;
    street?: string;
    city?: string;
    zipCode?: string;
  }): Promise<SucceededOrNotResponse>;

  deleteMember(id: Guid): Promise<SucceededOrNotResponse>;
}

export interface IBookService {
  getAllBooks(): Promise<Book[]>;

  getBook(bookId: string): Promise<Book>;

  deleteBook(bookId: string): Promise<SucceededOrNotResponse>;

  createBook(request: ICreateBookRequest): Promise<SucceededOrNotResponse>;

  editBook(request: IEditBookRequest): Promise<SucceededOrNotResponse>;
}

export interface IModulesService {
  getAllModules(): Promise<any[]>;
  getModule(id: string): Promise<any>;
  getModuleFlexible(id: string): Promise<any | null>;
  createModule(request: any): Promise<SucceededOrNotResponse>;
  updateModule(id: string, request: any): Promise<SucceededOrNotResponse>;
  deleteModule(id: string): Promise<SucceededOrNotResponse>;
  saveModuleFull(id: string, request: any): Promise<SucceededOrNotResponse>;
  uploadMedia(file: File): Promise<{ url: string }>;
  getModuleSections(moduleId: string): Promise<any[]>;
  assignModule(moduleId: string, memberId: string): Promise<SucceededOrNotResponse>;
  assignModuleToEquipe(moduleId: string, equipeId: string): Promise<SucceededOrNotResponse>;
  unassignModule(moduleId: string, memberId: string): Promise<SucceededOrNotResponse>;
  getModuleAssignments(moduleId: string): Promise<any[]>;
  getMyModules(): Promise<any[]>;
  getMyModuleDetail(moduleId: string): Promise<any>;
  markSectionRead(moduleId: string, sectionId: string): Promise<void>;
  getSectionProgress(moduleId: string): Promise<{ sectionId: string; isRead: boolean }[]>;
}

export interface IEquipesService {
  getAllEquipes(): Promise<Equipe[]>;
  getEquipe(id: string): Promise<Equipe>;
  getEquipeFlexible(id: string): Promise<Equipe | null>;
  createEquipe(request: any): Promise<SucceededOrNotResponse>;
  updateEquipe(id: string, request: any): Promise<SucceededOrNotResponse>;
  deleteEquipe(id: string): Promise<SucceededOrNotResponse>;
  getEquipeMembers(equipeId: string): Promise<any>;
  assignMembersToEquipe(equipeId: string, memberIds: string[]): Promise<SucceededOrNotResponse>;
  removeMemberFromEquipe(equipeId: string, memberId: string): Promise<SucceededOrNotResponse>;
  getMyEquipes(): Promise<MyEquipeListItem[]>;
  getMyEquipeDetails(equipeId: string): Promise<MyEquipeResponse | null>;
}

export interface IUserService {
  getCurrentUser(): Promise<User>;
}

export interface IConversationService {
  getConversations(): Promise<Conversation[]>
  getMessages(conversationId: string, page?: number, pageSize?: number): Promise<ChatMessage[]>
  createConversation(memberId: string): Promise<{ id: string }>
  sendMessage(conversationId: string, text: string, attachment?: File): Promise<ChatMessage>
  markAsRead(conversationId: string): Promise<void>
  getUnreadCount(): Promise<number>
}

export interface IEquipeConversationService {
  getConversations(): Promise<EquipeConversation[]>
  getMessages(conversationId: string, page?: number, pageSize?: number): Promise<EquipeMessage[]>
  sendMessage(conversationId: string, text: string, attachment?: File): Promise<EquipeMessage>
  markAsRead(conversationId: string): Promise<void>
  getUnreadCount(): Promise<number>
}

export interface IAppointmentService {
  getAvailableSlots(startDate: string, endDate: string): Promise<AvailableSlot[]>
  requestAppointment(date: string, motif?: string): Promise<any>
  respondAppointment(appointmentId: string, accept: boolean, reason?: string): Promise<any>
  getAvailability(): Promise<AvailabilityData>
  saveAvailability(slots: AvailabilitySlot[]): Promise<void>
  createOverride(data: { date: string, startTime?: string, endTime?: string, isBlocked: boolean }): Promise<AvailabilityOverride>
  deleteOverride(id: string): Promise<void>
}

export interface IQuizService {
  getAll(): Promise<any[]>
  getById(id: string): Promise<any>
  create(quiz: any): Promise<void>
  update(quiz: any): Promise<void>
  delete(id: string): Promise<void>
  getAssignedQuizzes(): Promise<any[]>
  submitResponse(response: any): Promise<any>
    assignQuiz(quizId: string, userIds: string[], followUpLabel?: string, availableAt?: Date, dueDate?: Date, equipeIds?: string[]): Promise<void>
    getAssignments(quizId: string): Promise<{ id: string; userId: string; version: number; followUpLabel?: string; availableAt?: string; dueDate?: string; completedAt?: string }[]>
  unassignQuiz(quizId: string, userIds: string[]): Promise<void>
    getUserResponses(quizAssignmentId: string): Promise<any>
    completeQuiz(quizAssignmentId: string): Promise<void>
  }
