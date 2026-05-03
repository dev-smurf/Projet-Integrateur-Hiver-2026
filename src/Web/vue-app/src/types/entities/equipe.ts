export interface IEquipes {
  id?: string;
  Id: string;
  nameFr?: string;
  nameEn?: string;
  memberIds?: string[];
  parentEquipeId?: string;
}

export class Equipe implements IEquipes {
  id?: string;
  Id: string = "";
  nameFr?: string;
  nameEn?: string;
  memberIds?: string[];
  parentEquipeId?: string;
}
