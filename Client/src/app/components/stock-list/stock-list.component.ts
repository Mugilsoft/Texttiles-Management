import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ApiService } from '../../services/api.service';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

export interface Stock {
  id: number;
  productId: number;
  productName: string;
  sku: string;
  branchId: number;
  branchName: string;
  quantity: number;
  minStockLevel: number;
}

export interface Branch {
  id: number;
  name: string;
}

@Component({
  selector: 'app-stock-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatSelectModule,
    MatFormFieldModule,
    MatSnackBarModule,
    FormsModule,
    RouterModule
  ],
  templateUrl: './stock-list.component.html',
  styleUrls: ['./stock-list.component.css']
})
export class StockListComponent implements OnInit {
  stocks: Stock[] = [];
  branches: Branch[] = [];
  selectedBranch: number | null = null;
  displayedColumns: string[] = ['sku', 'productName', 'branchName', 'quantity', 'status'];

  constructor(private apiService: ApiService, private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.loadBranches();
    this.loadStock();
  }

  loadBranches(): void {
    this.apiService.get<Branch>('branches').subscribe(data => this.branches = data);
  }

  loadStock(): void {
    let url = 'inventory/stock';
    if (this.selectedBranch) {
      url += `?branchId=${this.selectedBranch}`;
    }

    this.apiService.get<any>(url).subscribe({
      next: (data) => this.stocks = data,
      error: () => this.snackBar.open('Error loading stock', 'Close', { duration: 3000 })
    });
  }

  onBranchChange(): void {
    this.loadStock();
  }

  getStockStatus(element: Stock): string {
    if (element.quantity <= 0) return 'Out of Stock';
    if (element.quantity <= element.minStockLevel) return 'Low Stock';
    return 'In Stock';
  }

  getStatusColor(element: Stock): string {
    if (element.quantity <= 0) return 'warn';
    if (element.quantity <= element.minStockLevel) return 'accent';
    return 'primary';
  }
}
