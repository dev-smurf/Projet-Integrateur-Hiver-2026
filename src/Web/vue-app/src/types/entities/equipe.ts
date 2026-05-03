export interface IEquipes {
  id?: string;
  Id: string;
  nameFr?: string;
  NameFr?: string;
  nameEn?: string;
  NameEn?: string;
  memberIds?: string[];
  MemberIds?: string[];
  MemberUserIds?: string[];
  parentEquipeId?: string;
  ParentEquipeId?: string;
}

export class Equipe implements IEquipes {
  id?: string;
  Id: string = "";
  nameFr?: string;
  NameFr?: string;
  nameEn?: string;
  NameEn?: string;
  memberIds?: string[];
  MemberIds?: string[];
  MemberUserIds?: string[];
  parentEquipeId?: string;
  ParentEquipeId?: string;
}
