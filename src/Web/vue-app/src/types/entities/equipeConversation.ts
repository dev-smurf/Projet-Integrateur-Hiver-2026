export interface EquipeConversation {
  id: string
  equipeId: string
  equipeName: string
  equipeNameFr?: string
  equipeNameEn?: string
  membersCount: number
  lastMessage: string | null
  lastMessageAt: string
  unreadCount: number
}
