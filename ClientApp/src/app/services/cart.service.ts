import {Injectable} from '@angular/core';
import {Game} from "../interfaces/game";

@Injectable({
  providedIn: 'root'
})
export class CartService {

  games: { [gameId: string]: number } = {};

  constructor() {

  }

  getAllGames() {
    return structuredClone(this.games);
  }

  addGame(game: Game) {
    if (game.id in this.games) {
      this.games[game.id]++;
      return;
    }

    this.games[game.id] = 1;
  }

  removeGame(game: Game) {
    delete this.games[game.id];
  }
}
