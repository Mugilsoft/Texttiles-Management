import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { ApiService } from '../../services/api.service';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { VendorDialogComponent } from '../vendor-dialog/vendor-dialog.component';

export interface Vendor {
  id: number;
  name: string;
  contactPerson: string;
  email: string;
  phone: string;
  gstNumber: string;
  isActive: boolean;
}

@Component({
  selector: 'app-vendor-list',
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
  templateUrl: './vendor-list.component.html',
  styleUrls: ['./vendor-list.component.css']
})
export class VendorListComponent implements OnInit {
  vendors: Vendor[] = [];
  displayedColumns: string[] = ['name', 'contactPerson', 'phone', 'gstNumber', 'isActive', 'actions'];

  constructor(
    private apiService: ApiService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadVendors();
  }

  loadVendors(): void {
    this.apiService.get<Vendor>('vendors').subscribe({
      next: (data) => this.vendors = data,
      error: (err) => this.snackBar.open('Error loading vendors', 'Close', { duration: 3000 })
    });
  }

  openDialog(vendor: Vendor | null = null): void {
    const dialogRef = this.dialog.open(VendorDialogComponent, {
      width: '550px',
      data: vendor
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (result.id) {
          this.apiService.put('vendors', result.id, result).subscribe({
            next: () => {
              this.snackBar.open('Vendor updated successfully', 'Close', { duration: 3000 });
              this.loadVendors();
            },
            error: () => this.snackBar.open('Error updating vendor', 'Close', { duration: 3000 })
          });
        } else {
          this.apiService.post('vendors', result).subscribe({
            next: () => {
              this.snackBar.open('Vendor created successfully', 'Close', { duration: 3000 });
              this.loadVendors();
            },
            error: () => this.snackBar.open('Error creating vendor', 'Close', { duration: 3000 })
          });
        }
      }
    });
  }

  deleteVendor(id: number): void {
    if (confirm('Are you sure you want to delete this vendor?')) {
      this.apiService.delete('vendors', id).subscribe({
        next: () => {
          this.snackBar.open('Vendor deleted', 'Close', { duration: 3000 });
          this.loadVendors();
        },
        error: () => this.snackBar.open('Error deleting vendor', 'Close', { duration: 3000 })
      });
    }
  }
}
