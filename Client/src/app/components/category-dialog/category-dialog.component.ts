import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { Category } from '../category-list/category-list.component';

@Component({
  selector: 'app-category-dialog',
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
  templateUrl: './category-dialog.component.html',
  styleUrls: ['./category-dialog.component.css']
})
export class CategoryDialogComponent implements OnInit {
  categoryForm!: FormGroup;
  isEdit = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<CategoryDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Category | null
  ) { }

  ngOnInit(): void {
    this.isEdit = !!this.data;
    this.categoryForm = this.fb.group({
      name: [this.data?.name || '', [Validators.required, Validators.minLength(2)]],
      description: [this.data?.description || ''],
      isActive: [this.data ? this.data.isActive : true]
    });
  }

  onSave(): void {
    if (this.categoryForm.valid) {
      this.dialogRef.close({
        ...this.data,
        ...this.categoryForm.value
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
