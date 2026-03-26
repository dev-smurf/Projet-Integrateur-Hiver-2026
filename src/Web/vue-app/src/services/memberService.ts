import {AxiosError, AxiosResponse} from "axios"
import {injectable} from "inversify"

import "@/extensions/date.extensions"
import {ApiService} from "@/services/apiService"
import {IMemberService} from "@/injection/interfaces"
import {PaginatedResponse, SucceededOrNotResponse} from "@/types/responses";
import {DashboardSummaryDto, Member, MemberModuleDto} from "@/types/entities";
import {Guid} from "@/types";

@injectable()
export class MemberService extends ApiService implements IMemberService {
  public async getAuthenticated(): Promise<Member | undefined> {
    try {
      const response = await this
        ._httpClient
        .get<Member, AxiosResponse<Member>>(`${import.meta.env.VITE_API_BASE_URL}/members/me`)
      return response.data
    } catch (error) {
      return Promise.reject(error)
    }
  }

  public async search(pageIndex: number, pageSize: number, searchValue: string): Promise<PaginatedResponse<Member>> {
    const response = await this
        ._httpClient
        .get<any, AxiosResponse<PaginatedResponse<Member>>>(
            `${import.meta.env.VITE_API_BASE_URL}/members?pageIndex=${pageIndex}&pageSize=${pageSize}&searchValue=${searchValue}`)
        .catch(function (error: AxiosError): AxiosResponse<PaginatedResponse<Member>> | undefined {
          return error.response
        })
    if (!response) return { items: [], totalCount: 0 } as unknown as PaginatedResponse<Member>
    return response.data as PaginatedResponse<Member>
  }

  public async getMember(id: string): Promise<Member> {
    const response = await this
        ._httpClient
        .get<any, AxiosResponse<Member>>(`${import.meta.env.VITE_API_BASE_URL}/members/${id}`)
        .catch(function (error: AxiosError): AxiosResponse<Member> {
          return error.response as AxiosResponse<Member>
        })
    return response.data as Member
  }

  public async getMemberModules(memberId: string): Promise<MemberModuleDto[]> {
    const response = await this
      ._httpClient
      .get<MemberModuleDto[], AxiosResponse<MemberModuleDto[]>>(
        `${import.meta.env.VITE_API_BASE_URL}/members/${memberId}/modules`)
      .catch(function (error: AxiosError): AxiosResponse<MemberModuleDto[]> {
        return error.response as AxiosResponse<MemberModuleDto[]>
      })

    return response?.data ?? []
  }

  public async getMyModules(): Promise<MemberModuleDto[]> {
    const response = await this
      ._httpClient
      .get<MemberModuleDto[], AxiosResponse<MemberModuleDto[]>>(
        `${import.meta.env.VITE_API_BASE_URL}/members/me/modules`)
      .catch(function (error: AxiosError): AxiosResponse<MemberModuleDto[]> {
        return error.response as AxiosResponse<MemberModuleDto[]>
      })

    return response?.data ?? []
  }

  public async getDashboardSummary(): Promise<DashboardSummaryDto | null> {
    const response = await this
      ._httpClient
      .get<DashboardSummaryDto, AxiosResponse<DashboardSummaryDto>>(
        `${import.meta.env.VITE_API_BASE_URL}/admin/dashboard/summary`)
      .catch(function (error: AxiosError): AxiosResponse<DashboardSummaryDto> {
        return error.response as AxiosResponse<DashboardSummaryDto>
      })

    return response?.data ?? null
  }

  public async getRecentMembers(days = 30, pageSize = 50, searchValue = ""): Promise<Member[]> {
    const response = await this
      ._httpClient
      .get<Member[], AxiosResponse<Member[]>>(
        `${import.meta.env.VITE_API_BASE_URL}/admin/members/recent?days=${days}&pageSize=${pageSize}&searchValue=${encodeURIComponent(searchValue)}`)
      .catch(function (error: AxiosError): AxiosResponse<Member[]> {
        return error.response as AxiosResponse<Member[]>
      })

    return response?.data ?? []
  }

  public async addModuleToMember(memberId: string, moduleId: string): Promise<SucceededOrNotResponse> {
    const response = await this
      ._httpClient
      .post<any, AxiosResponse<SucceededOrNotResponse>>(
        `${import.meta.env.VITE_API_BASE_URL}/members/${memberId}/modules/${moduleId}`,
        {},
        this.headersWithJsonContentType())
      .catch(function (error: AxiosError): AxiosResponse<SucceededOrNotResponse> {
        return error.response as AxiosResponse<SucceededOrNotResponse>
      })

    const succeededOrNotResponse = response.data as SucceededOrNotResponse
    return new SucceededOrNotResponse(succeededOrNotResponse?.succeeded ?? true, succeededOrNotResponse?.errors)
  }

