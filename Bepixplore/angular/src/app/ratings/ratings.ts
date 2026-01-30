import { Component, Input, OnInit, inject } from '@angular/core';  // Se agrega inject
import { RatingService, RatingDto, CreateUpdateRatingDto } from '../proxy/ratings'; // Ajustá la ruta según tu proxy
import { ConfigStateService } from '@abp/ng.core';
import { CommonModule } from '@angular/common';
// 1. IMPORTANTE: Agregá esta importación
import { FormsModule } from '@angular/forms';

import { ToasterService, ConfirmationService, Confirmation } from '@abp/ng.theme.shared'; // Importamos servicios UI

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

  // Objeto para el formulario de nueva calificación (5.1 y 5.2)
  newRating: CreateUpdateRatingDto = {
    destinationId: '',
    score: 0,
    comment: ''
  };

  constructor(
    private ratingService: RatingService,
    private configState: ConfigStateService,
    private toaster: ToasterService,         // <--- AGREGADO
    private confirmation: ConfirmationService) // <--- AGREGADO 
    {
    // Obtenemos el ID del usuario actual para habilitar Edición/Eliminación (5.3)
    this.currentUserId = this.configState.getDeep('currentUser.id');
  }

  ngOnInit(): void {
    if (this.destinationId) {
      this.loadRatings();
    }
  }

  // 5.5: Listar comentarios de un destino
  loadRatings() {
    this.ratingService.getListByDestination(this.destinationId).subscribe(res => {
      this.ratings = res;

      if (this.ratings.length > 0) {
        const suma = this.ratings.reduce((acc, item) => acc + item.score, 0);

        // CAMBIÁ 'average' POR 'averageRating' AQUÍ
        this.averageRating = (suma / this.ratings.length).toFixed(1);
      } else {
        this.averageRating = '0';
      }
    });
  }

  // 5.1: Establecer puntaje desde las estrellas del HTML
  setScore(val: number) {
    this.newRating.score = val;
  }

  // 5.2: Crear nueva calificación --> CAMBIO
submitRating() {
    this.newRating.destinationId = this.destinationId;

    this.ratingService.create(this.newRating).subscribe({
      next: () => {
        // REEMPLAZO DE ALERT
        this.toaster.success('¡Gracias por tu calificación!', 'Enviado');
        this.newRating = { destinationId: '', score: 0, comment: '' }; 
        this.loadRatings(); 
      },
      error: (err) => this.toaster.error(err.error?.error?.message || 'Error al guardar', 'Error')
    });
  }

  // 5.4: Consultar promedio de calificaciones
  calculateAverage() {
    if (this.ratings.length > 0) {
      const total = this.ratings.reduce((sum, item) => sum + item.score, 0);
      this.averageRating = (total / this.ratings.length).toFixed(1);
    } else {
      this.averageRating = '0';
    }
  }

  // 5.3: Eliminar calificación propia   --> CAMBIO
  deleteRating(id: string) {
    // Confirmación elegante con botón corregido
    this.confirmation.warn(
      '¿Quieres borrar tu calificación?', 
      'Confirmar eliminación',
      { yesText: 'Confirmar' } // <--- CORRECCIÓN AQUÍ
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
