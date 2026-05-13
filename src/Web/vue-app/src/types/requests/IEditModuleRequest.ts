export interface IEditModuleRequest {
    id?: string;
    name?: string;
    subject?: string;
    content?: string;
    isPublished?: boolean;
    cardImage?: File | null;
}
