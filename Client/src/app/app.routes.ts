import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { LayoutComponent } from './components/layout/layout.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { CategoryListComponent } from './components/category-list/category-list.component';
import { ProductListComponent } from './components/product-list/product-list.component';
import { BranchListComponent } from './components/branch-list/branch-list.component';
import { CounterListComponent } from './components/counter-list/counter-list.component';
import { VendorListComponent } from './components/vendor-list/vendor-list.component';
import { CustomerListComponent } from './components/customer-list/customer-list.component';
import { StockListComponent } from './components/stock-list/stock-list.component';
import { StockAdjustmentComponent } from './components/stock-adjustment/stock-adjustment.component';
import { PurchaseListComponent } from './components/purchase-list/purchase-list.component';
import { PurchaseCreateComponent } from './components/purchase-create/purchase-create.component';
import { PosComponent } from './components/pos/pos.component';
import { DayBookComponent } from './components/daybook/day-book.component';
import { ReportsComponent } from './components/reports/reports.component';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },
    {
        path: '',
        component: LayoutComponent,
        canActivate: [AuthGuard],
        children: [
            { path: '', component: DashboardComponent },
            { path: 'dashboard', component: DashboardComponent },
            { path: 'categories', component: CategoryListComponent },
            { path: 'products', component: ProductListComponent },
            { path: 'branches', component: BranchListComponent },
            { path: 'counters', component: CounterListComponent },
            { path: 'vendors', component: VendorListComponent },
            { path: 'customers', component: CustomerListComponent },
            { path: 'inventory', component: StockListComponent },
            { path: 'inventory/adjust', component: StockAdjustmentComponent },
            { path: 'purchases', component: PurchaseListComponent },
            { path: 'purchases/create', component: PurchaseCreateComponent },
            { path: 'pos', component: PosComponent },
            { path: 'daybook', component: DayBookComponent },
            { path: 'reports', component: ReportsComponent },
        ]
    },
    { path: '**', redirectTo: '' }
];
