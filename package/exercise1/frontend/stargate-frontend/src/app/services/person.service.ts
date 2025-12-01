import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponse } from '../shared/base-response';

export interface GetPeopleResult {
    people: PersonAstronaut[];
}

export interface GetPersonByNameResult {
    person?: PersonAstronaut;
}

export interface PersonAstronaut {
    personId: number;
    name: string;
    currentRank?: string | null;
    currentDutyTitle?: string | null;
    careerStartDate?: string | null;
    careerEndDate?: string | null;
}

@Injectable({ providedIn: 'root' })
export class PersonService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiBaseUrl + '/person';

    getPeople(): Observable<ApiResponse<GetPeopleResult>> {
        return this.http.get<ApiResponse<GetPeopleResult>>(this.baseUrl);
    }

    createPerson(name: string): Observable<ApiResponse<void>> {
        return this.http.post<ApiResponse<void>>(this.baseUrl, JSON.stringify(name), {
            headers: { 'Content-Type': 'application/json' }
        });
    }
    
    getPerson(name: string): Observable<ApiResponse<GetPersonByNameResult>> {
        return this.http.get<ApiResponse<GetPersonByNameResult>>(`${this.baseUrl}/${encodeURIComponent(name)}`);
    }
}
