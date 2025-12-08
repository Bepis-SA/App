// src/app/cities/search-city/search-city.component.ts
import { Component, OnInit, OnDestroy, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl, FormGroup } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { DestinationService } from '../../proxy/destinations/destination.service';
import { CityDto } from '../../proxy/application/contracts/cities/models';
import { CitySearchRequestDto, CitySearchResultDto } from '../../proxy/cities/models';
import { Subject, of } from 'rxjs';
import {
  debounceTime,
  distinctUntilChanged,
  switchMap,
  catchError,
  takeUntil,
  finalize,
} from 'rxjs/operators';

@Component({
  selector: 'app-search-city',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    CoreModule,
  ],
  templateUrl: './search-city.component.html',
  styleUrls: ['./search-city.component.scss'],
})

export class SearchCityComponent implements OnInit, OnDestroy {
  private readonly destinationService = inject(DestinationService);

  searchForm = new FormGroup({
    query: new FormControl(''),
  });

  get queryControl(): FormControl {
    return this.searchForm.get('query') as FormControl;
  }

  @Output() citySelected = new EventEmitter<CityDto>();

  cities: CityDto[] = [];
  loading = false;

  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    this.queryControl.valueChanges
      .pipe(
        takeUntil(this.destroy$),
        debounceTime(400),
        distinctUntilChanged(),
        switchMap(term => {
          const query = (term ?? '').trim();
          this.loading = true;
          this.cities = [];

          if (query.length < 2) {
            this.loading = false;
            return of({ cities: [] } as CitySearchResultDto);
          }

          const request: CitySearchRequestDto = { partialName: query };
          return this.destinationService.searchCities(request).pipe(
            catchError(err => {
              console.error('Error al buscar ciudades:', err);
              return of({ cities: [] } as CitySearchResultDto);
            }),
            finalize(() => this.loading = false)
          );
        })
      )
      .subscribe((result: CitySearchResultDto) => {
        this.cities = result.cities ?? [];
      });
  }

  selectCity(city: CityDto): void {
    this.citySelected.emit(city);
    this.searchForm.reset();
    this.cities = [];
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}