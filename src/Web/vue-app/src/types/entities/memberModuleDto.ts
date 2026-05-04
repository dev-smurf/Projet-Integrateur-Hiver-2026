export interface MemberModuleDto {
  moduleId: string;
  // Backend currently exposes non-localized fields.
  name?: string;
  subject?: string;
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
