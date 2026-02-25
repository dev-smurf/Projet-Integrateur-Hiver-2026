import { injectable } from "inversify";
import { ApiService } from "./apiService";
import { AxiosResponse } from "axios";
import { Module } from "@/types/entities/module";
import { MOCK_MODULES } from "./mockData";


const USE_MOCK_DATA = true;

export interface IModuleService {
  getAll(): Promise<Module[]>;
}

@injectable()
export class ModuleService extends ApiService implements IModuleService {
  public async getAll(): Promise<Module[]> {
    if (USE_MOCK_DATA) {
     
      return new Promise(resolve => {
        setTimeout(() => resolve(MOCK_MODULES), 300);
      });
    }


    try {
      const response = await this._httpClient.get<any, AxiosResponse<Module[]>>(
        `${import.meta.env.VITE_API_BASE_URL}/administatrion/modules`
      );
      return response.data as Module[];
    } catch (error) {
      console.error("Erreur lors du chargement des modules:", error);
      return [];
    }
  }
}
