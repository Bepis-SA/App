// src/app/cities/search-city/search-city.component.ts
import { FavoriteService } from '../../proxy/favorites/favorite.service';
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
  private readonly favoriteService = inject(FavoriteService);

  searchForm = new FormGroup({
    query: new FormControl(''),
    country: new FormControl(''), // <--- Pais
    minPopulation: new FormControl<number | null>(null), // <--- Poblacion
  });

  get queryControl(): FormControl {
    return this.searchForm.get('query') as FormControl;
  }

  // Agrego estos getters debajo de queryControl para que sea más fácil usarlos:
  get countryControl(): FormControl { return this.searchForm.get('country') as FormControl; }
  get minPopulationControl(): FormControl { return this.searchForm.get('minPopulation') as FormControl; }

  @Output() citySelected = new EventEmitter<CityDto>();

  cities: CityDto[] = [];
  loading = false;

  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    this.searchForm.valueChanges // Escucha cambios en los 3 campos a la vez
      .pipe(
        takeUntil(this.destroy$),
        debounceTime(400),
        distinctUntilChanged((prev, curr) => JSON.stringify(prev) === JSON.stringify(curr)),
        switchMap(values => {
          const query = (values.query ?? '').trim();
          this.loading = true;
          this.cities = [];

          if (query.length < 2) {
            this.loading = false;
            return of({ cities: [] } as CitySearchResultDto);
          }

          // 3. ARMAMOS EL REQUEST CON LOS 3 FILTROS
          const request: CitySearchRequestDto = {
            partialName: query,
            country: values.country || undefined,
            minPopulation: values.minPopulation || undefined
          };

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

    // 2. CAMBIO CLAVE: Llamamos al FavoriteService en lugar de DestinationService
    this.favoriteService.add(input).subscribe({
      next: () => {
        alert(`¡${city.name} se guardó en tus destinos favoritos!`);
      },
      error: (err) => {
        console.error('Error al guardar en favoritos:', err);
        alert('Hubo un error al intentar guardar en tus favoritos.');
      }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
