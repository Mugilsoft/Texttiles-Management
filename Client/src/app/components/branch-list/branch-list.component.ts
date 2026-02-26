import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { ApiService } from '../../services/api.service';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { BranchDialogComponent } from '../branch-dialog/branch-dialog.component';

export interface Branch {
  id: number;
  name: string;
  code: string;
  address: string;
  phone: string;
  isActive: boolean;
}

@Component({
  selector: 'app-branch-list',
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
  templateUrl: './branch-list.component.html',
  styleUrls: ['./branch-list.component.css']
})
export class BranchListComponent implements OnInit {
  branches: Branch[] = [];
  displayedColumns: string[] = ['code', 'name', 'phone', 'isActive', 'actions'];

  constructor(
    private apiService: ApiService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadBranches();
  }

  loadBranches(): void {
    this.apiService.get<Branch>('branches').subscribe({
      next: (data) => this.branches = data,
      error: (err) => this.snackBar.open('Error loading branches', 'Close', { duration: 3000 })
    });
  }

  openDialog(branch: Branch | null = null): void {
    const dialogRef = this.dialog.open(BranchDialogComponent, {
      width: '550px',
      data: branch
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (result.id) {
          this.apiService.put('branches', result.id, result).subscribe({
            next: () => {
              this.snackBar.open('Branch updated successfully', 'Close', { duration: 3000 });
              this.loadBranches();
            },
            error: () => this.snackBar.open('Error updating branch', 'Close', { duration: 3000 })
          });
        } else {
          this.apiService.post('branches', result).subscribe({
            next: () => {
              this.snackBar.open('Branch created successfully', 'Close', { duration: 3000 });
              this.loadBranches();
            },
            error: () => this.snackBar.open('Error creating branch', 'Close', { duration: 3000 })
          });
        }
      }
    });
  }

  deleteBranch(id: number): void {
    if (confirm('Are you sure you want to delete this branch?')) {
      this.apiService.delete('branches', id).subscribe({
        next: () => {
          this.snackBar.open('Branch deleted', 'Close', { duration: 3000 });
          this.loadBranches();
        },
        error: () => this.snackBar.open('Error deleting branch', 'Close', { duration: 3000 })
      });
    }
  }
}
