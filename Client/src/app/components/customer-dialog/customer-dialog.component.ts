import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { Customer } from '../customer-list/customer-list.component';

@Component({
  selector: 'app-customer-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  templateUrl: './customer-dialog.component.html',
  styleUrls: ['./customer-dialog.component.css']
})
export class CustomerDialogComponent implements OnInit {
  customerForm!: FormGroup;
  isEdit = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<CustomerDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Customer | null
  ) { }

  ngOnInit(): void {
    this.isEdit = !!this.data;
    this.customerForm = this.fb.group({
      name: [this.data?.name || '', [Validators.required]],
      email: [this.data?.email || '', [Validators.email]],
      phone: [this.data?.phone || '', [Validators.required]],
      address: [this.data?.address || ''],
      registrationNumber: [this.data?.registrationNumber || ''],
      loyaltyPoints: [this.data?.loyaltyPoints || 0]
    });
  }

  onSave(): void {
    if (this.customerForm.valid) {
      this.dialogRef.close({
        ...this.data,
        ...this.customerForm.value
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
