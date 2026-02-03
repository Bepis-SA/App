import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListService, CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { Router } from '@angular/router';
import { map } from 'rxjs/operators';
import { FavoriteService } from '../proxy/favorites/favorite.service';
import { DestinationDto } from '../proxy/application/contracts/destinations/models';
import { ToasterService, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-destinations',
  standalone: true,
  imports: [CommonModule, CoreModule, ThemeSharedModule],
  providers: [ListService],
  templateUrl: './destinations.html',
  styleUrl: './destinations.scss'
})
export class Destinations implements OnInit {
  public readonly list = inject(ListService);
  private readonly favoriteService = inject(FavoriteService);
  private readonly toaster = inject(ToasterService);
  private readonly confirmation = inject(ConfirmationService);
  private readonly router = inject(Router);

  items: DestinationDto[] = [];
  totalCount = 0;

  ngOnInit() {
    const favoriteStreamCreator = (query) =>
      this.favoriteService.getList().pipe(
        map((response) => ({
          items: response,
          totalCount: response.length
        }))
      );

    this.list.hookToQuery(favoriteStreamCreator).subscribe((response) => {
      this.items = response.items;
      this.totalCount = response.totalCount;
    });
  }

  loadFavorites() {
    this.favoriteService.getList().subscribe((response) => {
      this.items = response;
      this.totalCount = response.length;
    });
  }

  viewDetails(item: any) {
    this.router.navigate(['/destinations/details', item.id]);
  }

  delete(id: string) {
    this.confirmation.warn(
      '¿Seguro que querés quitar este destino de tus favoritos?',
      'Quitar de favoritos',
      {
        yesText: 'Quitar',
        noText: 'Cancelar',
      } as Partial<Confirmation.Options>
    ).subscribe((status: Confirmation.Status) => {
      if (status === Confirmation.Status.confirm) {
        this.favoriteService.remove(id).subscribe({
          next: () => {
            this.loadFavorites();

            this.toaster.success(
              'El destino ha sido eliminado de tu lista.',
              'Favorito quitado'
            );
          },
          error: (err) => {
            this.toaster.error(
              'Hubo un problema al intentar eliminar el favorito.',
              'Error'
            );
            console.error(err);
          }
        });
      }
    });
  }
}
