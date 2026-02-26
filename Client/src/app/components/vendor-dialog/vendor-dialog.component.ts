import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { Vendor } from '../vendor-list/vendor-list.component';

@Component({
  selector: 'app-vendor-dialog',
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
  templateUrl: './vendor-dialog.component.html',
  styleUrls: ['./vendor-dialog.component.css']
})
export class VendorDialogComponent implements OnInit {
  vendorForm!: FormGroup;
  isEdit = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<VendorDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Vendor | null
  ) { }

  ngOnInit(): void {
    this.isEdit = !!this.data;
    this.vendorForm = this.fb.group({
      name: [this.data?.name || '', [Validators.required]],
      contactPerson: [this.data?.contactPerson || ''],
      email: [this.data?.email || '', [Validators.email]],
      phone: [this.data?.phone || ''],
      gstNumber: [this.data?.gstNumber || ''],
      isActive: [this.data ? this.data.isActive : true]
    });
  }

  onSave(): void {
    if (this.vendorForm.valid) {
      this.dialogRef.close({
        ...this.data,
        ...this.vendorForm.value
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
