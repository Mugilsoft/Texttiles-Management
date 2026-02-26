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
import { MatTabsModule } from '@angular/material/tabs';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ApiService } from '../../services/api.service';

@Component({
    selector: 'app-reports',
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
        MatTabsModule,
        MatButtonModule,
        MatIconModule
    ],
    templateUrl: './reports.component.html',
    styleUrls: ['./reports.component.css']
})
export class ReportsComponent implements OnInit {
    fromDate: Date = new Date();
    toDate: Date = new Date();
    branches: any[] = [];
    selectedBranch: number | null = null;

    salesReport: any[] = [];
    purchaseReport: any[] = [];

    salesColumns: string[] = ['sku', 'productName', 'quantity', 'revenue', 'tax'];
    purchaseColumns: string[] = ['sku', 'productName', 'quantity', 'cost', 'tax'];

    loading = false;

    constructor(private apiService: ApiService) {
        // Default to last 30 days
        this.fromDate.setDate(this.fromDate.getDate() - 30);
    }

    ngOnInit(): void {
        this.loadBranches();
        this.generateReports();
    }

    loadBranches(): void {
        this.apiService.get<any>('branches').subscribe(data => this.branches = data);
    }

    generateReports(): void {
        this.loading = true;
        const fromStr = this.fromDate.toISOString().split('T')[0];
        const toStr = this.toDate.toISOString().split('T')[0];
        let queryParams = `from=${fromStr}&to=${toStr}`;
        if (this.selectedBranch) {
            queryParams += `&branchId=${this.selectedBranch}`;
        }

        this.apiService.get<any>(`reports/sales-report?${queryParams}`).subscribe(data => {
            this.salesReport = data;
        });

        this.apiService.get<any>(`reports/purchase-report?${queryParams}`).subscribe(data => {
            this.purchaseReport = data;
            this.loading = false;
        });
    }
}
