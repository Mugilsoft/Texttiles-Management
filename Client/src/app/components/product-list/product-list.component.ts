import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { ApiService } from '../../services/api.service';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';

import { Product } from '../../models/product.model';

import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ProductDialogComponent } from '../product-dialog/product-dialog.component';

@Component({
  selector: 'app-product-list',
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
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  displayedColumns: string[] = ['sku', 'name', 'categoryName', 'salePrice', 'isActive', 'actions'];

  constructor(
    private apiService: ApiService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.apiService.get<Product>('products').subscribe({
      next: (data) => this.products = data,
      error: (err) => this.snackBar.open('Error loading products', 'Close', { duration: 3000 })
    });
  }

  addProduct(): void {
    const dialogRef = this.dialog.open(ProductDialogComponent, {
      width: '800px',
      data: null,
      autoFocus: 'first-tabbable',
      restoreFocus: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.apiService.post('products', result).subscribe({
          next: () => {
            this.snackBar.open('Product added successfully', 'Close', { duration: 3000 });
            this.loadProducts();
          },
          error: () => this.snackBar.open('Error adding product', 'Close', { duration: 3000 })
        });
      }
    });
  }

  editProduct(product: Product): void {
    const dialogRef = this.dialog.open(ProductDialogComponent, {
      width: '800px',
      data: product,
      autoFocus: 'first-tabbable',
      restoreFocus: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.apiService.put('products', product.id, result).subscribe({
          next: () => {
            this.snackBar.open('Product updated successfully', 'Close', { duration: 3000 });
            this.loadProducts();
          },
          error: () => this.snackBar.open('Error updating product', 'Close', { duration: 3000 })
        });
      }
    });
  }

  deleteProduct(id: number): void {
    if (confirm('Are you sure you want to delete this product?')) {
      this.apiService.delete('products', id).subscribe({
        next: () => {
          this.snackBar.open('Product deleted', 'Close', { duration: 3000 });
          this.loadProducts();
        },
        error: () => this.snackBar.open('Error deleting product', 'Close', { duration: 3000 })
      });
    }
  }
}