  public async updateMemberModuleProgress(memberId: string, moduleId: string, progressPercent: number): Promise<SucceededOrNotResponse> {
    const response = await this
      ._httpClient
      .put<any, AxiosResponse<SucceededOrNotResponse>>(
        `${import.meta.env.VITE_API_BASE_URL}/members/${memberId}/modules/${moduleId}/progress`,
        { progressPercent },
        this.headersWithJsonContentType())
      .catch(function (error: AxiosError): AxiosResponse<SucceededOrNotResponse> {
        return error.response as AxiosResponse<SucceededOrNotResponse>
      })

    const succeededOrNotResponse = response.data as SucceededOrNotResponse
    return new SucceededOrNotResponse(succeededOrNotResponse?.succeeded ?? true, succeededOrNotResponse?.errors)
  }

  public async removeModuleFromMember(memberId: string, moduleId: string): Promise<SucceededOrNotResponse> {
    const response = await this
      ._httpClient
      .delete<any, AxiosResponse<SucceededOrNotResponse>>(
        `${import.meta.env.VITE_API_BASE_URL}/members/${memberId}/modules/${moduleId}`)
      .catch(function (error: AxiosError): AxiosResponse<SucceededOrNotResponse> {
        return error.response as AxiosResponse<SucceededOrNotResponse>
      })

    const succeededOrNotResponse = response.data as SucceededOrNotResponse
    return new SucceededOrNotResponse(succeededOrNotResponse?.succeeded ?? true, succeededOrNotResponse?.errors)
  }

  public async createMember(member: Member): Promise<SucceededOrNotResponse> {
    const response = await this
        ._httpClient
        .post<any, AxiosResponse<SucceededOrNotResponse>>(
            `${import.meta.env.VITE_API_BASE_URL}/members`,
            member,
            this.headersWithJsonContentType())
        .catch(function (error: AxiosError): AxiosResponse<SucceededOrNotResponse> {
          return error.response as AxiosResponse<SucceededOrNotResponse>
        })
    const succeededOrNotResponse = response.data as SucceededOrNotResponse
    return new SucceededOrNotResponse(succeededOrNotResponse.succeeded, succeededOrNotResponse.errors)
  }

  public async updateMember(member: Member): Promise<SucceededOrNotResponse> {
    const response = await this
        ._httpClient
        .put<any, AxiosResponse<SucceededOrNotResponse>>(
            `${import.meta.env.VITE_API_BASE_URL}/members/${member.id}`,
            member,
            this.headersWithJsonContentType())
        .catch(function (error: AxiosError): AxiosResponse<SucceededOrNotResponse> {
          return error.response as AxiosResponse<SucceededOrNotResponse>
        })
    const succeededOrNotResponse = response.data as SucceededOrNotResponse
    return new SucceededOrNotResponse(succeededOrNotResponse.succeeded, succeededOrNotResponse.errors)
  }

  public async updateMyProfile(data: {
    firstName: string; lastName: string;
    phoneNumber?: string; phoneExtension?: number;
    apartment?: number; street?: string; city?: string; zipCode?: string;
  }): Promise<SucceededOrNotResponse> {
    const response = await this
      ._httpClient
      .put<any, AxiosResponse<SucceededOrNotResponse>>(
        `${import.meta.env.VITE_API_BASE_URL}/members/me`,
        data,
        this.headersWithJsonContentType())
      .catch(function (error: AxiosError): AxiosResponse<SucceededOrNotResponse> {
        return error.response as AxiosResponse<SucceededOrNotResponse>
      })
    const succeededOrNotResponse = response.data as SucceededOrNotResponse
    return new SucceededOrNotResponse(succeededOrNotResponse.succeeded, succeededOrNotResponse.errors)
  }

  public async deleteMember(id: Guid): Promise<SucceededOrNotResponse> {
    const response = await this
        ._httpClient
        .delete<any, AxiosResponse<SucceededOrNotResponse>>(`${import.meta.env.VITE_API_BASE_URL}/members/${id}`)
        .catch(function (error: AxiosError): AxiosResponse<SucceededOrNotResponse> {
          return error.response as AxiosResponse<SucceededOrNotResponse>
        })
    return new SucceededOrNotResponse(response.status === 204)
  }
}
