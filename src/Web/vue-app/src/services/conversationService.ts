import {AxiosResponse} from "axios"
import {injectable} from "inversify"
import {ApiService} from "@/services/apiService"
import {IConversationService} from "@/injection/interfaces"
import {ChatMessage, Conversation} from "@/types/entities"

@injectable()
export class ConversationService extends ApiService implements IConversationService {

  public async getConversations(): Promise<Conversation[]> {
    const response = await this._httpClient
      .get<Conversation[], AxiosResponse<Conversation[]>>(
        `${import.meta.env.VITE_API_BASE_URL}/conversations`
      )
    return response.data
  }

  public async getMessages(conversationId: string, page: number = 0, pageSize: number = 30): Promise<ChatMessage[]> {
    const response = await this._httpClient
      .get<ChatMessage[], AxiosResponse<ChatMessage[]>>(
        `${import.meta.env.VITE_API_BASE_URL}/conversations/${conversationId}/messages?page=${page}&pageSize=${pageSize}`
      )
    return response.data
  }

  public async createConversation(memberId: string): Promise<{ id: string }> {
    const response = await this._httpClient
      .post<{ id: string }, AxiosResponse<{ id: string }>>(
        `${import.meta.env.VITE_API_BASE_URL}/conversations`,
        { memberId },
        this.headersWithJsonContentType()
      )
    return response.data
  }

  public async sendMessage(conversationId: string, text: string): Promise<ChatMessage> {
    const response = await this._httpClient
      .post<ChatMessage, AxiosResponse<ChatMessage>>(
        `${import.meta.env.VITE_API_BASE_URL}/conversations/${conversationId}/messages`,
        { conversationId, text },
        this.headersWithJsonContentType()
      )
    return response.data
  }

  public async markAsRead(conversationId: string): Promise<void> {
    await this._httpClient
      .put(`${import.meta.env.VITE_API_BASE_URL}/conversations/${conversationId}/read`, {}, this.headersWithJsonContentType())
  }

  public async getUnreadCount(): Promise<number> {
    const response = await this._httpClient
      .get<{ count: number }, AxiosResponse<{ count: number }>>(
        `${import.meta.env.VITE_API_BASE_URL}/conversations/unread-count`
      )
    return response.data.count
  }
}
