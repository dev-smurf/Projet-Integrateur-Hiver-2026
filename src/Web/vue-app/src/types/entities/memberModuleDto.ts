export interface MemberModuleDto {
  moduleId: string;
  nameFr?: string;
  nameEn?: string;
  name?: string;
  sujetFr?: string;
  sujetEn?: string;
  subject?: string;
  cardImageUrl?: string;
  assignedAt?: string;
  progressPercent: number;
  isCompleted: boolean;
}
