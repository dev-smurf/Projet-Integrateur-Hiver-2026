export interface EquipeMessage {
  id: string
  text: string | null
  senderId: string
  senderName: string
  date: string
  equipeConversationId: string
  equipeId?: string
  attachmentUrl: string | null
  attachmentFileName: string | null
  attachmentContentType: string | null
}
