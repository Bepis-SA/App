import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListService, CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';

// Importamos el servicio y el DTO usando las rutas que vimos en tus capturas
import { DestinationService } from '../proxy/destinations/destination.service';
import { DestinationDto } from '../proxy/application/contracts/destinations/models';

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
  private readonly destinationService = inject(DestinationService);

  items: DestinationDto[] = [];
  totalCount = 0;

  ngOnInit() {
    // Conectamos la tabla con los datos de SQL Server
    const destinationStreamCreator = (query) => this.destinationService.getList(query);

    this.list.hookToQuery(destinationStreamCreator).subscribe((response) => {
      this.items = response.items;
      this.totalCount = response.totalCount;
    });
  }

  delete(id: string) {
    if (confirm('¿Seguro que querés eliminar este destino de tus favoritos?')) {
      this.destinationService.delete(id).subscribe(() => {
        this.list.get(); // Recarga la lista automáticamente
      });
    }
  }
}
