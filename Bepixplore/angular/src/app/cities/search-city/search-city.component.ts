import { FavoriteService } from '../../proxy/favorites/favorite.service';
import { Component, OnInit, OnDestroy, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl, FormGroup } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { DestinationService } from '../../proxy/destinations/destination.service';
import { CityDto } from '../../proxy/application/contracts/cities/models';
import { CitySearchRequestDto, CitySearchResultDto } from '../../proxy/cities/models';
import { Router } from '@angular/router';
import { Subject, of } from 'rxjs';
import { GeoDbCitySearchService } from '@proxy/external/geo-db';
import {
  debounceTime,
  distinctUntilChanged,
  switchMap,
  catchError,
  takeUntil,
  finalize,
} from 'rxjs/operators';

import { ToasterService } from '@abp/ng.theme.shared';

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
  private readonly citySearchService = inject(GeoDbCitySearchService);
  private readonly destinationService = inject(DestinationService);
  private readonly favoriteService = inject(FavoriteService);
  private readonly router = inject(Router);
  private readonly toaster = inject(ToasterService);

  searchForm = new FormGroup({
    query: new FormControl(''),
    country: new FormControl(''),
    minPopulation: new FormControl<number | null>(null),
    isPopular: new FormControl(false),
  });

  get queryControl(): FormControl {
    return this.searchForm.get('query') as FormControl;
  }
  get isPopularControl(): FormControl { return this.searchForm.get('isPopular') as FormControl; }
  get countryControl(): FormControl { return this.searchForm.get('country') as FormControl; }
  get minPopulationControl(): FormControl { return this.searchForm.get('minPopulation') as FormControl; }

  @Output() citySelected = new EventEmitter<CityDto>();

  cities: CityDto[] = [];
  loading = false;

  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    this.searchForm.valueChanges
      .pipe(
        takeUntil(this.destroy$),
        debounceTime(400),
        distinctUntilChanged((prev, curr) => JSON.stringify(prev) === JSON.stringify(curr)),
        switchMap(values => {
          const query = (values.query ?? '').trim();
          const isPopular = values.isPopular ?? false;

          this.loading = true;
          this.cities = [];

          if (!isPopular && query.length < 2) {
            this.loading = false;
            return of({ cities: [] } as CitySearchResultDto);
          }

          const request: CitySearchRequestDto = {
            partialName: query,
            country: values.country || undefined,
            minPopulation: values.minPopulation || undefined,
            isPopularFilter: isPopular
          };

          return this.citySearchService.searchCities(request).pipe(
            catchError(err => {
              console.error('Error:', err);
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

  saveCity(city: CityDto): void {
    const input = {
      name: city.name,
      country: city.country,
      city: city.name,
      population: city.population || 0,
      photo: '',
      updateDate: new Date().toISOString(),
      coordinates: {
        latitude: city.latitude,
        longitude: city.longitude
      }
    };

    this.favoriteService.add(input).subscribe({
      next: () => {
        this.toaster.success(`¡${city.name} se guardó en tus favoritos!`, 'Guardado');
      },
      error: (err) => {
        console.error('Error al guardar:', err);
        this.toaster.error('No se pudo guardar en favoritos', 'Error');
      }
    });
  }

  viewDetails(city: any) {
    const cityInput = {
      name: city.name,
      country: city.country,
      city: city.name,
      population: city.population || 0,
      photo: '',
      updateDate: new Date().toISOString(),
      coordinates: { latitude: city.latitude, longitude: city.longitude }
    };

    this.destinationService.create(cityInput as any).subscribe({
      next: (res: any) => {
        this.router.navigate(['/destinations/details'], { state: { data: res } });
      },
      error: (err) => {
        console.error('Error al preparar el destino:', err);
        this.router.navigate(['/destinations/details'], { state: { data: city } });
      }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
