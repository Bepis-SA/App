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
    // 1. Preparamos el "molde" (DTO) con los datos de la ciudad
    const input = {
      name: city.name,
      country: city.country,
      city: city.name,
      population: city.population || 0,
      photo: '', // Podés dejarlo vacío o asignar una imagen por defecto
      updateDate: new Date().toISOString(),
      coordinates: {
        latitude: city.latitude,
        longitude: city.longitude
      }
    };

    // 2. Llamamos al método create que ABP generó en el proxy
    this.destinationService.create(input).subscribe({
      next: () => {
        // Usamos el alert por ahora, luego podés usar ToasterService de ABP
        alert(`¡${city.name} se guardó correctamente en la base de datos!`);
      },
      error: (err) => {
        console.error('Error al guardar:', err);
        alert('Hubo un error al intentar guardar la ciudad.');
      }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
