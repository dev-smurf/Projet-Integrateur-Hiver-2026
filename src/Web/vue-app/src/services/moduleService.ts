import { IModulesService } from "@/injection/interfaces";
import { ApiService } from "./apiService";
import { ICreateModuleRequest } from "@/types/requests";
import { SucceededOrNotResponse } from "@/types/responses";

export class ModuleService extends ApiService implements IModulesService {

    public async createModule(request: ICreateModuleRequest): Promise<SucceededOrNotResponse> {
        try {
           const apiUrl = import.meta.env.VITE_API_BASE_URL;

            const response = await fetch(`${apiUrl}/module`, {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify(request),
            });

            let data: any;
            try {
                data = await response.json();
            } catch {
                data = {
                    succeeded: false,
                    errors: [{ errorType: "server", errorMessage: "Réponse invalide" }]
                };
            }

            if (!response.ok) {
                return new SucceededOrNotResponse(
                    data.succeeded ?? false,
                    data.errors ?? [{ errorType: "server", errorMessage: `HTTP ${response.status}` }]
                );
            }

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
