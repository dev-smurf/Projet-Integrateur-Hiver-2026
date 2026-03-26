import type { ModuleSectionDto } from './moduleSection';

export interface ModuleDto {
    id: string;
    name?: string;
    subject?: string;
    content?: string;
    cardImageUrl?: string;
    sections?: ModuleSectionDto[];
}
