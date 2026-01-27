import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TravelExperienceService } from '../../proxy/experiences/travel-experience.service';
import { TravelExperienceDto, CreateUpdateTravelExperienceDto } from '../../proxy/experiences/models';

@Component({
  selector: 'app-travel-experience',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './travel-experience.component.html',
  styleUrls: ['./travel-experience.component.scss']
})
export class TravelExperienceComponent implements OnInit {
  private readonly experienceService = inject(TravelExperienceService);

  experiences: TravelExperienceDto[] = [];
  keyword: string = ''; // 4.6: Para el buscador por palabras clave

  // Datos para el formulario de nueva reseña
  newReview: Partial<CreateUpdateTravelExperienceDto> = {
    rating: 1, // 1: Positiva por defecto
    description: '',
    travelDate: new Date().toISOString()
  };

  ngOnInit(): void {
    this.getExperiences();
  }

  // 4.6: Obtener y filtrar experiencias
  getExperiences() {
    this.experienceService.getList(this.keyword).subscribe((data) => {
      this.experiences = data;
    });
  }

  // 4.5: Enviar la reseña al backend
  saveReview(destinationId: string) {
    const input = {
      ...this.newReview,
      destinationId: destinationId
    } as CreateUpdateTravelExperienceDto;

    this.experienceService.create(input).subscribe(() => {
      alert('¡Reseña guardada con éxito!');
      this.getExperiences(); // Refrescamos la lista
      this.newReview.description = ''; // Limpiamos el texto
    });
  }
}
