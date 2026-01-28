import { Component, OnInit, inject, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

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

  ngOnInit(): void {
    // Intentamos obtener la ciudad del historial, pero priorizamos el Input
    this.city = history.state.data;

    // Si viene el ID del padre o del historial, cargamos
    if (this.destinationId || (this.city && (this.city.id || this.city.destinationId))) {
      this.getExperiences();
    }
  }

  deleteExperience(id: string) {
    if (confirm('¿Seguro que querés borrar esta historia?')) {
      this.experienceService.delete(id).subscribe(() => {
        alert('Eliminado con éxito');
        this.getExperiences(); // Recargamos la lista
      });
    }
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
        alert('¡Historia actualizada!');
        this.resetForm();
      });
    } else {
      this.experienceService.create(input).subscribe(() => {
        alert('¡Historia publicada!');
        this.resetForm();
      });
    }
  }

}
