import { IModulesService } from "@/injection/interfaces";
import { injectable } from "inversify";
import { SucceededOrNotResponse } from "@/types/responses";
import { AxiosError, AxiosResponse } from "axios";
import { ApiService } from "@/services";
import { ICreateModuleRequest, IEditModuleRequest } from "@/types/requests";

@injectable()
export class ModuleService extends ApiService implements IModulesService {


    public async createModule(request: ICreateModuleRequest): Promise<SucceededOrNotResponse> {
        const formData = new FormData();
        if (request.nameFr) formData.append("NameFr", request.nameFr);
        if (request.nameEn) formData.append("NameEn", request.nameEn);
        if (request.contenueFr) formData.append("ContenueFr", request.contenueFr);
        if (request.contenueEn) formData.append("ContenueEn", request.contenueEn);
        if (request.sujetFr) formData.append("SujetFr", request.sujetFr);
        if (request.sujetEn) formData.append("SujetEn", request.sujetEn);
        if (request.cardImage) formData.append("CardImage", request.cardImage);

        const response = await this._httpClient
            .post<ICreateModuleRequest, AxiosResponse<SucceededOrNotResponse>>(
                `${import.meta.env.VITE_API_BASE_URL}/module`,
                formData,
                this.headersWithFormDataContentType()
            )
            .catch((error: AxiosError): AxiosResponse<SucceededOrNotResponse> => {
                return error.response as AxiosResponse<SucceededOrNotResponse>;
            });

        const data = response?.data;
        return new SucceededOrNotResponse(
            data?.succeeded ?? false,
            data?.errors ?? []
        );
    }

    public async updateModule(id: string, request: IEditModuleRequest): Promise<SucceededOrNotResponse> {
        const formData = new FormData();

        formData.append("Id", id);

        if (request.nameFr) formData.append("NameFr", request.nameFr);
        if (request.nameEn) formData.append("NameEn", request.nameEn);
        if (request.contenueFr) formData.append("ContenueFr", request.contenueFr);
        if (request.contenueEn) formData.append("ContenueEn", request.contenueEn);
        if (request.sujetFr) formData.append("SujetFr", request.sujetFr);
        if (request.sujetEn) formData.append("SujetEn", request.sujetEn);

        if (request.cardImage) {
            formData.append("CardImage", request.cardImage);
        }

        const response = await this._httpClient
            .put<IEditModuleRequest, AxiosResponse<SucceededOrNotResponse>>(
                `${import.meta.env.VITE_API_BASE_URL}/module/${id}`,
                formData,
                this.headersWithFormDataContentType()
            )
            .catch((error: AxiosError): AxiosResponse<SucceededOrNotResponse> => {
                return error.response as AxiosResponse<SucceededOrNotResponse>;
            });

        const data = response?.data;
        return new SucceededOrNotResponse(
            data?.succeeded ?? false,
            data?.errors ?? []
        );
    }
}