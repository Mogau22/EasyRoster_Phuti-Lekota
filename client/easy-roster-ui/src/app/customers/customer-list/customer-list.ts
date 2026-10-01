import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  inject
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { CustomerService } from '../../core/services/customer.service';
import { Customer } from '../models/customer';

@Component({
  selector: 'app-customer-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './customer-list.html',
  styleUrl: './customer-list.scss'
})
export class CustomerList implements OnInit, OnChanges {

  private readonly customerService = inject(CustomerService);

  @Input() refreshKey = 0;
  @Output() create = new EventEmitter<void>();
  @Output() edit = new EventEmitter<Customer>();

  customers: Customer[] = [];
  search = '';

  isLoading = false;
  errorMessage = '';

  ngOnInit(): void {
    this.loadCustomers();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (
      changes['refreshKey'] &&
      !changes['refreshKey'].firstChange
    ) {
      this.loadCustomers();
    }
  }

  loadCustomers(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.customerService
      .getCustomers(this.search)
      .subscribe({
        next: customers => {
          this.customers = customers;
          this.isLoading = false;
        },

        error: error => {
          console.error(
            'Failed to load customers:',
            error
          );

          this.errorMessage =
            'Unable to load customers. Please try again.';

          this.isLoading = false;
        }
      });
  }

  searchCustomers(): void {
    this.loadCustomers();
  }

  clearSearch(): void {
    this.search = '';
    this.loadCustomers();
  }

  createCustomer(): void {
  this.create.emit();
  }
  
  editCustomer(customer: Customer): void {
    this.edit.emit(customer);

    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  }

  deleteCustomer(customer: Customer): void {
    const confirmed = window.confirm(
      `Are you sure you want to delete ${customer.firstName} ${customer.surname}?`
    );

    if (!confirmed) {
      return;
    }

    this.customerService
      .deleteCustomer(customer.id)
      .subscribe({
        next: () => {
          this.loadCustomers();
        },

        error: error => {
          console.error(
            'Failed to delete customer:',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Unable to delete customer. Please try again.';
        }
      });
  }
}