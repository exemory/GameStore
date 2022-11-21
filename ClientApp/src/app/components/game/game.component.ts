import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {HttpClient, HttpStatusCode} from "@angular/common/http";
import {GameWithDetails} from "../../interfaces/game-with-details";
import {NotificationService} from "../../services/notification.service";
import {Game} from "../../interfaces/game";
import {environment as env} from "../../../environments/environment";
import {Comment} from "../../interfaces/comment";
import {forkJoin} from "rxjs";
import {FormBuilder} from "@angular/forms";
import {CommentCreationData} from "../../interfaces/comment-creation-data";
import {AuthService} from "../../services/auth.service";
import {CommentUpdateData} from "../../interfaces/comment-update-data";
import {CartService} from "../../services/cart.service";
import {UserRole} from "../../enums/user-role";

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.scss']
})
export class GameComponent implements OnInit {

  @ViewChild('editCommentBody') editCommentBody!: ElementRef;
  @ViewChild('replyCommentBody') replyCommentBody!: ElementRef;

  loading = true;
  loadingError = false;
  game!: GameWithDetails;
  comments!: Comment[];
  deletedComments = <Comment[]>[];

  sendingCreateRequest = false;
  commentCreationData!: CommentCreationData;

  editingComment?: Comment;
  commentUpdateData: CommentUpdateData = {
    body: ''
  }
  sendingUpdateRequest = false;

  replyingComment?: Comment;
  commentReplyData!: CommentCreationData;
  sendingReplyRequest = false;

  constructor(private route: ActivatedRoute,
              private api: HttpClient,
              private ns: NotificationService,
              private router: Router,
              private fb: FormBuilder,
              private auth: AuthService,
              private cart: CartService) {
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

          this.initData();

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

  private initData() {
    this.commentCreationData = {
      gameId: this.game.id,
      body: ''
    }

    this.commentReplyData = {
      gameId: this.game.id,
      body: ''
    }
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

  get isUserManager() {
    if (!this.auth.isLoggedIn.value) {
      return false;
    }

    const userRoles = this.auth.session!.userInfo.roles;

    return userRoles.includes(UserRole.Manager);
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

  getFullUserNameFromComment(comment: Comment) {
    return `${comment.userInfo.firstName} ${comment.userInfo.lastName}`;
  }

  isCurrentUserAuthorOfComment(comment: Comment) {
    return comment.userInfo.username === this.auth.session?.userInfo.username;
  }

  isUserHasAccessToManipulateComment(comment: Comment) {
    return this.isUserManager || this.isCurrentUserAuthorOfComment(comment);
  }

  getCommentUserAvatarUrl(comment: Comment) {
    return comment.userInfo.hasAvatar ? `${env.apiUrl}avatar?username=${comment.userInfo.username}` :
      'assets/default-user-avatar.png';
  }

  isCommentDeleted(comment: Comment) {
    return this.deletedComments.includes(comment);
  }

  postCreatedComment() {
    if (!this.auth.isLoggedIn.value) {
      this.auth.openSignInDialog();
      return;
    }

    this.commentCreationData.body = this.commentCreationData.body.trim();

    this.sendingCreateRequest = true;

    this.api.post<Comment>('comments', this.commentCreationData)
      .subscribe({
        next: comment => {
          this.comments.unshift(comment);
          this.commentCreationData.body = '';
          this.sendingCreateRequest = false;

          this.ns.notifySuccess('Comment has been posted.');
        },
        error: err => {
          this.sendingCreateRequest = false;
          this.ns.notifyError(`Operation failed. ${err.error?.message ?? ''}`);
        }
      });
  }

  deleteComment(comment: Comment) {
    if (this.isCommentEditing(comment)) {
      this.cancelCommentEditing();
    }

    if (this.isCommentReplying(comment)) {
      this.cancelCommentReply();
    }

    this.api.delete(`comments/${comment.id}`)
      .subscribe({
        next: () => {
          this.deletedComments.push(comment);
        },
        error: err => {
          this.ns.notifyError(`Operation failed. ${err.error?.message ?? ''}`);
        }
      });
  }

  restoreComment(comment: Comment) {
    this.api.put(`comments/${comment.id}/restore`, undefined)
      .subscribe({
        next: () => {
          const index = this.deletedComments.indexOf(comment);
          if (index !== -1) {
            this.deletedComments.splice(index, 1);
          }
        },
        error: err => {
          this.ns.notifyError(`Operation failed. ${err.error?.message ?? ''}`);
        }
      });
  }

  editComment(comment: Comment) {
    this.cancelCommentReply();
    this.commentUpdateData.body = comment.body;
    this.editingComment = comment;

    setTimeout(() => this.editCommentBody.nativeElement.focus());
  }

  cancelCommentEditing() {
    this.editingComment = undefined;
  }

  replyComment(comment: Comment) {
    if (!this.auth.isLoggedIn.value) {
      this.auth.openSignInDialog();
      return;
    }

    this.cancelCommentEditing();
    this.replyingComment = comment;

    setTimeout(() => this.replyCommentBody.nativeElement.focus());
  }

  cancelCommentReply() {
    this.commentReplyData.body = '';
    this.replyingComment = undefined;
  }

  isCommentEditing(comment: Comment) {
    return comment === this.editingComment;
  }

  isCommentReplying(comment: Comment) {
    return comment === this.replyingComment;
  }

  saveEditedComment(comment: Comment) {
    this.commentUpdateData.body = this.commentUpdateData.body.trim();

    this.sendingUpdateRequest = true;

    this.api.put<Comment>(`comments/${comment.id}`, this.commentUpdateData)
      .subscribe({
        next: () => {
          comment.body = this.commentUpdateData.body;
          this.cancelCommentEditing();
          this.sendingUpdateRequest = false;
        },
        error: err => {
          this.sendingUpdateRequest = false;
          this.ns.notifyError(`Operation failed. ${err.error?.message ?? ''}`);
        }
      });
  }

  postReplyComment(comment: Comment) {
    this.commentReplyData.body = this.commentReplyData.body.trim();
    this.commentReplyData.parentId = comment.id;

    this.sendingReplyRequest = true;

    this.api.post<Comment>('comments', this.commentReplyData)
      .subscribe({
        next: comment => {
          this.comments.unshift(comment);
          this.cancelCommentReply();
          this.sendingReplyRequest = false;

          this.ns.notifySuccess('Comment has been posted.');
        },
        error: err => {
          this.sendingReplyRequest = false;
          this.ns.notifyError(`Operation failed. ${err.error?.message ?? ''}`);
        }
      });
  }

  addGameToCart(game: GameWithDetails) {
    this.cart.addGame(game);
  }
}
