import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges
} from '@angular/core';

import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { CustomerService } from '../../core/services/customer.service';
import { Customer, CustomerRequest } from '../models/customer';

@Component({
  selector: 'app-customer-form',
  standalone: true,
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './customer-form.component.html',
  styleUrl: './customer-form.component.scss'
})
export class CustomerFormComponent implements OnChanges {

  @Input() customer: Customer | null = null;

  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  isSubmitting = false;
  successMessage = '';
  errorMessage = '';

  readonly customerForm = new FormGroup({
    type: new FormControl('Person', {
      nonNullable: true,
      validators: [
        Validators.required
      ]
    }),

    firstName: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.maxLength(100)
      ]
    }),

    surname: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.maxLength(100)
      ]
    }),

    email: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.email
      ]
    }),

    cellphone: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required
      ]
    }),

    amountTotal: new FormControl(0, {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.min(0)
      ]
    })
  });

  constructor(
    private readonly customerService: CustomerService
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (!changes['customer']) {
      return;
    }

    this.clearMessages();

    if (this.customer) {
      this.customerForm.patchValue({
        type: this.customer.type,
        firstName: this.customer.firstName,
        surname: this.customer.surname,
        email: this.customer.email,
        cellphone: this.customer.cellphone,
        amountTotal: this.customer.amountTotal
      });

      return;
    }

    this.resetForm();
  }

  submit(): void {
    this.clearMessages();

    if (this.customerForm.invalid) {
      this.customerForm.markAllAsTouched();
      return;
    }

    const request: CustomerRequest = {
      ...this.customerForm.getRawValue()
    };

    this.isSubmitting = true;

    if (this.isEditMode) {
      this.updateCustomer(request);
      return;
    }

    this.createCustomer(request);
  }

  cancel(): void {
    this.isSubmitting = false;
    this.clearMessages();
    this.resetForm();
    this.cancelled.emit();
  }

  private createCustomer(request: CustomerRequest): void {
    this.customerService
      .createCustomer(request)
      .subscribe({
        next: customer => {
          console.log('Customer created:', customer);

          this.successMessage =
            'Customer created successfully.';

          this.isSubmitting = false;

          this.resetForm();
          this.saved.emit();
        },

        error: error => {
          console.error(
            'Failed to create customer:',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Unable to create customer. Please try again.';

          this.isSubmitting = false;
        }
      });
  }

  private updateCustomer(request: CustomerRequest): void {
    if (!this.customer) {
      return;
    }

    this.customerService
      .updateCustomer(this.customer.id, request)
      .subscribe({
        next: customer => {
          console.log('Customer updated:', customer);

          this.successMessage =
            'Customer updated successfully.';

          this.isSubmitting = false;

          this.resetForm();
          this.saved.emit();
        },

        error: error => {
          console.error(
            'Failed to update customer:',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Unable to update customer. Please try again.';

          this.isSubmitting = false;
        }
      });
  }

  private resetForm(): void {
    this.customerForm.reset({
      type: 'Person',
      firstName: '',
      surname: '',
      email: '',
      cellphone: '',
      amountTotal: 0
    });

    this.customerForm.markAsPristine();
    this.customerForm.markAsUntouched();
  }

  private clearMessages(): void {
    this.successMessage = '';
    this.errorMessage = '';
  }

  get isEditMode(): boolean {
    return this.customer !== null;
  }

  get type() {
    return this.customerForm.controls.type;
  }

  get firstName() {
    return this.customerForm.controls.firstName;
  }

  get surname() {
    return this.customerForm.controls.surname;
  }

  get email() {
    return this.customerForm.controls.email;
  }

  get cellphone() {
    return this.customerForm.controls.cellphone;
  }

  get amountTotal() {
    return this.customerForm.controls.amountTotal;
  }
}