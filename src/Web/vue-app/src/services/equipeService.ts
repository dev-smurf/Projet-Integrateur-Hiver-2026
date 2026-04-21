import { IEquipesService } from "@/injection/interfaces";
import { injectable } from "inversify";
import { SucceededOrNotResponse } from "@/types/responses";
import { ApiService } from "@/services";
import { ICreateEquipeRequest } from "@/types";
import { Equipe } from "@/types/entities";
import { IEditEquipeRequest } from "@/types/requests/IEditEquipeRequest";

export interface MyEquipeResponse {
  id?: string;
  nameFr?: string;
  nameEn?: string;
  members: MyEquipeMember[];
  modules: MyEquipeModule[];
}

export interface MyEquipeMember {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
}

export interface MyEquipeModule {
  moduleId: string;
  nameFr?: string;
  nameEn?: string;
  sujetFr?: string;
  sujetEn?: string;
  cardImageUrl?: string;
  progressPercent: number;
  isCompleted: boolean;
}

export interface GetEquipeMembersResponse {
  equipeId: string;
  members: {
    id: string;
    memberId: string;
    equipeId: string;
    firstname?: string;
    lastname?: string;
    email?: string;
  }[];
}

@injectable()
export class EquipeService extends ApiService implements IEquipesService {
  public async assignMembersToEquipe(equipeId: string, memberIds: string[]): Promise<SucceededOrNotResponse> {
    const response = await this._httpClient.post<any, import("axios").AxiosResponse<SucceededOrNotResponse>>(
      `${import.meta.env.VITE_API_BASE_URL}/equipes/${equipeId}/assign-members`,
      { memberIds },
      this.headersWithJsonContentType(),
    );

    const data = response.data;
      return new SucceededOrNotResponse(
          data?.succeeded ?? (response.status >= 200 && response.status < 300),
          data?.errors ?? [],
      );
  }

  public async removeMemberFromEquipe(equipeId: string, memberId: string): Promise<SucceededOrNotResponse> {
    const response = await this._httpClient.delete<any, import("axios").AxiosResponse<SucceededOrNotResponse>>(
      `${import.meta.env.VITE_API_BASE_URL}/equipes/${equipeId}/members/${memberId}`,
    );

    const data = response.data;
      return new SucceededOrNotResponse(
          data?.succeeded ?? (response.status >= 200 && response.status < 300),
          data?.errors ?? [],
      );
  }

  public async getMyEquipe(): Promise<MyEquipeResponse | null> {
    try {
      const response = await this._httpClient.get<MyEquipeResponse>(
        `${import.meta.env.VITE_API_BASE_URL}/members/me/equipe`,
      );
      return response.data ?? null;
    } catch {
      return null;
    }
  }

  public async getAllEquipes(): Promise<Equipe[]> {
    try {
      const response = await this._httpClient.get<Equipe[]>(
        `${import.meta.env.VITE_API_BASE_URL}/equipes`,
      );
      return response.data ?? [];
    } catch (error) {
      console.error("Erreur lors de la récupération des équipes:", error);
      return [];
    }
  }

  public async getEquipe(id: string): Promise<Equipe> {
    try {
      const response = await this._httpClient.get<Equipe>(
        `${import.meta.env.VITE_API_BASE_URL}/equipe/${id}`,
      );
      return response.data;
    } catch (error) {
      console.error(`Erreur lors de la récupération de l'équipe ${id}:`, error);
      throw error;
    }
  }

  public async getEquipeFlexible(id: string): Promise<Equipe | null> {
    try {
      return await this.getEquipe(id);
    } catch {
      try {
        const list = await this.getAllEquipes();
        const found = list.find((m: Equipe) => {
          const ids = [
            m.Id,
            (m as any).id,
            (m as any).IdString,
            (m as any).Id && (m as any).Id.toString && (m as any).Id.toString(),
          ];
          return ids.some((x: any) => x === id);
        });
        if (found) return found;

        const num = Number(id);
        if (!isNaN(num)) {
          if (num > 0 && num <= list.length) return list[num - 1];
          if (num >= 0 && num < list.length) return list[num];
        }
        return null;
      } catch (e) {
        console.error("Erreur lors du fallback getAllModules:", e);
        return null;
      }
    }
  }

  public async createEquipe(
    request: ICreateEquipeRequest,
  ): Promise<SucceededOrNotResponse> {
    try {
      const response = await this._httpClient.post<
        ICreateEquipeRequest,
        import("axios").AxiosResponse<any>
      >(
        `${import.meta.env.VITE_API_BASE_URL}/equipes`,
        request,
        { headers: { "Content-Type": "application/json" } },
      );

      const errors = Array.isArray(response.data?.errors)
        ? response.data.errors
        : [];
      return new SucceededOrNotResponse(
        response.data?.succeeded ?? true,
        errors,
      );
    } catch (error: any) {
      const message = error.response?.data?.errors || [
        "Une erreur de communication est survenue.",
      ];
      return new SucceededOrNotResponse(
        false,
        Array.isArray(message) ? message : [message],
      );
    }
  }

  public async updateEquipe(
    id: string,
    request: IEditEquipeRequest,
  ): Promise<SucceededOrNotResponse> {
    try {
      const response = await this._httpClient.put<SucceededOrNotResponse>(
        `${import.meta.env.VITE_API_BASE_URL}/equipe/${id}`,
        request,
        { headers: { "Content-Type": "application/json" } },
      );

      const errors = Array.isArray(response.data?.errors)
        ? response.data.errors
        : [];
      return new SucceededOrNotResponse(
        response.data?.succeeded ?? true,
        errors,
      );
    } catch (error: any) {
      const message = error.response?.data?.errors || [
        "Impossible de modifier l'équipe.",
      ];
      return new SucceededOrNotResponse(
        false,
        Array.isArray(message) ? message : [message],
      );
    }
  }

  public async deleteEquipe(id: string): Promise<SucceededOrNotResponse> {
    try {
      const response = await this._httpClient.delete<any>(
        `${import.meta.env.VITE_API_BASE_URL}/equipe/${id}`,
      );
      const data = response.data;
      const succeeded =
        data?.succeeded ?? data?.Succeeded ?? response.status === 200;
      const errors = Array.isArray(data?.errors)
        ? data.errors
        : Array.isArray(data?.Errors)
          ? data.Errors
          : [];
      return new SucceededOrNotResponse(succeeded, errors);
    } catch (error: any) {
      const message = error.response?.data?.errors || [
        "Impossible de supprimer l'équipe.",
      ];
      return new SucceededOrNotResponse(
        false,
        Array.isArray(message) ? message : [message],
      );
    }
  }

  public async getEquipeMembers(equipeId: string): Promise<GetEquipeMembersResponse> {
    const response = await this._httpClient.get<GetEquipeMembersResponse>(
      `${import.meta.env.VITE_API_BASE_URL}/equipes/${equipeId}/members`,
    );

    return response.data;
  }
}
