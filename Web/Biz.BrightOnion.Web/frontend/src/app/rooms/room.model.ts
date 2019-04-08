export class Room {
  public id: number;
  public name: string;
  public isActive: boolean;
  public managerId: number;
  public managerName: string;
  public slicesPerPizza: number = 8;
}

export class User {
  public userId: number;
  public email: string;
}
