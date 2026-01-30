import { Component, OnInit, OnChanges, SimpleChanges, Input, inject } from '@angular/core';
import { CommonModule, Location } from '@angular/common'; // Location
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { ToasterService, ConfirmationService, Confirmation } from '@abp/ng.theme.shared'; // notificaciones

import { DestinationService } from '../../proxy/destinations/destination.service';
import { TravelExperienceService } from '../../proxy/experiences/travel-experience.service';
import { TravelExperienceDto, CreateUpdateTravelExperienceDto } from '../../proxy/experiences/models';

export interface GetTravelExperienceListDto {
  destinationId: string;
  keyword?: string;
  rating?: number | null;
  maxResultCount: number;
  skipCount: number;
  sorting?: string;
}

@Component({
  selector: 'app-travel-experience',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './travel-experience.html',
  styleUrls: ['./travel-experience.scss']
})

export class TravelExperienceComponent implements OnInit {
  // 3. RECIBIR EL ID DESDE EL PADRE (Soluciona NG8002)
  @Input() destinationId: string;

  private readonly destinationService = inject(DestinationService);
  private readonly experienceService = inject(TravelExperienceService);
  readonly router = inject(Router);
  private readonly location = inject(Location);

  private readonly toaster = inject(ToasterService);  //notificaciones
  private readonly confirmation = inject(ConfirmationService);

  isEditing = false; // Estado para saber si estamos editando
  editingId: string | null = null;
  city: any;
  experiences: TravelExperienceDto[] = [];
  keyword: string = '';
  selectedRating: number | null = null; // null significa "Todas"

  newReview: Partial<CreateUpdateTravelExperienceDto> = {
    rating: 1,
    description: '',
    travelDate: new Date().toISOString()
  };

// VITAL: Escucha si el padre le manda un ID nuevo (ej: al dar Favorito)
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['destinationId'] && this.destinationId) {
      this.getExperiences();
    }
  }
  
  ngOnInit(): void {
    // Intentamos obtener la ciudad del historial, pero priorizamos el Input
    this.city = history.state.data;

    // Si viene el ID del padre o del historial, cargamos
    if (this.destinationId || (this.city && (this.city.id || this.city.destinationId))) {
      this.getExperiences();
    }
  }

deleteExperience(id: string) {
    this.confirmation.warn(
      '¿Estás seguro de que quieres borrar esta historia?', 
      'Confirmar eliminación',
      { yesText: 'Confirmar' } // <--- AGREGAMOS ESTA LÍNEA
    ).subscribe((status: Confirmation.Status) => {
      
      if (status === Confirmation.Status.confirm) {
        this.experienceService.delete(id).subscribe({
          next: () => {
            this.experiences = this.experiences.filter(e => e.id !== id);
            this.getExperiences();
            this.toaster.info('Experiencia eliminada correctamente', 'Eliminado');
          },
          error: () => this.toaster.error('No se pudo eliminar la experiencia', 'Error')
        });
      }
    });
  }

  prepareEdit(exp: any) {
    this.isEditing = true;
    this.editingId = exp.id;
    // Llenamos el formulario con lo que ya existe
    this.newReview = {
      description: exp.description,
      rating: exp.rating,
      travelDate: exp.travelDate // Se envía, pero el backend lo ignorará según tu regla
    };
  }

  getExperiences() {
    const cityId = this.destinationId || this.city?.id || this.city?.destinationId;
    if (!cityId) return;

    // Agregamos los valores de paginación aquí
    const input: GetTravelExperienceListDto = {
      destinationId: cityId,
      keyword: this.keyword || '',
      rating: this.selectedRating,
      maxResultCount: 10, // Traemos las primeras 10 reseñas
      skipCount: 0        // Empezamos desde la primera
    };

    this.experienceService.getList(input).subscribe({
      next: (data) => {
        this.experiences = data;
      },
      error: (err) => console.error('Error:', err)
    });
  }

  resetForm() {
    this.getExperiences();
    this.newReview = { rating: 1, description: '', travelDate: new Date().toISOString() };
    this.isEditing = false;
    this.editingId = null;
    this.getExperiences();
  }

  saveReview() {
    const cityId = this.destinationId || this.city?.id;
    const input = { ...this.newReview, destinationId: cityId } as any;

    if (this.isEditing && this.editingId) {
      this.experienceService.update(this.editingId, input).subscribe(() => {
        this.toaster.success('¡Historia actualizada con éxito!');  //TOASTER  
        this.resetForm();
      });
    } else {
      this.experienceService.create(input).subscribe(() => {
        this.toaster.success('¡Historia publicada!');  //TOASTER
        this.resetForm();
      });
    }
  }
goBack(): void {
    this.location.back();
  }
}
