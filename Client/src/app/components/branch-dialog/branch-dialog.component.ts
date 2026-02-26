import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { Branch } from '../branch-list/branch-list.component';

@Component({
  selector: 'app-branch-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSlideToggleModule
  ],
  templateUrl: './branch-dialog.component.html',
  styleUrls: ['./branch-dialog.component.css']
})
export class BranchDialogComponent implements OnInit {
  branchForm!: FormGroup;
  isEdit = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<BranchDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Branch | null
  ) { }

  ngOnInit(): void {
    this.isEdit = !!this.data;
    this.branchForm = this.fb.group({
      name: [this.data?.name || '', [Validators.required]],
      code: [this.data?.code || '', [Validators.required]],
      address: [this.data?.address || ''],
      phone: [this.data?.phone || ''],
      isActive: [this.data ? this.data.isActive : true]
    });
  }

  onSave(): void {
    if (this.branchForm.valid) {
      this.dialogRef.close({
        ...this.data,
        ...this.branchForm.value
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
