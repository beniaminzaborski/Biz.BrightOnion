export class MakeOrder {
  public roomId: number;
  public purchaserId: number;
  public quantity: number;
}

export class Order {
  public orderId: number;
  public roomId: number;
  public day: Date;
  public totalPizzas: number;
  public freeSlicesToGrab: number;
  public orderItems: OrderItem[];
}

export class OrderItem {
  public orderItemId: number;
  public purchaserId: number;
  public quantity: number;
}

export class OrdersApproval {
  public pizzaQuantity: number;
  public room: string;
}
