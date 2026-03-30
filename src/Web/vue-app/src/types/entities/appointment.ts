export enum AppointmentStatus {
  Pending = 0,
  Accepted = 1,
  Refused = 2
}

export interface Appointment {
  id: string
  memberId: string
  adminId: string
  date: string
  duration: string
  motif: string | null
  status: AppointmentStatus
  refusalReason: string | null
  conversationId: string
}

export interface AvailableSlot {
  date: string
  startTime: string
  endTime: string
}

export interface AvailabilitySlot {
  id?: string
  dayOfWeek: number
  startTime: string
  endTime: string
}

export interface AvailabilityOverride {
  id?: string
  date: string
  startTime: string | null
  endTime: string | null
  isBlocked: boolean
}

export interface AvailabilityData {
  recurring: AvailabilitySlot[]
  overrides: AvailabilityOverride[]
}
