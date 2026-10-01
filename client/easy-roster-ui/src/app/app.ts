import { Component } from '@angular/core';

import { Customer } from './customers/models/customer';
import { CustomerFormComponent } from './customers/customer-form/customer-form.component';
import { CustomerList } from './customers/customer-list/customer-list';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CustomerFormComponent,
    CustomerList
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {

  selectedCustomer: Customer | null = null;
  showForm = false;
  refreshKey = 0;

  createCustomer(): void {
    this.selectedCustomer = null;
    this.showForm = true;
  }

  editCustomer(customer: Customer): void {
    this.selectedCustomer = customer;
    this.showForm = true;
  }

  customerSaved(): void {
    this.selectedCustomer = null;
    this.showForm = false;

    // Forces customer list to reload after Create/Update
    this.refreshKey++;
  }

  cancelForm(): void {
    this.selectedCustomer = null;
    this.showForm = false;
  }
}