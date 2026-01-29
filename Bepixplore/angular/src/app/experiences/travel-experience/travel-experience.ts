import { Component, OnInit, inject, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ConfigStateService } from '@abp/ng.core';

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
  readonly router = inject(Router);

  isEditing = false;
  editingId: string | null = null;

  city: any;
  experiences: TravelExperienceDto[] = [];
  keyword: string = '';
  selectedRating: number | null = null;
  currentUserId: string | undefined;

  newReview: Partial<CreateUpdateTravelExperienceDto> = {
    rating: 1,
    description: '',
    travelDate: new Date().toISOString()
  };

  constructor(
    private experienceService: TravelExperienceService,
    private configState: ConfigStateService
  ) {
    this.currentUserId = this.configState.getDeep('currentUser.id');
  }

  ngOnInit(): void {
    this.city = history.state.data;

    if (this.destinationId || (this.city && (this.city.id || this.city.destinationId))) {
      this.getExperiences();
    }
  }

  deleteExperience(id: string) {
    if (confirm('¿Seguro que querés borrar esta historia?')) {
      this.experienceService.delete(id).subscribe(() => {
        alert('Eliminado con éxito');
        this.getExperiences();
      });
    }
  }

  prepareEdit(exp: any) {
    this.isEditing = true;
    this.editingId = exp.id;
    this.newReview = {
      description: exp.description,
      rating: exp.rating,
      travelDate: exp.travelDate
    };
  }

  getExperiences() {
    const cityId = this.destinationId || this.city?.id || this.city?.destinationId;
    if (!cityId) return;

    const input: GetTravelExperienceListDto = {
      destinationId: cityId,
      keyword: this.keyword || '',
      rating: this.selectedRating,
      maxResultCount: 10,
      skipCount: 0        
    };

    this.experienceService.getList(input).subscribe({
      next: (data: any) => {
        this.experiences = data.items;
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
