export interface ChatMessage {
  id: string
  text: string
  senderId: string
  senderName: string
  date: string
  readAt: string | null
  conversationId: string
}
