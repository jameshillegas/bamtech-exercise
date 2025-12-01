import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PersonService, PersonAstronaut } from '../services/person.service';
import { BreadcrumbComponent } from '../components/breadcrumb.component';
import { Router, RouterLink } from '@angular/router';
import { ProgressLoaderComponent } from './progress-loader.component';

@Component({
  selector: 'app-people-list',
  standalone: true,
  imports: [CommonModule, FormsModule, BreadcrumbComponent, RouterLink, ProgressLoaderComponent],
  templateUrl: './people-list.component.html'
})
export class PeopleListComponent {
  private svc = inject(PersonService);
  private router = inject(Router);
  people = signal<PersonAstronaut[]>([]);
  error = signal<string | null>(null);
  query = '';
  searchedWithQuery = false;
  loading = signal<boolean>(false);

  ngOnInit() { this.load(); }

  load() {
    this.error.set(null);
    this.loading.set(true);
    this.svc.getPeople().subscribe({
      next: response => {
        this.searchedWithQuery = false;
        if (response.success && response.data?.people) {
          this.people.set(response.data.people);
        } else {
            this.error.set(response.message || 'Failed to load');
        }
        this.loading.set(false);
      },
      error: err => { 
        this.error.set('Failed to load'); 
        this.loading.set(false); 
        console.error(err); 
      }
    });
  }

  routeToDetails(name: string) {
    this.router.navigate(['/astronaut'], { queryParams: { name: encodeURIComponent(name) } });
  } 

  lookupPerson() {
    this.error.set(null);
    const name = this.query.trim();
    if (!name) return;
    this.svc.getPerson(name).subscribe({
      next: res => { 
        this.searchedWithQuery = true;
        if (res.success && res.data?.person) {
          this.people.set([res.data.person]);
        } else {
          this.error.set('Lookup failed');
        }
      },
      error: err => { 
        console.log('err', err);
        this.error.set(err.error?.message || 'Lookup failed'); 
        console.error(err); 
      }
    });
  }
}
