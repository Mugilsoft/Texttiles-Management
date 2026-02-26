import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { ApiService } from '../../services/api.service';
import { Product, Category } from '../../models/product.model';

@Component({
    selector: 'app-product-dialog',
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
    templateUrl: './product-dialog.component.html',
    styleUrls: ['./product-dialog.component.css']
})
export class ProductDialogComponent implements OnInit {
    productForm!: FormGroup;
    isEdit = false;
    categories: Category[] = [];

    constructor(
        private fb: FormBuilder,
        private apiService: ApiService,
        private dialogRef: MatDialogRef<ProductDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: Product | null
    ) { }

    ngOnInit(): void {
        this.isEdit = !!this.data;
        this.initForm();
        this.loadCategories();
    }

    // private initForm(): void {
    //     this.productForm = this.fb.group({
    //         name: [this.data?.name || '', [Validators.required]],
    //         sku: [this.data?.sku || '', [Validators.required]],
    //         barcode: [this.data?.barcode || ''],
    //         categoryId: [this.data?.categoryId || '', [Validators.required]],
    //         size: [this.data?.size || ''],
    //         color: [this.data?.color || ''],
    //         fabric: [this.data?.fabric || ''],
    //         purchasePrice: [this.data?.purchasePrice || 0, [Validators.required, Validators.min(0)]],
    //         salePrice: [this.data?.salePrice || 0, [Validators.required, Validators.min(0)]],
    //         mrp: [this.data?.mrp || 0, [Validators.required, Validators.min(0)]],
    //         taxPercentage: [this.data?.taxPercentage || 0, [Validators.required, Validators.min(0)]],
    //         isActive: [this.data ? this.data.isActive : true]
    //     });
    // }
    private initForm(): void {
  this.productForm = this.fb.group({
    name: [this.data?.name ?? '', Validators.required],
    sku: [this.data?.sku ?? '', Validators.required],
    barcode: [this.data?.barcode ?? ''],
    categoryId: [this.data?.categoryId ?? null, Validators.required],
    size: [this.data?.size ?? ''],
    color: [this.data?.color ?? ''],
    fabric: [this.data?.fabric ?? ''],
    purchasePrice: [this.data?.purchasePrice ?? 0, [Validators.required, Validators.min(0)]],
    salePrice: [this.data?.salePrice ?? 0, [Validators.required, Validators.min(0)]],
    mrp: [this.data?.mrp ?? 0, [Validators.required, Validators.min(0)]],
    taxPercentage: [this.data?.taxPercentage ?? 0, [Validators.required, Validators.min(0)]],
    isActive: [this.data?.isActive ?? true]
  });
}

    private loadCategories(): void {
        this.apiService.get<Category>('categories').subscribe({
            next: (data) => this.categories = data,
            error: (err) => console.error('Error loading categories', err)
        });
    }

    CreateProduct(): void {
        if (this.productForm.valid) {
            this.dialogRef.close({
                ...this.data,
                ...this.productForm.value
            });
        }
    }

    onCancel(): void {
        this.dialogRef.close();
    }
}
