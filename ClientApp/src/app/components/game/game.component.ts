import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {HttpClient, HttpStatusCode} from "@angular/common/http";
import {GameWithDetails} from "../../interfaces/game-with-details";
import {NotificationService} from "../../services/notification.service";

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.scss']
})
export class GameComponent implements OnInit {

  loading = true;
  loadingError = false;
  game!: GameWithDetails;

  constructor(private route: ActivatedRoute,
              private api: HttpClient,
              private ns: NotificationService,
              private router: Router) {
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const gameKey = params.get('gameKey')!;

      this.loadGameInfo(gameKey);
    });
  }

  private loadGameInfo(gameKey: string) {
    this.api.get<GameWithDetails>(`games/${gameKey}`)
      .subscribe({
        next: game => {
          this.game = game;
          this.loading = false;
        },
        error: err => {
          this.loadingError = true;

          if (err.status === HttpStatusCode.NotFound) {
            this.ns.notifyError('Game does not exist.');
            this.router.navigate(['../']);
            return;
          }

          this.ns.notifyError(`Loading data failed. ${err.error?.message ?? ''}`, true);
        }
      });
  }

}
