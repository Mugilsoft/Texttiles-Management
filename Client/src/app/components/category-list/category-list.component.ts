import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { ApiService } from '../../services/api.service';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { CategoryDialogComponent } from '../category-dialog/category-dialog.component';

export interface Category {
  id: number;
  name: string;
  description: string;
  isActive: boolean;
}

@Component({
  selector: 'app-category-list',
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
  templateUrl: './category-list.component.html',
  styleUrls: ['./category-list.component.css']
})
export class CategoryListComponent implements OnInit {
  categories: Category[] = [];
  displayedColumns: string[] = ['id', 'name', 'description', 'isActive', 'actions'];

  constructor(
    private apiService: ApiService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.apiService.get<Category>('categories').subscribe({
      next: (data) => this.categories = data,
      error: (err) => this.snackBar.open('Error loading categories', 'Close', { duration: 3000 })
    });
  }

  openDialog(category: Category | null = null): void {
    const dialogRef = this.dialog.open(CategoryDialogComponent, {
      width: '500px',
      data: category
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (result.id) {
          // Update
          this.apiService.put('categories', result.id, result).subscribe({
            next: () => {
              this.snackBar.open('Category updated successfully', 'Close', { duration: 3000 });
              this.loadCategories();
            },
            error: () => this.snackBar.open('Error updating category', 'Close', { duration: 3000 })
          });
        } else {
          // Create
          this.apiService.post('categories', result).subscribe({
            next: () => {
              this.snackBar.open('Category created successfully', 'Close', { duration: 3000 });
              this.loadCategories();
            },
            error: () => this.snackBar.open('Error creating category', 'Close', { duration: 3000 })
          });
        }
      }
    });
  }

  deleteCategory(id: number): void {
    if (confirm('Are you sure you want to delete this category?')) {
      this.apiService.delete('categories', id).subscribe({
        next: () => {
          this.snackBar.open('Category deleted', 'Close', { duration: 3000 });
          this.loadCategories();
        },
        error: () => this.snackBar.open('Error deleting category', 'Close', { duration: 3000 })
      });
    }
  }
}
