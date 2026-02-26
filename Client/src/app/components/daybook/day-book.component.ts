import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatDividerModule } from '@angular/material/divider';
import { ApiService } from '../../services/api.service';

interface DayBook {
    date: string;
    totalSales: number;
    totalPurchases: number;
    totalTaxCollected: number;
    totalTaxPaid: number;
    netCashFlow: number;
    salesByPaymentMode: { paymentMode: string; amount: number }[];
}

@Component({
    selector: 'app-day-book',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        MatCardModule,
        MatDatepickerModule,
        MatFormFieldModule,
        MatInputModule,
        MatNativeDateModule,
        MatSelectModule,
        MatTableModule,
        MatDividerModule
    ],
    templateUrl: './day-book.component.html',
    styleUrls: ['./day-book.component.css']
})
export class DayBookComponent implements OnInit {
    selectedDate: Date = new Date();
    dayBook: DayBook | null = null;
    branches: any[] = [];
    selectedBranch: number | null = null;
    loading = false;

    constructor(private apiService: ApiService) { }

    ngOnInit(): void {
        this.loadBranches();
        this.loadDayBook();
    }

    loadBranches(): void {
        this.apiService.get<any>('branches').subscribe(data => this.branches = data);
    }

    loadDayBook(): void {
        this.loading = true;
        const dateStr = this.selectedDate.toISOString().split('T')[0];
        let url = `reports/daybook?date=${dateStr}`;
        if (this.selectedBranch) {
            url += `&branchId=${this.selectedBranch}`;
        }

        this.apiService.getSingle<any>(url).subscribe({
            next: (data: any) => {
                this.dayBook = data;
                this.loading = false;
            },
            error: () => {
                this.loading = false;
            }
        });
    }

    onFilterChange(): void {
        this.loadDayBook();
    }
}
