import {Component, OnInit, ViewEncapsulation} from '@angular/core';
import {Game} from "../../interfaces/game";
import {HttpClient} from "@angular/common/http";
import {NotificationService} from "../../services/notification.service";
import {environment as env} from "../../../environments/environment";
import {MatDialog} from "@angular/material/dialog";
import {AddGameDialogComponent} from "./add-game-dialog/add-game-dialog.component";
import {EditGameDialogComponent} from "./edit-game-dialog/edit-game-dialog.component";
import {GameUpdateData} from "../../interfaces/game-update-data";
import {Genre} from "../../interfaces/genre";
import {PlatformType} from "../../interfaces/platform-type";
import {forkJoin} from "rxjs";

@Component({
  selector: 'app-games',
  templateUrl: './games.component.html',
  styleUrls: ['./games.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class GamesComponent implements OnInit {

  loading = true;
  loadingError = false;

  games!: Game[];
  genres!: Genre[];
  platforms!: PlatformType[];

  nameFilter = '';
  genresFilter = <Genre[]>[];

  constructor(private api: HttpClient,
              private ns: NotificationService,
              private dialog: MatDialog) {
  }

  ngOnInit(): void {
    forkJoin([this.getGames(), this.getGenres()])
      .subscribe({
        next: ([games, genres]) => {
          this.games = games;
          this.genres = genres;
          this.loading = false;
        }, error: err => {
          this.ns.notifyError(`Loading data failed. ${err.error?.message ?? ''}`, true);
          this.loading = false;
          this.loadingError = true;
        }
      });
  }

  private getGames() {
    return this.api.get<Game[]>('games');
  }

  private getGenres() {
    return this.api.get<Genre[]>('genres');
  }

  onBuyClick(event: any, game: Game): void {
    event.stopPropagation();
  }

  getGameImageUrl(game: Game): string {
    return `url("${env.apiUrl}games/${game.key}/image")`;
  }

  openAddGameDialog() {
    const dialogRef = this.dialog.open(AddGameDialogComponent,
      {
        maxWidth: '400px',
        width: '100%'
      });

    dialogRef.afterClosed().subscribe((game?: Game) => {
        if (game) {
          this.games.push(game);
        }
      }
    );
  }

  openEditGameDialog(game: Game) {
    const dialogRef = this.dialog.open(EditGameDialogComponent,
      {
        maxWidth: '400px',
        width: '100%',
        data: game
      });

    dialogRef.afterClosed().subscribe((update?: { data: GameUpdateData, genres: Genre[], platforms: PlatformType[] }) => {
        if (update) {
          game.key = update.data.key;
          game.name = update.data.name;
          game.price = update.data.price;
          game.description = update.data.description;

          game.genres = update.genres.map(g => g.name);
        }
      }
    );
  }

  deleteGame(game: Game) {
    this.api.delete(`games/${game.id}`)
      .subscribe({
        next: () => {
          const index = this.games.indexOf(game);
          if (index !== -1) {
            this.games.splice(index, 1);
          }

          this.ns.notifySuccess("Game has been deleted.");
        },
        error: err => {
          this.ns.notifyError(`Operation failed. ${err.error?.message ?? ''}`);
        }
      });
  }

  get filteredGames(): Game[] {
    const nameFilter = this.nameFilter?.trim().toLowerCase();
    let filteredGames = this.games;

    if (this.nameFilter) {
      filteredGames = filteredGames.filter(g => g.name.toLowerCase().includes(nameFilter));
    }

    if (this.genresFilter.length) {
      filteredGames = filteredGames.filter(g => this.genresFilter.some(genre => g.genres.includes(genre.name)));
    }

    return filteredGames;
  }

  get rootGenres() {
    const genres = this.genres.filter(g => g.parentId === null);
    return genres.sort((a, b) => a.name.localeCompare(b.name));
  }

  genresByRootGenre(rootGenre: Genre) {
    const genres = this.genres.filter(g => g.id === rootGenre.id || g.parentId === rootGenre.id);
    return genres.sort((a, b) => a.name.localeCompare(b.name));
  }

  get noGamesByFilter() {
    return (this.nameFilter || this.genresFilter.length) && !this.filteredGames.length;
  }
}
