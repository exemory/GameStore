import {Component, OnInit} from '@angular/core';
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

@Component({
  selector: 'app-games',
  templateUrl: './games.component.html',
  styleUrls: ['./games.component.scss']
})
export class GamesComponent implements OnInit {

  loading = true;
  loadingError = false;
  games!: Game[];

  constructor(private api: HttpClient,
              private ns: NotificationService,
              private dialog: MatDialog) {
  }

  ngOnInit(): void {
    this.api.get<Game[]>('games')
      .subscribe({
        next: games => {
          this.games = games;
          this.loading = false;
        },
        error: err => {
          this.ns.notifyError(`Loading data failed. ${err.error?.message ?? ''}`, true);
          this.loading = false;
          this.loadingError = true;
        }
      })
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
}
