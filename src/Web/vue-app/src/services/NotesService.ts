import { injectable, inject } from "inversify";
import { type AxiosInstance } from "axios";
import { TYPES } from "@/injection/types";

export interface NoteDto {
  id: string;
  memberId: string;
  memberName: string;
  createdByAdminId: string;
  createdByAdminName: string;
  content: string;
  isPrivate: boolean;
  created: string;
}

export interface EquipeNoteDto {
  id: string;
  equipeId: string;
  equipeName: string;
  createdByAdminId: string;
  createdByAdminName: string;
  content: string;
  isPrivate: boolean;
  created: string;
}

export interface CreateEquipeNoteRequest {
  equipeId: string;
  content: string;
  isPrivate: boolean;
}

export interface INotesService {
  getAllNotes(): Promise<NoteDto[]>;
  createNote(request: CreateNoteRequest): Promise<NoteDto | null>;
  updateNote(id: string, request: { content: string; isPrivate: boolean }): Promise<NoteDto | null>;
  deleteNote(id: string): Promise<boolean>;
  getMyNotes(): Promise<NoteDto[]>;
  
  // Equipe Notes
  getAllEquipeNotes(): Promise<EquipeNoteDto[]>;
  createEquipeNote(request: CreateEquipeNoteRequest): Promise<EquipeNoteDto | null>;
  updateEquipeNote(id: string, request: { content: string; isPrivate: boolean }): Promise<EquipeNoteDto | null>;
  deleteEquipeNote(id: string): Promise<boolean>;
  getMyEquipeNotes(): Promise<EquipeNoteDto[]>;
}

@injectable()
export class NotesService implements INotesService {
  private readonly _axios: AxiosInstance;

  constructor(@inject(TYPES.AxiosInstance) axios: AxiosInstance) {
    this._axios = axios;
  }

  async getAllNotes(): Promise<NoteDto[]> {
    try {
      const response = await this._axios.get<NoteDto[]>("/api/admin/notes");
      return response.data;
    } catch (error) {
      console.error("Failed to fetch notes", error);
      return [];
    }
  }

  async createNote(request: CreateNoteRequest): Promise<NoteDto | null> {
    try {
      const response = await this._axios.post<NoteDto>("/api/admin/notes", request);
      return response.data;
    } catch (error) {
      console.error("Failed to create note", error);
      return null;
    }
  }

  async updateNote(id: string, request: { content: string; isPrivate: boolean }): Promise<NoteDto | null> {
    try {
      const response = await this._axios.put<NoteDto>(`/api/admin/notes/${id}`, request);
      return response.data;
    } catch (error) {
      console.error("Failed to update note", error);
      return null;
    }
  }

  async deleteNote(id: string): Promise<boolean> {
    try {
      await this._axios.delete(`/api/admin/notes/${id}`);
      return true;
    } catch (error) {
      console.error("Failed to delete note", error);
      return false;
    }
  }

  async getMyNotes(): Promise<NoteDto[]> {
    try {
      const response = await this._axios.get<NoteDto[]>("/api/members/me/notes");
      return response.data;
    } catch (error) {
      console.error("Failed to fetch my notes", error);
      return [];
    }
  }

  // Equipe Notes
  async getAllEquipeNotes(): Promise<EquipeNoteDto[]> {
    try {
      const response = await this._axios.get<EquipeNoteDto[]>("/api/admin/equipe-notes");
      return response.data;
    } catch (error) {
      console.error("Failed to fetch equipe notes", error);
      return [];
    }
  }

  async createEquipeNote(request: CreateEquipeNoteRequest): Promise<EquipeNoteDto | null> {
    try {
      const response = await this._axios.post<EquipeNoteDto>("/api/admin/equipe-notes", request);
      return response.data;
    } catch (error) {
      console.error("Failed to create equipe note", error);
      return null;
    }
  }

  async updateEquipeNote(id: string, request: { content: string; isPrivate: boolean }): Promise<EquipeNoteDto | null> {
    try {
      const response = await this._axios.put<EquipeNoteDto>(`/api/admin/equipe-notes/${id}`, request);
      return response.data;
    } catch (error) {
      console.error("Failed to update equipe note", error);
      return null;
    }
  }

  async deleteEquipeNote(id: string): Promise<boolean> {
    try {
      await this._axios.delete(`/api/admin/equipe-notes/${id}`);
      return true;
    } catch (error) {
      console.error("Failed to delete equipe note", error);
      return false;
    }
  }

  async getMyEquipeNotes(): Promise<EquipeNoteDto[]> {
    try {
      const response = await this._axios.get<EquipeNoteDto[]>("/api/members/me/equipe-notes");
      return response.data;
    } catch (error) {
      console.error("Failed to fetch my equipe notes", error);
      return [];
    }
  }
}
