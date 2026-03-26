export enum MessageType {
  Text = 0,
  AppointmentRequest = 1,
  AppointmentResponse = 2
}

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
  type: MessageType
  appointmentId: string | null
  appointmentDate: string | null
  appointmentStatus: number | null
  appointmentMotif: string | null
}
