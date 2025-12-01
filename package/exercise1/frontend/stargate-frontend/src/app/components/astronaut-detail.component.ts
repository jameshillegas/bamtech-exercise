import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AstronautDutyService, GetAstronautByNameResult } from '../services/astronaut-duty.service';
import { BreadcrumbComponent } from '../components/breadcrumb.component';
import { ActivatedRoute } from '@angular/router';
import { ProgressLoaderComponent } from './progress-loader.component';

@Component({
  selector: 'app-astronaut-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe, BreadcrumbComponent, ProgressLoaderComponent],
  templateUrl: './astronaut-detail.component.html'
})
export class AstronautDetailComponent implements OnInit {
  private svc = inject(AstronautDutyService);
  private route = inject(ActivatedRoute);
  query = '';
  data = signal<GetAstronautByNameResult | null>(null);
  error = signal<string | null>(null);
  loading = signal<boolean>(false);

  ngOnInit(): void {
    this.route.queryParamMap.subscribe(params => {
      const nameParam = params.get('name');
      if (nameParam) {
        this.query = decodeURIComponent(nameParam);
        this.lookup();
      }
    });
  }

  localDateString(dateStr?: string, defaultValue: string = 'Unknown'): string {
    if (!dateStr) return defaultValue;
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-US', { dateStyle: 'short' });
  }

  lookup() {
    this.error.set(null);
    this.data.set(null);
    const name = this.query.trim();
    if (!name) return;
    this.loading.set(true);
    this.svc.getByName(name).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.data.set(res.data);
        } else {
          this.error.set('Lookup failed');
        }
        this.loading.set(false);
      },
      error: err => {
        this.loading.set(false);
        this.error.set(err.error?.message || 'Lookup failed');
        console.error(err);
      }
    });
  }
} 