import {AxiosResponse} from "axios"
import {injectable} from "inversify"
import {ApiService} from "@/services/apiService"
import {IEquipeConversationService} from "@/injection/interfaces"
import {EquipeConversation, EquipeMessage} from "@/types/entities"

@injectable()
export class EquipeConversationService extends ApiService implements IEquipeConversationService {

  public async getConversations(): Promise<EquipeConversation[]> {
    const response = await this._httpClient
      .get<EquipeConversation[], AxiosResponse<EquipeConversation[]>>(
        `${import.meta.env.VITE_API_BASE_URL}/equipe-conversations`
      )
    return response.data
  }

  public async getMessages(conversationId: string, page: number = 0, pageSize: number = 30): Promise<EquipeMessage[]> {
    const response = await this._httpClient
      .get<EquipeMessage[], AxiosResponse<EquipeMessage[]>>(
        `${import.meta.env.VITE_API_BASE_URL}/equipe-conversations/${conversationId}/messages?page=${page}&pageSize=${pageSize}`
      )
    return response.data
  }

  public async sendMessage(conversationId: string, text: string, attachment?: File): Promise<EquipeMessage> {
    const formData = new FormData()
    formData.append('conversationId', conversationId)
    if (text) formData.append('text', text)
    if (attachment) formData.append('attachment', attachment)

    const response = await this._httpClient
      .post<EquipeMessage, AxiosResponse<EquipeMessage>>(
        `${import.meta.env.VITE_API_BASE_URL}/equipe-conversations/${conversationId}/messages`,
        formData,
        this.headersWithFormDataContentType()
      )
    return response.data
  }

  public async markAsRead(conversationId: string): Promise<void> {
    await this._httpClient
      .put(`${import.meta.env.VITE_API_BASE_URL}/equipe-conversations/${conversationId}/read`, {}, this.headersWithJsonContentType())
  }

  public async getUnreadCount(): Promise<number> {
    const response = await this._httpClient
      .get<{ count: number }, AxiosResponse<{ count: number }>>(
        `${import.meta.env.VITE_API_BASE_URL}/equipe-conversations/unread-count`
      )
    return response.data.count
  }
}
