import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PersonService } from '../services/person.service';
import { BreadcrumbComponent } from '../components/breadcrumb.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-person',
  standalone: true,
  imports: [CommonModule, FormsModule, BreadcrumbComponent],
  templateUrl: './add-person.component.html'
})
export class AddPersonComponent {
  private svc = inject(PersonService);
  private router = inject(Router);
  error = signal<string | null>(null);
  name = ''

  addPerson() {
    this.error.set(null);
    const name = this.name.trim();
    if (!name) return;
    this.svc.createPerson(name).subscribe({
      next: res => { 
        if (res.success) {
          this.name = '';
          this.router.navigate(['/people']);
        } else {
          this.error.set(res.message || 'Add person failed');
        }
      },
      error: err => { 
        this.error.set(err.error?.message || 'Add person failed'); 
        console.error(err); 
      }
    });
  }
}
