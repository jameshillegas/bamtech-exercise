import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layouts/main-layout.component';
import { PeopleListComponent } from './components/people-list.component';
import { AstronautDetailComponent } from './components/astronaut-detail.component';
import { HomePageComponent } from './pages/home-page.component';
import { AddPersonComponent } from './components/add-person.component';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', pathMatch: 'full', component: HomePageComponent },
      { path: 'people', component: PeopleListComponent },
      { path: 'astronaut', component: AstronautDetailComponent },
      { path: 'people/add', component: AddPersonComponent }
    ]
  }
];
