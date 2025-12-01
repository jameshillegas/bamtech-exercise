import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponse } from '../shared/base-response';

export interface AstronautDuty {
  id: number;
  personId: number;
  dutyStartDate: string;
  dutyEndDate?: string;
  dutyTitle: string;
  rank: string;
}

export interface AstronautPerson {
  personId: number;
  name: string;
  currentRank?: string;
  currentDutyTitle?: string;
  careerStartDate?: string;
  careerEndDate?: string;
}

export interface GetAstronautByNameResult {  
  person?: AstronautPerson;
  astronautDuties: AstronautDuty[];
}

@Injectable({ providedIn: 'root' })
export class AstronautDutyService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiBaseUrl + '/AstronautDuty';

  getByName(name: string): Observable<ApiResponse<GetAstronautByNameResult>> {
    return this.http.get<ApiResponse<GetAstronautByNameResult>>(`${this.baseUrl}/${encodeURIComponent(name)}`);
  }
}
