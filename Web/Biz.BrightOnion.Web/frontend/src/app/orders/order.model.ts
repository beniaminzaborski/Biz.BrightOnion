import { exec } from "child_process";

export class MakeOrder {
  public roomId: number;
  public purchaserId: number;
  public quantity: number;
}

export class CancelOrder {
  public orderId: number;
  public purchaserId: number;
}

export class ApproveOrder {
  public orderId: number;
  public roomId: number;
  public userId: number;
}

export class Order {
  public orderId: number;
  public roomId: number;
  public day: Date;
  public totalPizzas: number = 0;
  public freeSlicesToGrab: number = 0;
  public orderItems: OrderItem[];
  public isApproved: boolean;
}

export class OrderItem {
  public orderItemId: number;
  public purchaserId: number;
  public purchaserEmail: string;
  public quantity: number;
}

export class OrdersApproval {
  public pizzaQuantity: number;
  public room: string;
}
