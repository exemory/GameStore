import {Component, OnInit} from '@angular/core';
import {Game} from "../../interfaces/game";
import {HttpClient} from "@angular/common/http";
import {NotificationService} from "../../services/notification.service";

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
              private ns: NotificationService) {
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
}
