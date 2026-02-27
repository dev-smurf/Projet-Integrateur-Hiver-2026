import { IModulesService } from "@/injection/interfaces";
import { injectable } from "inversify";
import { ApiService } from "@/services";
import { SucceededOrNotResponse } from "@/types/responses";
import { AxiosError, AxiosResponse } from "axios";
import { ICreateModuleRequest } from "@/types/requests/createModuleRequest";
import { IEditModuleRequest } from "@/types/requests/editModuleRequest";

@injectable()
export class ModuleService extends ApiService implements IModulesService {

  public async createModule(request: ICreateModuleRequest): Promise<SucceededOrNotResponse> {
    const formData = new FormData();

    formData.append("NameFr",      request.nameFr ?? "");
    formData.append("NameEn",      request.nameEn || request.nameFr || "");       // 👈 fallback Fr
    formData.append("ContenueFr",  request.contenueFr ?? "");
    formData.append("ContenueEn",  request.contenueEn || request.contenueFr || ""); // 👈 fallback Fr
    formData.append("SujetFr",     request.sujetFr ?? "");
    formData.append("SujetEn",     request.sujetEn || request.sujetFr || "");     // 👈 fallback Fr

    if (request.cardImage) {
      const base64 = await fileToBase64(request.cardImage);
      formData.append("CardImageBase64", base64);
    }

    const response = await this._httpClient
      .post<ICreateModuleRequest, AxiosResponse<any>>(
        `${import.meta.env.VITE_API_BASE_URL}/module`,
        formData
      )
      .catch(function (error: AxiosError): AxiosResponse<SucceededOrNotResponse> {
        return error.response as AxiosResponse<SucceededOrNotResponse>;
      });

    const succeededOrNotResponse = response.data as SucceededOrNotResponse;
    return new SucceededOrNotResponse(succeededOrNotResponse.succeeded, succeededOrNotResponse.errors);
  }

  public async updateModule(id: string, request: IEditModuleRequest): Promise<SucceededOrNotResponse> {
    const formData = new FormData();

    formData.append("Id", id);
    formData.append("NameFr",      request.nameFr ?? "");
    formData.append("NameEn",      request.nameEn || request.nameFr || "");       // 👈 fallback Fr
    formData.append("ContenueFr",  request.contenueFr ?? "");
    formData.append("ContenueEn",  request.contenueEn || request.contenueFr || ""); // 👈 fallback Fr
    formData.append("SujetFr",     request.sujetFr ?? "");
    formData.append("SujetEn",     request.sujetEn || request.sujetFr || "");     // 👈 fallback Fr

    if (request.cardImage) {
      const base64 = await fileToBase64(request.cardImage);
      formData.append("CardImageBase64", base64);
    }

    const response = await this._httpClient
      .put<IEditModuleRequest, AxiosResponse<SucceededOrNotResponse>>(
        `${import.meta.env.VITE_API_BASE_URL}/module/${id}`,
        formData
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

function fileToBase64(file: File): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => {
      const result = reader.result as string;
      resolve(result.split(",")[1]);
    };
    reader.onerror = reject;
    reader.readAsDataURL(file);
  });
}