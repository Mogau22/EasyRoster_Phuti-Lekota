import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '../../../environments/environment';
import { Customer, CustomerRequest } from '../../customers/models/customer';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {

  private readonly url =
    `${environment.apiUrl}/customers`;

  constructor(
    private readonly http: HttpClient
  ) {}

  getCustomers(search = '') {
    return this.http.get<Customer[]>(
      `${this.url}?search=${encodeURIComponent(search)}`
    );
  }

  getCustomer(id: number) {
    return this.http.get<Customer>(
      `${this.url}/${id}`
    );
  }

  createCustomer(customer: CustomerRequest) {
    return this.http.post<Customer>(
      this.url,
      customer
    );
  }

  updateCustomer(
    id: number,
    customer: CustomerRequest
  ) {
    return this.http.put<Customer>(
      `${this.url}/${id}`,
      customer
    );
  }

  deleteCustomer(id: number) {
    return this.http.delete<void>(
      `${this.url}/${id}`
    );
  }
}