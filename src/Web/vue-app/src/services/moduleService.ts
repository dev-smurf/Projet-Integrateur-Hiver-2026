import { IModulesService } from "@/injection/interfaces";
import { injectable } from "inversify";
import { SucceededOrNotResponse } from "@/types/responses";
import { ApiService } from "@/services";
import { ICreateModuleRequest, IEditModuleRequest } from "@/types";
import { ModuleDto } from "@/types/entities";
import type { ISaveModuleFullRequest } from "@/types/requests/ISaveModuleFullRequest";
import type { ModuleSectionDto } from "@/types/entities/moduleSection";

@injectable()
export class ModulesApiService extends ApiService implements IModulesService {

    public async getAllModules(): Promise<ModuleDto[]> {
        try {
            const response = await this._httpClient.get<ModuleDto[]>(
                `${import.meta.env.VITE_API_BASE_URL}/modules`
            );
            return response.data ?? [];
        } catch (error) {
            console.error("Erreur lors de la récupération des modules:", error);
            return [];
        }
    }

    public async getModule(id: string): Promise<ModuleDto> {
        try {
            const response = await this._httpClient.get<ModuleDto>(
                `${import.meta.env.VITE_API_BASE_URL}/module/${id}`
            );
            return response.data;
        } catch (error) {
            console.error(`Erreur lors de la récupération du module ${id}:`, error);
            throw error;
        }
    }

    public async getModuleFlexible(id: string): Promise<ModuleDto | null> {
        try {
            return await this.getModule(id);
        } catch {
            try {
                const list = await this.getAllModules();
                const found = list.find((m: ModuleDto) => {
                    const ids = [m.id, (m as any).Id];
                    return ids.some((x: any) => x === id);
                });
                if (found) return found;
                return null;
            } catch (e) {
                console.error('Erreur lors du fallback getAllModules:', e);
                return null;
            }
        }
    }

    public async createModule(request: ICreateModuleRequest): Promise<SucceededOrNotResponse> {
        const formData = this.prepareFormData(request);

        try {
            const response = await this._httpClient.post<ICreateModuleRequest, import("axios").AxiosResponse<any>>(
                `${import.meta.env.VITE_API_BASE_URL}/modules`,
                formData,
                this.headersWithFormDataContentType()
            );

            const errors = Array.isArray(response.data?.errors) ? response.data.errors : [];
            return new SucceededOrNotResponse(response.data?.succeeded ?? true, errors);

        } catch (error: any) {
            const message = error.response?.data?.errors || ["Une erreur de communication est survenue."];
            return new SucceededOrNotResponse(false, Array.isArray(message) ? message : [message]);
        }
    }

    public async updateModule(id: string, request: IEditModuleRequest): Promise<SucceededOrNotResponse> {
        const formData = this.prepareFormData(request);

        try {
            const response = await this._httpClient.put<IEditModuleRequest, import("axios").AxiosResponse<any>>(
                `${import.meta.env.VITE_API_BASE_URL}/module/${id}`,
                formData,
                this.headersWithFormDataContentType()
            );

            const errors = Array.isArray(response.data?.errors) ? response.data.errors : [];
            return new SucceededOrNotResponse(response.data?.succeeded ?? true, errors);

        } catch (error: any) {
            const message = error.response?.data?.errors || ["Impossible de modifier le module."];
            return new SucceededOrNotResponse(false, Array.isArray(message) ? message : [message]);
        }
    }

    public async deleteModule(id: string): Promise<SucceededOrNotResponse> {
        try {
            const response = await this._httpClient.delete<any>(
                `${import.meta.env.VITE_API_BASE_URL}/module/${id}`
            );
            const data = response.data;
            const succeeded = data?.succeeded ?? data?.Succeeded ?? response.status === 200;
            const errors = Array.isArray(data?.errors) ? data.errors
                         : Array.isArray(data?.Errors) ? data.Errors
                         : [];
            return new SucceededOrNotResponse(succeeded, errors);
        } catch (error: any) {
            console.error('[ModuleService] Error during deleteModule:', error);
            const message = error.response?.data?.errors || ["Impossible de supprimer le module."];
            return new SucceededOrNotResponse(false, Array.isArray(message) ? message : [message]);
        }
    }

