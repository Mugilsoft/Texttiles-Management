import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';

export interface PurchaseOrder {
  id: number;
  poNumber: string;
  poDate: string;
  vendorName: string;
  branchName: string;
  netAmount: number;
  status: string;
}

@Component({
  selector: 'app-purchase-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatChipsModule,
    MatTooltipModule,
    MatSnackBarModule,
    RouterModule
  ],
  templateUrl: './purchase-list.component.html',
  styleUrls: ['./purchase-list.component.css']
})
export class PurchaseListComponent implements OnInit {
  purchases: PurchaseOrder[] = [];
  displayedColumns: string[] = ['poNumber', 'poDate', 'vendorName', 'branchName', 'netAmount', 'status', 'actions'];

  constructor(private apiService: ApiService, private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.loadPurchases();
  }

  loadPurchases(): void {
    this.apiService.get<PurchaseOrder>('purchases').subscribe({
      next: (data) => this.purchases = data,
      error: () => this.snackBar.open('Error loading purchases', 'Close', { duration: 3000 })
    });
  }

  receiveOrder(id: number): void {
    if (confirm('Mark this order as received? This will update your stock inventory.')) {
      this.apiService.post(`purchases/${id}/receive`, {}).subscribe({
        next: () => {
          this.snackBar.open('Order received and stock updated', 'Close', { duration: 3000 });
          this.loadPurchases();
        },
        error: (err) => this.snackBar.open(err.error?.message || 'Error receiving order', 'Close', { duration: 3000 })
      });
    }
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Received': return 'primary';
      case 'Pending': return 'accent';
      case 'Cancelled': return 'warn';
      default: return '';
    }
  }
}
