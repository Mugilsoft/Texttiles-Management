import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { ApiService } from '../../services/api.service';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { CustomerDialogComponent } from '../customer-dialog/customer-dialog.component';

export interface Customer {
  id: number;
  name: string;
  email: string;
  phone: string;
  address: string;
  registrationNumber: string;
  loyaltyPoints: number;
}

@Component({
  selector: 'app-customer-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatSnackBarModule,
    MatDialogModule
  ],
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.css']
})
export class CustomerListComponent implements OnInit {
  customers: Customer[] = [];
  displayedColumns: string[] = ['name', 'phone', 'email', 'registrationNumber', 'loyaltyPoints', 'actions'];

  constructor(
    private apiService: ApiService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadCustomers();
  }

  loadCustomers(): void {
    this.apiService.get<Customer>('customers').subscribe({
      next: (data) => this.customers = data,
      error: (err) => this.snackBar.open('Error loading customers', 'Close', { duration: 3000 })
    });
  }

  openDialog(customer: Customer | null = null): void {
    const dialogRef = this.dialog.open(CustomerDialogComponent, {
      width: '550px',
      data: customer
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (result.id) {
          this.apiService.put('customers', result.id, result).subscribe({
            next: () => {
              this.snackBar.open('Customer updated successfully', 'Close', { duration: 3000 });
              this.loadCustomers();
            },
            error: () => this.snackBar.open('Error updating customer', 'Close', { duration: 3000 })
          });
        } else {
          this.apiService.post('customers', result).subscribe({
            next: () => {
              this.snackBar.open('Customer created successfully', 'Close', { duration: 3000 });
              this.loadCustomers();
            },
            error: () => this.snackBar.open('Error creating customer', 'Close', { duration: 3000 })
          });
        }
      }
    });
  }

  deleteCustomer(id: number): void {
    if (confirm('Are you sure you want to delete this customer?')) {
      this.apiService.delete('customers', id).subscribe({
        next: () => {
          this.snackBar.open('Customer deleted', 'Close', { duration: 3000 });
          this.loadCustomers();
        },
        error: () => this.snackBar.open('Error deleting customer', 'Close', { duration: 3000 })
      });
    }
  }
}
