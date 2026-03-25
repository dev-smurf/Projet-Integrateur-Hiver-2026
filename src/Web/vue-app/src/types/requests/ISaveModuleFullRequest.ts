export interface ISaveModuleFullRequest {
    name: string;
    subject?: string;
    content?: string;
    sections: ISectionPayload[];
}

export interface ISectionPayload {
    id?: string;
    title: string;
    content?: string;
    sortOrder: number;
    isDeleted: boolean;
}
