import { Component, Input, OnInit } from '@angular/core';
import { RatingService, RatingDto, CreateUpdateRatingDto } from '../proxy/ratings';
import { ConfigStateService } from '@abp/ng.core';
import { CommonModule } from '@angular/common';
// 1. IMPORTANTE: Agregá esta importación
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-ratings',
  standalone: true,
  // 2. AGREGALO AQUÍ: Esto habilita el uso de [(ngModel)]
  imports: [CommonModule, FormsModule],
  templateUrl: './ratings.html',
  styleUrl: './ratings.scss'
})
export class RatingsComponent implements OnInit {
  @Input() destinationId: string; // Recibe el ID desde el padre (CityDetail)

  ratings: RatingDto[] = [];
  averageRating: string = '0';
  currentUserId: string;
  isEditing = false;
  editingId: string | null = null;

  // Objeto para el formulario de nueva calificación (5.1 y 5.2)
  newRating: CreateUpdateRatingDto = {
    destinationId: '',
    score: 0,
    comment: ''
  };

  constructor(
    private ratingService: RatingService,
    private configState: ConfigStateService
  ) {
    this.currentUserId = this.configState.getDeep('currentUser.id');
  }

  submitRating() {
    this.newRating.destinationId = this.destinationId;

    if (this.isEditing && this.editingId) {
      this.ratingService.update(this.editingId, this.newRating).subscribe({
        next: () => {
          alert('¡Calificación actualizada!');
          this.resetForm();
        },
        error: (err) => alert(err.error?.error?.message || 'Error al actualizar')
      });
    } else {
      this.ratingService.create(this.newRating).subscribe({
        next: () => {
          alert('¡Gracias por calificar!');
          this.resetForm();
        },
        error: (err) => alert(err.error?.error?.message || 'Error al guardar')
      });
    }
  }

  prepareEdit(rating: RatingDto) {
    this.isEditing = true;
    this.editingId = rating.id;

    this.newRating = {
      ...this.newRating,
      score: rating.score,
      comment: rating.comment
    };

    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  ngOnInit(): void {
    if (this.destinationId) {
      this.loadRatings();
    }
  }

  loadRatings() {
    const destId = this.destinationId;

    this.ratingService.getAverageRating(destId).subscribe(avg => {
      this.averageRating = avg.toFixed(1);
    });

    this.ratingService.getListByDestination(destId).subscribe(res => {
      this.ratings = res;
    });
  }

  resetForm() {
    this.isEditing = false;
    this.editingId = null;
    this.newRating = {
      destinationId: this.destinationId,
      score: 0,
      comment: ''
    };
    this.loadRatings(); // Refresca la lista y el promedio
  }

  // 5.1: Establecer puntaje desde las estrellas del HTML
  setScore(val: number) {
    this.newRating.score = val;
  }

  // 5.3: Eliminar calificación propia
  deleteRating(id: string) {
    if (confirm('¿Borrar tu calificación?')) {
      this.ratingService.delete(id).subscribe(() => this.loadRatings());
    }
  }
}
