import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {HeaderComponent} from './components/header/header.component';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {GamesComponent} from './components/games/games.component';
import {MatProgressSpinnerModule} from "@angular/material/progress-spinner";
import {HTTP_INTERCEPTORS, HttpClientModule} from "@angular/common/http";
import {MatSnackBarModule} from "@angular/material/snack-bar";
import {ApiInterceptor} from "./interceptors/api.interceptor";
import {MatButtonModule} from "@angular/material/button";
import {GameComponent} from './components/game/game.component';
import {MatIconModule} from "@angular/material/icon";
import {MatMenuModule} from "@angular/material/menu";
import {AddGameDialogComponent} from './components/games/add-game-dialog/add-game-dialog.component';
import {MatDialogModule} from "@angular/material/dialog";
import {MatFormFieldModule} from "@angular/material/form-field";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {MatInputModule} from "@angular/material/input";
import {MatChipsModule} from "@angular/material/chips";
import {MatAutocompleteModule} from "@angular/material/autocomplete";
import {DragDropModule} from "@angular/cdk/drag-drop";
import {ChipsInputComponent} from './shared/components/chips-input/chips-input.component';
import {CurrencyInputComponent} from "./shared/components/currency-input/currency-input.component";
import {EditGameDialogComponent} from './components/games/edit-game-dialog/edit-game-dialog.component';
import {MatSelectModule} from "@angular/material/select";
import {CurrencyPipe} from "@angular/common";
import {SignInDialogComponent} from './components/sign-in-dialog/sign-in-dialog.component';
import {MatCheckboxModule} from "@angular/material/checkbox";
import {SignUpDialogComponent} from './components/sign-up-dialog/sign-up-dialog.component';
import {UploadAvatarDialogComponent} from './components/upload-avatar-dialog/upload-avatar-dialog.component';
import {DndDirective} from './shared/directives/dnd.directive';
import {TimeSincePipe} from './shared/pipes/time-since.pipe';
import {CartDialogComponent} from './components/cart-dialog/cart-dialog.component';
import {MatBadgeModule} from "@angular/material/badge";
import {CompletionOrderDialogComponent} from './components/completion-order-dialog/completion-order-dialog.component';
import { UsersComponent } from './components/users/users.component';
import {MatSortModule} from "@angular/material/sort";
import {MatTableModule} from "@angular/material/table";
import {MatPaginatorModule} from "@angular/material/paginator";

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    GamesComponent,
    GameComponent,
    AddGameDialogComponent,
    ChipsInputComponent,
    CurrencyInputComponent,
    EditGameDialogComponent,
    SignInDialogComponent,
    SignUpDialogComponent,
    UploadAvatarDialogComponent,
    DndDirective,
    TimeSincePipe,
    CartDialogComponent,
    CompletionOrderDialogComponent,
    UsersComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatProgressSpinnerModule,
    HttpClientModule,
    MatSnackBarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatDialogModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatInputModule,
    MatChipsModule,
    MatAutocompleteModule,
    DragDropModule,
    FormsModule,
    MatSelectModule,
    MatCheckboxModule,
    MatBadgeModule,
    MatSortModule,
    MatTableModule,
    MatPaginatorModule
  ],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: ApiInterceptor, multi: true},
    CurrencyPipe
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
