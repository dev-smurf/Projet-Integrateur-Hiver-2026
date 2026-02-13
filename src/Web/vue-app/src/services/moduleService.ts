import { IModulesService } from "@/injection/interfaces";
import { ApiService } from "./apiService";
import { ICreateModuleRequest } from "@/types/requests";
import { SucceededOrNotResponse } from "@/types/responses";

export class ModuleService extends ApiService implements IModulesService {

    public async createModule(request: ICreateModuleRequest): Promise<SucceededOrNotResponse> {

        try {
            const response = await fetch(
                `${import.meta.env.VITE_API_BASE_URL}/module`,
                {
                    method: "POST",
                    body: request as any
                }
            );

            const data = await response.json();

            return new SucceededOrNotResponse(
                data.succeeded,
                data.errors
            );

        } catch (error: unknown) {

            if (error instanceof Error) {
                console.error(error.message);
            } else {
                console.error("Erreur inconnue", error);
            }

            return new SucceededOrNotResponse(false, [
                {
                    errorType: "network",
                    errorMessage: "Erreur réseau"
                }
            ]);
        }
    }
}
