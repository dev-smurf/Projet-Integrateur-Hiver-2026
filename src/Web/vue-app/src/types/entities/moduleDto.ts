import type { ModuleSectionDto } from './moduleSection';

export interface ModuleDto {
    id: string;
    name?: string;
    nameFr?: string;
    nameEn?: string;
    subject?: string;
    sujetFr?: string;
    sujetEn?: string;
    content?: string;
    cardImageUrl?: string;
    isPublished?: boolean;
    sections?: ModuleSectionDto[];
}
