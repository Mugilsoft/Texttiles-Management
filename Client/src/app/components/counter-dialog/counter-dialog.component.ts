import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { Counter } from '../counter-list/counter-list.component';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-counter-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatSlideToggleModule
  ],
  templateUrl: './counter-dialog.component.html',
  styleUrls: ['./counter-dialog.component.css']
})
export class CounterDialogComponent implements OnInit {
  counterForm!: FormGroup;
  branches: any[] = [];
  isEdit = false;

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService,
    private dialogRef: MatDialogRef<CounterDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Counter | null
  ) { }

  ngOnInit(): void {
    this.isEdit = !!this.data;
    this.counterForm = this.fb.group({
      name: [this.data?.name || '', [Validators.required]],
      code: [this.data?.code || '', [Validators.required]],
      branchId: [this.data?.branchId || '', [Validators.required]],
      isActive: [this.data ? this.data.isActive : true]
    });

    this.loadBranches();
  }

  loadBranches(): void {
    this.apiService.get<any>('branches').subscribe(data => this.branches = data);
  }

  onSave(): void {
    if (this.counterForm.valid) {
      this.dialogRef.close({
        ...this.data,
        ...this.counterForm.value
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
