import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { ApiService } from '../../services/api.service';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { CounterDialogComponent } from '../counter-dialog/counter-dialog.component';

export interface Counter {
  id: number;
  name: string;
  code: string;
  branchId: number;
  branchName: string;
  isActive: boolean;
}

@Component({
  selector: 'app-counter-list',
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
  templateUrl: './counter-list.component.html',
  styleUrls: ['./counter-list.component.css']
})
export class CounterListComponent implements OnInit {
  counters: Counter[] = [];
  displayedColumns: string[] = ['code', 'name', 'branchName', 'isActive', 'actions'];

  constructor(
    private apiService: ApiService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadCounters();
  }

  loadCounters(): void {
    this.apiService.get<Counter>('counters').subscribe({
      next: (data) => this.counters = data,
      error: (err) => this.snackBar.open('Error loading counters', 'Close', { duration: 3000 })
    });
  }

  openDialog(counter: Counter | null = null): void {
    const dialogRef = this.dialog.open(CounterDialogComponent, {
      width: '500px',
      data: counter
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (result.id) {
          this.apiService.put('counters', result.id, result).subscribe({
            next: () => {
              this.snackBar.open('Counter updated successfully', 'Close', { duration: 3000 });
              this.loadCounters();
            },
            error: () => this.snackBar.open('Error updating counter', 'Close', { duration: 3000 })
          });
        } else {
          this.apiService.post('counters', result).subscribe({
            next: () => {
              this.snackBar.open('Counter created successfully', 'Close', { duration: 3000 });
              this.loadCounters();
            },
            error: () => this.snackBar.open('Error creating counter', 'Close', { duration: 3000 })
          });
        }
      }
    });
  }

  deleteCounter(id: number): void {
    if (confirm('Are you sure you want to delete this counter?')) {
      this.apiService.delete('counters', id).subscribe({
        next: () => {
          this.snackBar.open('Counter deleted', 'Close', { duration: 3000 });
          this.loadCounters();
        },
        error: () => this.snackBar.open('Error deleting counter', 'Close', { duration: 3000 })
      });
    }
  }
}
