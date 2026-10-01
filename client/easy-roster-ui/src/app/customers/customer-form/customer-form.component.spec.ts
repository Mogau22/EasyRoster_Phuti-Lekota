import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { CustomerFormComponent } from './customer-form.component';

describe('CustomerFormComponent', () => {
  let component: CustomerFormComponent;
  let fixture: ComponentFixture<CustomerFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CustomerFormComponent],
      providers: [
        provideHttpClient()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CustomerFormComponent);
    component = fixture.componentInstance;

    component.customerForm.reset({
      type: 'Person',
      firstName: '',
      surname: '',
      email: '',
      cellphone: '',
      amountTotal: 0
    });
  });

  it('is invalid when required customer details are missing', () => {
    expect(component.customerForm.invalid).toBeTrue();
  });

  it('rejects an invalid email address', () => {
    component.customerForm.patchValue({
      firstName: 'Simple',
      surname: 'Joe',
      email: 'invalid',
      cellphone: '0821234567'
    });

    expect(
      component.customerForm.controls.email.hasError('email')
    ).toBeTrue();
  });

  it('rejects a negative amount', () => {
    component.customerForm.patchValue({
      firstName: 'Simple',
      surname: 'Joe',
      email: 'simple.joe@example.com',
      cellphone: '0821234567',
      amountTotal: -1
    });

    expect(
      component.customerForm.controls.amountTotal.hasError('min')
    ).toBeTrue();
  });

  it('is valid for a complete customer', () => {
    component.customerForm.patchValue({
      firstName: 'Simple',
      surname: 'Joe',
      email: 'simple.joe@example.com',
      cellphone: '0821234567',
      amountTotal: 100
    });

    expect(component.customerForm.valid).toBeTrue();
  });
});