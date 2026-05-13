export interface IEquipes {
  Id: string;
  NameFr?: string;
  NameEn?: string;
  ParentEquipeId?: string;
  id?: string;
  nameFr?: string;
  nameEn?: string;
  parentEquipeId?: string;
  memberUserIds?: string[];
}

export class Equipe implements IEquipes {
  Id: string = "";
  NameFr?: string;
  NameEn?: string;
  ParentEquipeId?: string;
  id?: string;
  nameFr?: string;
  nameEn?: string;
  parentEquipeId?: string;
  memberUserIds?: string[];
}