    public async saveModuleFull(id: string, request: ISaveModuleFullRequest): Promise<SucceededOrNotResponse> {
        try {
            const response = await this._httpClient.put<any>(
                `${import.meta.env.VITE_API_BASE_URL}/module/${id}/full`,
                request,
                this.headersWithJsonContentType()
            );
            const errors = Array.isArray(response.data?.errors) ? response.data.errors : [];
            return new SucceededOrNotResponse(response.data?.succeeded ?? true, errors);
        } catch (error: any) {
            const message = error.response?.data?.errors || ["Impossible de sauvegarder le module."];
            return new SucceededOrNotResponse(false, Array.isArray(message) ? message : [message]);
        }
    }

    public async uploadMedia(file: File): Promise<{ url: string }> {
        const formData = new FormData();
        formData.append('File', file);
        const response = await this._httpClient.post<{ url: string }>(
            `${import.meta.env.VITE_API_BASE_URL}/module/media`,
            formData,
            this.headersWithFormDataContentType()
        );
        return response.data;
    }

    public async getModuleSections(moduleId: string): Promise<ModuleSectionDto[]> {
        const response = await this._httpClient.get<ModuleSectionDto[]>(
            `${import.meta.env.VITE_API_BASE_URL}/module/${moduleId}/sections`
        );
        return response.data ?? [];
    }

    public async assignModule(moduleId: string, memberId: string): Promise<SucceededOrNotResponse> {
        try {
            const response = await this._httpClient.post<any>(
                `${import.meta.env.VITE_API_BASE_URL}/module/${moduleId}/assign`,
                { memberId },
                this.headersWithJsonContentType()
            );
            return new SucceededOrNotResponse(response.data?.succeeded ?? true, response.data?.errors ?? []);
        } catch (error: any) {
            return new SucceededOrNotResponse(false, [error.response?.data?.errors?.[0] || "Erreur d'assignation."]);
        }
    }

    public async unassignModule(moduleId: string, memberId: string): Promise<SucceededOrNotResponse> {
        try {
            const response = await this._httpClient.delete<any>(
                `${import.meta.env.VITE_API_BASE_URL}/module/${moduleId}/assign/${memberId}`
            );
            return new SucceededOrNotResponse(response.data?.succeeded ?? true, response.data?.errors ?? []);
        } catch (error: any) {
            return new SucceededOrNotResponse(false, [error.response?.data?.errors?.[0] || "Erreur de désassignation."]);
        }
    }

    public async getModuleAssignments(moduleId: string): Promise<any[]> {
        try {
            const response = await this._httpClient.get<any[]>(
                `${import.meta.env.VITE_API_BASE_URL}/module/${moduleId}/assignments`
            );
            return response.data ?? [];
        } catch {
            return [];
        }
    }

    public async getMyModules(): Promise<ModuleDto[]> {
        try {
            const response = await this._httpClient.get<ModuleDto[]>(
                `${import.meta.env.VITE_API_BASE_URL}/member/modules`
            );
            return Array.isArray(response.data) ? response.data : [];
        } catch {
            return [];
        }
    }

    public async getMyModuleDetail(moduleId: string): Promise<ModuleDto> {
        const response = await this._httpClient.get<ModuleDto>(
            `${import.meta.env.VITE_API_BASE_URL}/member/modules/${moduleId}`
        );
        return response.data;
    }

    private prepareFormData(request: ICreateModuleRequest | IEditModuleRequest): FormData {
        const formData = new FormData();

        if (request.name) formData.append("Name", request.name);
        if (request.subject) formData.append("Subject", request.subject);
        if (request.content) formData.append("Content", request.content);

        if (request.cardImage) formData.append("CardImage", request.cardImage);

        return formData;
    }
}
