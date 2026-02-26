import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormControl } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatSelectModule } from '@angular/material/select';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { ApiService } from '../../services/api.service';
import { Product } from '../../models/product.model';
import { Observable, startWith, map, tap } from 'rxjs';

interface CartItem {
    productId: number;
    productName: string;
    sku: string;
    quantity: number;
    unitPrice: number;
    taxPercentage: number;
    taxAmount: number;
    discount: number;
    subTotal: number;
}

@Component({
    selector: 'app-pos',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        MatCardModule,
        MatFormFieldModule,
        MatInputModule,
        MatButtonModule,
        MatIconModule,
        MatTableModule,
        MatAutocompleteModule,
        MatSelectModule,
        MatDividerModule,
        MatSnackBarModule
    ],
    templateUrl: './pos.component.html',
    styleUrls: ['./pos.component.css']
})
export class PosComponent implements OnInit {
    searchControl = new FormControl('');
    products: Product[] = [];
    filteredProducts!: Observable<Product[]>;
    cart: CartItem[] = [];
    displayedColumns: string[] = ['productName', 'quantity', 'unitPrice', 'tax', 'subTotal', 'actions'];

    summary = {
        totalAmount: 0,
        totalTax: 0,
        discount: 0,
        netAmount: 0
    };

    paymentMode = 'Cash';
    paymentModes = ['Cash', 'Card', 'UPI', 'Wallet', 'Credit'];

    loading = false;

    constructor(private apiService: ApiService, private snackBar: MatSnackBar) { }

    ngOnInit(): void {
        this.loadProducts();
        this.filteredProducts = this.searchControl.valueChanges.pipe(
            startWith(''),
            map(value => typeof value === 'string' ? value : ''),
            tap(value => {
                if (value && value.length >= 3) {
                    const exactMatch = this.products.find(p =>
                        p.barcode === value || p.sku.toLowerCase() === value.toLowerCase()
                    );
                    if (exactMatch) {
                        this.onProductSelect(exactMatch);
                    }
                }
            }),
            map(name => name ? this._filter(name) : this.products.slice())
        );
    }

    private loadProducts(): void {
        this.apiService.get<Product>('products').subscribe(data => this.products = data);
    }

    displayFn(product: Product): string {
        return product ? `${product.name} (${product.sku})` : '';
    }

    private _filter(name: string): Product[] {
        const filterValue = name.toLowerCase();
        return this.products.filter(p =>
            p.name.toLowerCase().includes(filterValue) ||
            p.sku.toLowerCase().includes(filterValue) ||
            (p.barcode && p.barcode.includes(filterValue))
        );
    }

    onProductSelect(product: Product): void {
        const existing = this.cart.find(item => item.productId === product.id);
        if (existing) {
            existing.quantity++;
            this.updateItemCalculations(existing);
        } else {
            const newItem: CartItem = {
                productId: product.id,
                productName: product.name,
                sku: product.sku,
                quantity: 1,
                unitPrice: product.salePrice,
                taxPercentage: product.taxPercentage,
                taxAmount: 0,
                discount: 0,
                subTotal: 0
            };
            this.updateItemCalculations(newItem);
            this.cart = [...this.cart, newItem];
        }
        this.searchControl.setValue('');
        this.calculateSummary();
    }

    updateItemCalculations(item: CartItem): void {
        const baseAmount = item.quantity * item.unitPrice;
        item.taxAmount = (baseAmount * item.taxPercentage) / 100;
        item.subTotal = baseAmount + item.taxAmount - item.discount;
        this.calculateSummary();
    }

    removeItem(index: number): void {
        this.cart.splice(index, 1);
        this.cart = [...this.cart];
        this.calculateSummary();
    }

    calculateSummary(): void {
        this.summary.totalAmount = this.cart.reduce((sum, item) => sum + (item.quantity * item.unitPrice), 0);
        this.summary.totalTax = this.cart.reduce((sum, item) => sum + item.taxAmount, 0);
        this.summary.netAmount = this.cart.reduce((sum, item) => sum + item.subTotal, 0) - this.summary.discount;
    }

    checkout(): void {
        if (this.cart.length === 0) {
            this.snackBar.open('Cart is empty', 'Close', { duration: 3000 });
            return;
        }

        this.loading = true;
        const saleData = {
            branchId: 1, // Defaulting to 1 for now, should come from auth/session
            counterId: 1,
            paymentMode: this.paymentMode,
            totalAmount: this.summary.totalAmount,
            totalTax: this.summary.totalTax,
            discount: this.summary.discount,
            netAmount: this.summary.netAmount,
            items: this.cart
        };

        this.apiService.post('sales', saleData).subscribe({
            next: () => {
                this.snackBar.open('Sale completed successfully', 'Close', { duration: 3000 });
                this.cart = [];
                this.calculateSummary();
                this.loading = false;
            },
            error: () => {
                this.snackBar.open('Error completing sale', 'Close', { duration: 3000 });
                this.loading = false;
            }
        });
    }
}
