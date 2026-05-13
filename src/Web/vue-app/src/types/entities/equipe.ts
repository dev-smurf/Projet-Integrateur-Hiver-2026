export interface IEquipes {
  Id: string;
  id?: string;
  nameFr?: string;
  nameEn?: string;
  parentEquipeId?: string;
  memberUserIds?: string[];
}

export class Equipe implements IEquipes {
  Id: string = "";
  id?: string;
  nameFr?: string;
  nameEn?: string;
  parentEquipeId?: string;
  memberUserIds?: string[];
}
