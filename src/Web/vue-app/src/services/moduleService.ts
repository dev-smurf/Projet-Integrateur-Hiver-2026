import { IModulesService } from "@/injection/interfaces";
import { injectable } from "inversify";
import { SucceededOrNotResponse } from "@/types/responses";
import { AxiosError, AxiosResponse } from "axios";
import { ApiService } from "@/services";
import { ICreateModuleRequest, IEditModuleRequest } from "@/types";

@injectable()
export class ModuleService extends ApiService implements IModulesService {

    /**
     * Récupère tous les modules
     * URL ciblée au backend : GET /modules
     */
    public async getAllModules(): Promise<any[]> {
        try {
            const response = await this._httpClient.get<any[]>(
                `${import.meta.env.VITE_API_BASE_URL}/modules`
            );
            // On retourne directement les données (ton tableau avec "Module 1")
            return response.data ?? [];
        } catch (error) {
            console.error("Erreur lors de la récupération des modules:", error);
            return [];
        }
    }

    /**
     * Récupère un module spécifique par son ID
     * URL ciblée au backend : GET /module/{id}
     */
    public async getModule(id: string): Promise<any> {
        try {
            const response = await this._httpClient.get<any>(
                `${import.meta.env.VITE_API_BASE_URL}/module/${id}`
            );
            return response.data;
        } catch (error) {
            console.error(`Erreur lors de la récupération du module ${id}:`, error);
            return null;
        }
    }

    /**
     * Création d'un module (Utilise FormData pour gérer l'image)
     * URL ciblée au backend : POST /module
     */
    public async createModule(request: ICreateModuleRequest): Promise<SucceededOrNotResponse> {
        const formData = new FormData();

        // Mapping exact vers les propriétés PascalCase de ton entité C#
        if (request.nameFr) formData.append("NameFr", request.nameFr);
        if (request.nameEn) formData.append("NameEn", request.nameEn);
        if (request.contenueFr) formData.append("ContenueFr", request.contenueFr);
        if (request.contenueEn) formData.append("ContenueEn", request.contenueEn);
        if (request.sujetFr) formData.append("SujetFr", request.sujetFr);
        if (request.sujetEn) formData.append("SujetEn", request.sujetEn);
        if (request.cardImage) formData.append("CardImage", request.cardImage);

        try {
            const response = await this._httpClient.post<SucceededOrNotResponse>(
                `${import.meta.env.VITE_API_BASE_URL}/module`,
                formData,
                this.headersWithFormDataContentType()
            );
            return new SucceededOrNotResponse(response.data.succeeded, response.data.errors);
        } catch (error: any) {
            // Empêche le crash si le serveur renvoie une erreur vide ou un 401
            const serverErrors = error.response?.data?.errors || ["Une erreur de communication avec le serveur est survenue."];
            return new SucceededOrNotResponse(false, serverErrors);
        }
    }

    /**
     * Mise à jour d'un module existant
     * URL ciblée au backend : PUT /module/{id}
     */
    public async updateModule(id: string, request: IEditModuleRequest): Promise<SucceededOrNotResponse> {
        const formData = new FormData();
        formData.append("Id", id);

        if (request.nameFr) formData.append("NameFr", request.nameFr);
        if (request.nameEn) formData.append("NameEn", request.nameEn);
        if (request.contenueFr) formData.append("ContenueFr", request.contenueFr);
        if (request.contenueEn) formData.append("ContenueEn", request.contenueEn);
        if (request.sujetFr) formData.append("SujetFr", request.sujetFr);
        if (request.sujetEn) formData.append("SujetEn", request.sujetEn);
        if (request.cardImage) formData.append("CardImage", request.cardImage);

        try {
            const response = await this._httpClient.put<SucceededOrNotResponse>(
                `${import.meta.env.VITE_API_BASE_URL}/module/${id}`,
                formData,
                this.headersWithFormDataContentType()
            );
            return new SucceededOrNotResponse(response.data.succeeded, response.data.errors);
        } catch (error: any) {
            const serverErrors = error.response?.data?.errors || ["Impossible de modifier le module."];
            return new SucceededOrNotResponse(false, serverErrors);
        }
    }
}