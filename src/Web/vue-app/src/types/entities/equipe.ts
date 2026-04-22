export interface IEquipes {
  Id: string;
  nameFr?: string;
  nameEn?: string;
  memberIds?: string[];
}

export class Equipe implements IEquipes {
  Id: string = "";
  nameFr?: string;
  nameEn?: string;
  memberIds?: string[];
}
