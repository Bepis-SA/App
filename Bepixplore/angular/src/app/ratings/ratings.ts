import { RatingService, RatingDto, CreateUpdateRatingDto } from '../proxy/ratings';
import { ConfigStateService } from '@abp/ng.core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Component, Input, OnInit, OnChanges, SimpleChanges, inject } from '@angular/core';

import { ToasterService, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-ratings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ratings.html',
  styleUrl: './ratings.scss'
})
export class RatingsComponent implements OnInit, OnChanges {
  @Input() destinationId: string;

  ratings: RatingDto[] = [];
  averageRating: string = '0';
  currentUserId: string;
  isEditing: boolean = false;
  editingRatingId: string | null = null;

  newRating: CreateUpdateRatingDto = {
    destinationId: '',
    score: 0,
    comment: ''
  };

  constructor(
    private ratingService: RatingService,
    private configState: ConfigStateService,
    private toaster: ToasterService,
    private confirmation: ConfirmationService)
  {
    this.currentUserId = this.configState.getDeep('currentUser.id');
  }


  ngOnChanges(changes: SimpleChanges): void {
    if (changes['destinationId'] && this.destinationId) {
      this.loadRatings();
    }
  }

  ngOnInit(): void {
    if (this.destinationId) {
      this.loadRatings();
    }
  }

  loadRatings() {
    const destId = this.destinationId;
    if (!destId) return;

    this.ratingService.getAverageRating(destId).subscribe(avg => {
      this.averageRating = avg.toFixed(1);
    });

    this.ratingService.getListByDestination(destId).subscribe(res => {
      let list = res.items;

      const userRatingIndex = list.findIndex(r => r.userId === this.currentUserId);

      if (userRatingIndex > 0) {
        const userRating = list.splice(userRatingIndex, 1)[0];
        list.unshift(userRating);
      }

      this.ratings = list;
    });
  }

  setScore(val: number) {
    this.newRating.score = val;
  }

  resetForm() {
    this.isEditing = false;
    this.editingRatingId = null;
    this.newRating = { destinationId: '', score: 0, comment: '' };
  }

  prepareEdit(rat: RatingDto) {
    this.isEditing = true;
    this.editingRatingId = rat.id;
    this.newRating = {
      destinationId: this.destinationId,
      score: rat.score,
      comment: rat.comment
    };
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  submitRating() {
    this.newRating.destinationId = this.destinationId;

    if (this.isEditing && this.editingRatingId) {
      this.ratingService.update(this.editingRatingId, this.newRating).subscribe({
        next: () => {
          this.toaster.success('Calificación actualizada', 'Éxito');
          this.resetForm();
          this.loadRatings();
        },
        error: (err) => this.toaster.error(err.error?.error?.message || 'Error al actualizar', 'Error')
      });
    } else {
      this.ratingService.create(this.newRating).subscribe({
        next: () => {
          this.toaster.success('¡Gracias por tu calificación!', 'Enviado');
          this.resetForm();
          this.loadRatings();
        },
        error: (err) => this.toaster.error(err.error?.error?.message || 'Error al guardar', 'Error')
      });
    }
  }
  get hasUserAlreadyRated(): boolean {
    return this.ratings.some(rat => rat.userId === this.currentUserId);
  }

  deleteRating(id: string) {
    this.confirmation.warn(
      '¿Quieres borrar tu calificación?',
      'Confirmar eliminación',
      { yesText: 'Confirmar' }
    ).subscribe((status: Confirmation.Status) => {
      if (status === Confirmation.Status.confirm) {
        this.ratingService.delete(id).subscribe(() => {
          this.toaster.info('Calificación eliminada', 'Borrado');
          this.loadRatings();
        });
      }
    });
  }
}
