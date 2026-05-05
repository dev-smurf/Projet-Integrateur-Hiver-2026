export interface IEquipes {
  id: string;
  nameFr?: string;
  nameEn?: string;
  memberIds?: string[];
  parentEquipeId?: string;
  sousEquipes?: IEquipes[];
}

export class Equipe implements IEquipes {
  id: string = "";
  nameFr?: string;
  nameEn?: string;
  memberIds?: string[];
  parentEquipeId?: string;
  sousEquipes?: Equipe[];
}
