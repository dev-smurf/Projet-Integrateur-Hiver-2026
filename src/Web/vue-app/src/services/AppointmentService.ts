import {AxiosResponse} from "axios"
import {injectable} from "inversify"
import {ApiService} from "@/services/apiService"
import {IAppointmentService} from "@/injection/interfaces"
import type {AvailableSlot, AvailabilityData, AvailabilitySlot, AvailabilityOverride} from "@/types/entities"

@injectable()
export class AppointmentService extends ApiService implements IAppointmentService {

  public async getAvailableSlots(startDate: string, endDate: string): Promise<AvailableSlot[]> {
    const response = await this._httpClient
      .get<AvailableSlot[], AxiosResponse<AvailableSlot[]>>(
        `${import.meta.env.VITE_API_BASE_URL}/appointments/available-slots?startDate=${startDate}&endDate=${endDate}`
      )
    return response.data
  }

  public async requestAppointment(date: string, motif?: string): Promise<any> {
    const response = await this._httpClient
      .post(
        `${import.meta.env.VITE_API_BASE_URL}/appointments/request`,
        {date, motif},
        this.headersWithJsonContentType()
      )
    return response.data
  }

  public async respondAppointment(appointmentId: string, accept: boolean, reason?: string): Promise<any> {
    const response = await this._httpClient
      .post(
        `${import.meta.env.VITE_API_BASE_URL}/appointments/respond`,
        {appointmentId, accept, reason},
        this.headersWithJsonContentType()
      )
    return response.data
  }

  public async getAvailability(): Promise<AvailabilityData> {
    const response = await this._httpClient
      .get<AvailabilityData, AxiosResponse<AvailabilityData>>(
        `${import.meta.env.VITE_API_BASE_URL}/appointments/availability`
      )
    return response.data
  }

  public async saveAvailability(slots: AvailabilitySlot[]): Promise<void> {
    await this._httpClient
      .put(
        `${import.meta.env.VITE_API_BASE_URL}/appointments/availability`,
        {slots},
        this.headersWithJsonContentType()
      )
  }

  public async createOverride(data: { date: string, startTime?: string, endTime?: string, isBlocked: boolean }): Promise<AvailabilityOverride> {
    const response = await this._httpClient
      .post<AvailabilityOverride, AxiosResponse<AvailabilityOverride>>(
        `${import.meta.env.VITE_API_BASE_URL}/appointments/availability/overrides`,
        data,
        this.headersWithJsonContentType()
      )
    return response.data
  }

  public async deleteOverride(id: string): Promise<void> {
    await this._httpClient
      .delete(`${import.meta.env.VITE_API_BASE_URL}/appointments/availability/overrides/${id}`)
  }
}
