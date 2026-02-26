import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { Observable, startWith, map, of, switchMap } from 'rxjs';
import { Product } from '../../models/product.model';

export interface Branch {
  id: number;
  name: string;
}

@Component({
  selector: 'app-stock-adjustment',
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
    MatSnackBarModule,
    MatIconModule,
    MatAutocompleteModule,
    RouterModule
  ],
  templateUrl: './stock-adjustment.component.html',
  styleUrls: ['./stock-adjustment.component.css']
})
export class StockAdjustmentComponent implements OnInit {
  adjustmentForm!: FormGroup;
  searchControl = new FormControl('');
  products: Product[] = [];
  filteredProducts!: Observable<Product[]>;
  branches: Branch[] = [];
  currentStock: number | null = null;
  loading = false;
  fetchingStock = false;

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService,
    private snackBar: MatSnackBar,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.adjustmentForm = this.fb.group({
      productId: ['', Validators.required],
      branchId: ['', Validators.required],
      quantity: ['', [Validators.required]],
      referenceNumber: [''],
      remarks: ['']
    });

    this.loadDropdowns();
    this.setupProductSearch();
    this.setupStockListener();
  }

  private setupProductSearch(): void {
    this.filteredProducts = this.searchControl.valueChanges.pipe(
      startWith(''),
      map(value => typeof value === 'string' ? value : ''),
      map(name => name ? this._filter(name) : this.products.slice())
    );
  }

  private setupStockListener(): void {
    this.adjustmentForm.get('branchId')?.valueChanges.subscribe(() => this.fetchCurrentStock());
    this.adjustmentForm.get('productId')?.valueChanges.subscribe(() => this.fetchCurrentStock());
  }

  private _filter(name: string): Product[] {
    const filterValue = name.toLowerCase();
    return this.products.filter(option =>
      option.name.toLowerCase().includes(filterValue) ||
      option.sku.toLowerCase().includes(filterValue)
    );
  }

  displayFn(product: Product): string {
    return product ? `${product.name} (${product.sku})` : '';
  }

  onProductSelected(product: Product): void {
    this.adjustmentForm.patchValue({ productId: product.id });
  }

  fetchCurrentStock(): void {
    const productId = this.adjustmentForm.get('productId')?.value;
    const branchId = this.adjustmentForm.get('branchId')?.value;

    if (productId && branchId) {
      this.fetchingStock = true;
      this.apiService.get<any>(`inventory/stock?branchId=${branchId}`).subscribe({
        next: (stocks) => {
          const item = stocks.find((s: any) => s.productId === productId);
          this.currentStock = item ? item.quantity : 0;
          this.fetchingStock = false;
        },
        error: () => {
          this.currentStock = null;
          this.fetchingStock = false;
        }
      });
    } else {
      this.currentStock = null;
    }
  }

  loadDropdowns(): void {
    this.apiService.get<Product>('products').subscribe(data => {
      this.products = data;
      this.searchControl.setValue(''); // Trigger initial filter
    });
    this.apiService.get<Branch>('branches').subscribe(data => this.branches = data);
  }

  onSubmit(): void {
    if (this.adjustmentForm.invalid) return;

    this.loading = true;
    this.apiService.post('inventory/adjust', this.adjustmentForm.value).subscribe({
      next: () => {
        this.snackBar.open('Stock adjusted successfully', 'Close', { duration: 3000 });
        this.router.navigate(['/inventory']);
      },
      error: () => {
        this.snackBar.open('Error adjusting stock', 'Close', { duration: 3000 });
        this.loading = false;
      }
    });
  }
}
