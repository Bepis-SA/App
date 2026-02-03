import { Component, OnInit, inject } from '@angular/core';
import { DynamicLayoutComponent } from '@abp/ng.core';
import { LoaderBarComponent } from '@abp/ng.theme.shared';
import { ReplaceableComponentsService } from '@abp/ng.core'; 
import { eAccountComponents } from '@abp/ng.account'; // <--- Enum con los nombres de componentes
import { MyManageProfileComponent } from './account/my-manage-profile/my-manage-profile.component';

@Component({
  selector: 'app-root',
  template: `
    <abp-loader-bar />
    <abp-dynamic-layout />
  `,
  imports: [LoaderBarComponent, DynamicLayoutComponent],
})
export class AppComponent implements OnInit  {
 private replaceableComponents = inject(ReplaceableComponentsService);
 ngOnInit() {
   this.replaceableComponents.add({
     component: MyManageProfileComponent,
     key: eAccountComponents.ManageProfile,
   });
 }
}

