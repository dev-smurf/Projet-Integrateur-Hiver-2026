export interface IModules {
    Id:string
 nameFr?: string
    nameEn?: string
    contenueFr?: string
    contenueEn?: string
    cardImage?: File
    sujetFr?:string
    sujetEn?:string
}

export class Module implements IModules {
    Id: string
    nameFr?: string
    nameEn?: string
    contenueFr?: string
    contenueEn?: string
   cardImage?: File
    sujetFr?:string
    sujetEn?:string
}