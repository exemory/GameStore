import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {HttpClient, HttpStatusCode} from "@angular/common/http";
import {GameWithDetails} from "../../interfaces/game-with-details";
import {NotificationService} from "../../services/notification.service";
import {Game} from "../../interfaces/game";
import {environment as env} from "../../../environments/environment";
import {Comment} from "../../interfaces/comment";
import {forkJoin} from "rxjs";
import {timeSince} from "../../shared/helpers/timeSince";

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.scss']
})
export class GameComponent implements OnInit {

  loading = true;
  loadingError = false;
  game!: GameWithDetails;
  comments!: Comment[]

  constructor(private route: ActivatedRoute,
              private api: HttpClient,
              private ns: NotificationService,
              private router: Router) {
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const gameKey = params.get('gameKey')!;

      this.loadGame(gameKey);
    });
  }

  private loadGame(gameKey: string) {
    forkJoin([this.getGame(gameKey), this.getComments(gameKey)])
      .subscribe({
        next: ([game, comments]) => {
          this.game = game;
          this.comments = comments
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

  private getGame(gameKey: string) {
    return this.api.get<GameWithDetails>(`games/${gameKey}`);
  }

  private getComments(gameKey: string) {
    return this.api.get<Comment[]>(`comments?gameKey=${gameKey}`);
  }

  getGameImageUrl(game: Game): string {
    return `${env.apiUrl}games/${game.key}/image`;
  }

  get rootComments() {
    return this.comments.filter(c => c.parentId === null);
  }

  getChildren(comment: Comment) {
    return this.comments.filter(c => c.parentId === comment.id);
  }

  rootAndHasChildren(comment: Comment) {
    return comment.parentId === null && this.getChildren(comment).length;
  }

  reply(comment: Comment) {

  }

  timeSince(comment: Comment){
    return timeSince(comment.creationDate);
  }

  getFullUserNameFromComment(comment: Comment) {
    return `${comment.userInfo.firstName} ${comment.userInfo.lastName}`;
  }

  getCommentUserAvatarUrl(comment: Comment) {
    return `${env.apiUrl}avatar?username=${comment.userInfo.username}`;
  }
}
