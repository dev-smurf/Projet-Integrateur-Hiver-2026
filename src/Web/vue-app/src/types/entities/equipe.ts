export interface IEquipes {
  Id: string;
  nameFr?: string;
  nameEn?: string;
  memberUserIds?: string[];
}

export class Equipe implements IEquipes {
  Id: string = "";
  nameFr?: string;
  nameEn?: string;
  memberUserIds?: string[];
}
