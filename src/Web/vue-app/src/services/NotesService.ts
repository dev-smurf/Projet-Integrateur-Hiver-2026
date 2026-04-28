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

export interface CreateNoteRequest {
  memberId: string;
  content: string;
  isPrivate: boolean;
}

export interface INotesService {
  getAllNotes(): Promise<NoteDto[]>;
  createNote(request: CreateNoteRequest): Promise<NoteDto | null>;
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
}
