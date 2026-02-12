export interface IModules {
  nom : string
  contenue : string
  sujet : string 
}

export class Module implements IModules {
  nom : string=""
  contenue: string=""
  sujet : string=""  
}