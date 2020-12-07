import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import {MatIconModule} from '@angular/material/icon';
import {MatExpansionModule} from '@angular/material/expansion';



const materialModules = [
  // MatButtonModule,
  MatChipsModule,
  MatIconModule,
  MatExpansionModule
];

@NgModule({
  imports: [
    ...materialModules
  ],
  exports: [
    ...materialModules
  ],
})
export class MaterialModule {
}