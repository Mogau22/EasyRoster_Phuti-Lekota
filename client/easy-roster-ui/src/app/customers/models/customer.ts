export interface Customer { id:number; type:string; firstName:string; surname:string; email:string; cellphone:string; amountTotal:number; }
export type CustomerRequest = Omit<Customer, 'id'>;
