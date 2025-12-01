import { Component, inject, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

interface Crumb { label: string; url: string; }

@Component({
  selector: 'app-breadcrumb',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.css']
})
export class BreadcrumbComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  @Input() items: Crumb[] | null = null;
  crumbs: Crumb[] = [];

  ngOnInit() {
    this.buildCrumbs();
    this.router.events.subscribe(() => this.buildCrumbs());
  }

  private buildCrumbs() {
    if (this.items && this.items.length) {
      this.crumbs = this.items;
      return;
    }
    const crumbs: Crumb[] = [];
    let url = '';
    let current = this.route.root;
    while (current) {
      const snapshot = current.snapshot;
      const routeUrl = snapshot.url.map(u => u.path).join('/');
      if (routeUrl) {
        url += `/${routeUrl}`;
        const label = snapshot.routeConfig?.data?.['breadcrumb'] || routeUrl;
        crumbs.push({ label, url });
      }
      current = (current.firstChild as ActivatedRoute) || null as any;
    }
    this.crumbs = crumbs;
  }
}
