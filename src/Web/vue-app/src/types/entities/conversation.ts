export interface Conversation {
  id: string
  memberName: string
  adminName: string
  memberId: string
  adminId: string
  lastMessage: string
  lastMessageAt: string
  unreadCount: number
}
