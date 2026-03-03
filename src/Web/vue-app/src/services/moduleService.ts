import { IModulesService } from "@/injection/interfaces";
import { injectable } from "inversify";
import { SucceededOrNotResponse } from "@/types/responses";
import { ApiService } from "@/services";
import { ICreateModuleRequest, IEditModuleRequest } from "@/types";
import { ModuleDto } from "@/types/entities";

@injectable()
export class ModulesApiService extends ApiService implements IModulesService {

    /**
     * Récupère tous les modules
     */
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

    /**
     * Récupère un module spécifique par son ID
     */
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
                    const ids = [m.id, (m as any).Id, (m as any).IdString, ((m as any).Id && (m as any).Id.toString && (m as any).Id.toString())];
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
                console.error('Erreur lors du fallback getAllModules:', e);
                return null;
            }
        }
    }

    /**
     * Création d'un module
     */
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

    /**
     * Mise à jour d'un module existant
     */
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

    /**
     * Suppression (soft delete) d'un module
     */
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

    /**
     * Prépare le FormData pour POST et PUT
     */
    private prepareFormData(request: ICreateModuleRequest | IEditModuleRequest): FormData {
        const formData = new FormData();

        // Champs FR
        if (request.nameFr) formData.append("NameFr", request.nameFr);
        if (request.sujetFr) formData.append("SujetFr", request.sujetFr);
        if (request.contenueFr) formData.append("ContenueFr", request.contenueFr);


        // Image
        if (request.cardImage) formData.append("CardImage", request.cardImage);

        return formData;
    }
}
