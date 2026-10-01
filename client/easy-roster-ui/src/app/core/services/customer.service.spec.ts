import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting
} from '@angular/common/http/testing';
import { CustomerService } from './customer.service';

const request = {
  type: 'Person',
  firstName: 'Simple',
  surname: 'Joe',
  email: 'simple.joe@example.com',
  cellphone: '0821234567',
  amountTotal: 0
};

const apiUrl = 'http://localhost:5000/api/customers';

describe('CustomerService', () => {
  let service: CustomerService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        CustomerService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(CustomerService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('passes the search term to the API', () => {
    service.getCustomers('simple').subscribe();

    const req = http.expectOne(`${apiUrl}?search=simple`);

    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('posts a customer', () => {
    service.createCustomer(request).subscribe();

    const req = http.expectOne(apiUrl);

    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(request);

    req.flush({ id: 1, ...request });
  });

  it('updates a customer', () => {
    service.updateCustomer(1, request).subscribe();

    const req = http.expectOne(`${apiUrl}/1`);

    expect(req.request.method).toBe('PUT');
    req.flush(null);
  });

  it('deletes a customer', () => {
    service.deleteCustomer(1).subscribe();

    const req = http.expectOne(`${apiUrl}/1`);

    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});