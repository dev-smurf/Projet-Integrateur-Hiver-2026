export interface MemberModuleDto {
  moduleId: string;
  // Backend currently exposes non-localized fields.
  name?: string;
  subject?: string;
  nameFr?: string;
  nameEn?: string;
  sujetFr?: string;
  sujetEn?: string;
  cardImageUrl?: string;
  progressPercent: number;
  isCompleted: boolean;
}
