import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-purchase-create',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSnackBarModule
  ],
  templateUrl: './purchase-create.component.html',
  styleUrls: ['./purchase-create.component.css']
})
export class PurchaseCreateComponent implements OnInit {
  purchaseForm!: FormGroup;
  vendors: any[] = [];
  branches: any[] = [];
  products: any[] = [];
  loading = false;

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService,
    private snackBar: MatSnackBar,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.purchaseForm = this.fb.group({
      vendorId: ['', Validators.required],
      branchId: ['', Validators.required],
      poDate: [new Date(), Validators.required],
      remarks: [''],
      items: this.fb.array([])
    });

    this.loadDropdowns();
    this.addItem(); // Start with one empty item
  }

  get items() {
    return this.purchaseForm.get('items') as FormArray;
  }

  loadDropdowns(): void {
    this.apiService.get<any>('vendors').subscribe(data => this.vendors = data);
    this.apiService.get<any>('branches').subscribe(data => this.branches = data);
    this.apiService.get<any>('products').subscribe(data => this.products = data);
  }

  addItem(): void {
    const itemForm = this.fb.group({
      productId: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitPrice: [0, [Validators.required, Validators.min(0)]],
      taxPercentage: [0],
      taxAmount: [0],
      subTotal: [0]
    });

    this.items.push(itemForm);
  }

  removeItem(index: number): void {
    this.items.removeAt(index);
  }

  calculateItem(index: number): void {
    const item = this.items.at(index);
    const qty = item.get('quantity')?.value || 0;
    const price = item.get('unitPrice')?.value || 0;
    const taxP = item.get('taxPercentage')?.value || 0;

    const baseAmount = qty * price;
    const taxAmount = (baseAmount * taxP) / 100;
    const subTotal = baseAmount + taxAmount;

    item.patchValue({
      taxAmount: taxAmount,
      subTotal: subTotal
    }, { emitEvent: false });
  }

  getTotal(): number {
    return this.items.controls.reduce((acc, curr) => acc + (curr.get('subTotal')?.value || 0), 0);
  }

  onSubmit(): void {
    if (this.purchaseForm.invalid || this.items.length === 0) {
      this.snackBar.open('Please fill all required fields and add at least one item', 'Close', { duration: 3000 });
      return;
    }

    this.loading = true;
    this.apiService.post('purchases', this.purchaseForm.value).subscribe({
      next: () => {
        this.snackBar.open('Purchase Order created successfully', 'Close', { duration: 3000 });
        this.router.navigate(['/purchases']);
      },
      error: () => {
        this.snackBar.open('Error creating purchase order', 'Close', { duration: 3000 });
        this.loading = false;
      }
    });
  }
}
