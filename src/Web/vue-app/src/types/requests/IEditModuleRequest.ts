export interface IEditModuleRequest {
    id?: string;
    name?: string;
    subject?: string;
    content?: string;
    cardImage?: File | null;
}
