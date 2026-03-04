export interface ChatMessage {
  id: string
  text: string | null
  senderId: string
  senderName: string
  date: string
  readAt: string | null
  conversationId: string
  attachmentUrl: string | null
  attachmentFileName: string | null
  attachmentContentType: string | null
}
