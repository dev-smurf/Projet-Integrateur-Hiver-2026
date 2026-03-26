export interface IModules {
    Id: string;
    name?: string;
    content?: string;
    cardImage?: File;
    subject?: string;
}

export class Module implements IModules {
    Id: string = '';
    name?: string;
    content?: string;
    cardImage?: File;
    subject?: string;
}
