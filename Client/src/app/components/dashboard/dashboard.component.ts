import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, MatGridListModule, MatCardModule, MatIconModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  stats = [
    { title: 'Today\'s Sales', value: '₹ 0.00', icon: 'payments', color: '#4caf50' },
    { title: 'Total Products', value: '0', icon: 'inventory_2', color: '#2196f3' },
    { title: 'Pending Orders', value: '0', icon: 'shopping_cart', color: '#ff9800' },
    { title: 'Low Stock Items', value: '0', icon: 'warning', color: '#f44336' }
  ];

  constructor(private apiService: ApiService) { }

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    this.apiService.get<any>('products').subscribe(p => {
      this.stats[1].value = p.length.toString();
    });
    this.apiService.get<any>('categories').subscribe(c => {
      // Just placeholder for now
    });
  }
}
