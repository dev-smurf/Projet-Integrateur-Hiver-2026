export interface IEquipes {
  id?: string;
  Id?: string;
  nameFr?: string;
  NameFr?: string;
  nameEn?: string;
  NameEn?: string;
  memberIds?: string[];
  MemberIds?: string[];
  MemberUserIds?: string[];
  parentEquipeId?: string;
  ParentEquipeId?: string;
  parentEquipeNameFr?: string;
  ParentEquipeNameFr?: string;
  parentEquipeNameEn?: string;
  ParentEquipeNameEn?: string;
  sousEquipes?: IEquipes[];
  SousEquipes?: IEquipes[];
}

export class Equipe implements IEquipes {
  id?: string;
  Id?: string;
  nameFr?: string;
  NameFr?: string;
  nameEn?: string;
  NameEn?: string;
  memberIds?: string[];
  MemberIds?: string[];
  MemberUserIds?: string[];
  parentEquipeId?: string;
  ParentEquipeId?: string;
  parentEquipeNameFr?: string;
  ParentEquipeNameFr?: string;
  parentEquipeNameEn?: string;
  ParentEquipeNameEn?: string;
  sousEquipes?: Equipe[];
  SousEquipes?: Equipe[];
}
