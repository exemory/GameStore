import {OrderItem} from "./order-item";

export interface OrderCreationData {
  firstName: string,
  lastName: string,
  email: string,
  phone: string,
  paymentType: string,
  comments?: string,

  items: OrderItem[]
}
