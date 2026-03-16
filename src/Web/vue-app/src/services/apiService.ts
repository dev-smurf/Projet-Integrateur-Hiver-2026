import axios, {type AxiosInstance, type AxiosResponse, type InternalAxiosRequestConfig} from "axios";
import {inject, injectable} from "inversify";
import {useApiStore} from "@/stores/apiStore";

import "@/extensions/date.extensions";
import Cookies from "universal-cookie";
import {TYPES} from "@/injection/types";
import {IApiService} from "@/injection/interfaces";
import {SucceededOrNotResponse} from "@/types/responses";

@injectable()
export class ApiService implements IApiService {
  _httpClient: AxiosInstance;

  constructor(@inject(TYPES.AxiosInstance) httpClient: AxiosInstance) {
    this._httpClient = httpClient;
 
    this._httpClient.interceptors.request.use(
        async (config) => {

          if (!this.getAccessToken() && this.getRefreshToken()) {
            await this.refreshToken(config, false);
          } else if (this.getAccessToken()) {
            const bearer = `Bearer ${this.getAccessToken()}`;
            config.headers.Authorization = bearer;
            this._httpClient.defaults.headers.common['Authorization'] = bearer;
          }
          return config;
        },
        (error) => {
          return Promise.reject(error);
        }
    );

    this._httpClient.interceptors.response.use(
        (response) => {
          return response;
        },
        async (error) => {
          const originalRequest = error.config;

          // Skip retry if already retried or if error is not 401
          if (error.response?.status !== 401 || originalRequest._retry) {
            return Promise.reject(error);
          }

          originalRequest._retry = true;
          console.log('Request returned 401, attempting to refresh token');

          try {
            await this.refreshToken(originalRequest, true);
            return this._httpClient(originalRequest);
          } catch (refreshError) {
            return Promise.reject(refreshError);
          }
        }
    );
  }

  private getAccessToken() {
    return new Cookies().get("accessToken");
  }

  private getRefreshToken() {
    return new Cookies().get("refreshToken");
  }

  private async refreshToken(
      config: InternalAxiosRequestConfig<any>,
      retryRequest: boolean
  ) {
    try {
      return await axios
          .get(
              `${import.meta.env.VITE_API_BASE_URL}/authentication/refresh-token`,
              { 
                withCredentials: true,
                headers: {
                  'Accept': 'application/json'
                }
              }
          )
          .then((response: AxiosResponse<SucceededOrNotResponse>) => {
            if (!response.data) return;

            const succeededOrNotResponse = response.data;
            if (!succeededOrNotResponse.succeeded) {
              this.logoutUserAndRedirectToHomePage();
              return;
            }

            const bearer = `Bearer ${this.getAccessToken()}`;
            config.headers.Authorization = bearer;
            this._httpClient.defaults.headers.common['Authorization'] = bearer;

            if (retryRequest) {
              return this._httpClient(config)
            }
          })
          .catch((error) => {
            this.logoutUserAndRedirectToHomePage();
            return Promise.reject(error);
          })
    } catch (error) {
      this.logoutUserAndRedirectToHomePage()
      return Promise.reject(error);
    }
  }

  private logoutUserAndRedirectToHomePage() {
    const apiStore = useApiStore();
    apiStore.setNeedToLogout(true);

  }

  public headersWithJsonContentType() {
    return {
      headers: {
        "Content-Type": 'application/json',
      },
    };
  }

  public headersWithFormDataContentType() {
    return {
      headers: {
        "Content-Type": 'multipart/form-data',
      },
    };
  }

  public buildEmptyBody(): string {
    return '{}'
  }
}