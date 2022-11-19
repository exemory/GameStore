import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {GamesComponent} from "./components/games/games.component";
import {GameComponent} from "./components/game/game.component";
import {UsersComponent} from "./components/users/users.component";

const routes: Routes = [
  {path: 'games', component: GamesComponent},
  {path: 'games/:gameKey', component: GameComponent},
  {path: 'users', component: UsersComponent},
  {path: '**', redirectTo: 'games'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
