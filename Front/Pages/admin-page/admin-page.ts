import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AdminSidebarComponent } from '../../components/admin-sidebar/admin-sidebar';

@Component({
  selector: 'app-admin-page',
  standalone: true,
  imports: [CommonModule, RouterOutlet, AdminSidebarComponent],
  templateUrl: './admin-page.html',
  styleUrl: './admin-page.scss'
})
export class AdminPage {}