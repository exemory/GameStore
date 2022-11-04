import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {HttpClient, HttpStatusCode} from "@angular/common/http";
import {GameWithDetails} from "../../interfaces/game-with-details";
import {NotificationService} from "../../services/notification.service";
import {Game} from "../../interfaces/game";
import {environment as env} from "../../../environments/environment";
import {Comment} from "../../interfaces/comment";
import {forkJoin} from "rxjs";
import {FormBuilder, Validators} from "@angular/forms";
import {CommentCreationData} from "../../interfaces/comment-creation-data";
import {AuthService} from "../../services/auth.service";
import {CommentUpdateData} from "../../interfaces/comment-update-data";

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

  sendingComment = false;
  createCommentForm = this.fb.group({
    body: ['', Validators.maxLength(600)]
  });

  editingComment?: Comment;
  editCommentForm = this.fb.group({
    body: ['', Validators.maxLength(600)]
  });
  savingComment = false;

  replyingComment?: Comment;
  replyCommentForm = this.fb.group({
    body: ['', Validators.maxLength(600)]
  });
  sendingReplyComment = false;

  constructor(private route: ActivatedRoute,
              private api: HttpClient,
              private ns: NotificationService,
              private router: Router,
              private fb: FormBuilder,
              private auth: AuthService) {
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

  getFullUserNameFromComment(comment: Comment) {
    return `${comment.userInfo.firstName} ${comment.userInfo.lastName}`;
  }

  isCurrentUserAuthorIfComment(comment: Comment) {
    return comment.userInfo.username === this.auth.session?.userInfo.username;
  }

  getCommentUserAvatarUrl(comment: Comment) {
    return comment.userInfo.hasAvatar ? `${env.apiUrl}avatar?username=${comment.userInfo.username}` :
      'assets/default-user-avatar.png';
  }

  isCommentDeleted(comment: Comment) {
    return this.deletedComments.includes(comment);
  }

  createCommentSubmit() {
    if (this.createCommentForm.invalid) {
      return;
    }

    const formValue = this.createCommentForm.value;

    if (!formValue.body?.trim()) {
      return;
    }

    const data: CommentCreationData = {
      gameId: this.game.id,
      body: formValue.body?.trim()
    }

    this.sendingComment = true;

    this.api.post<Comment>('comments', data)
      .subscribe({
        next: comment => {
          this.comments.unshift(comment);
          this.createCommentForm.reset();
          this.sendingComment = false;

          this.ns.notifySuccess('Comment has been added.');
        },
        error: err => {
          this.sendingComment = false;
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
    this.editCommentForm.patchValue({
      body: comment.body
    });
    this.editingComment = comment;

    setTimeout(() => this.editCommentBody.nativeElement.focus());
  }

  cancelCommentEditing() {
    this.editingComment = undefined;
  }

  replyComment(comment: Comment) {
    this.cancelCommentEditing();
    this.replyingComment = comment;

    setTimeout(() => this.replyCommentBody.nativeElement.focus());
  }

  cancelCommentReply() {
    this.replyCommentForm.reset();
    this.replyingComment = undefined;
  }

  isCommentEditing(comment: Comment) {
    return comment === this.editingComment;
  }

  isCommentReplying(comment: Comment) {
    return comment === this.replyingComment;
  }

  saveEditedComment() {
    if (this.editCommentForm.invalid || !this.editingComment) {
      return;
    }

    const formValue = this.editCommentForm.value;

    if (!formValue.body?.trim()) {
      return;
    }

    const data: CommentUpdateData = {
      body: formValue.body?.trim()
    }

    this.savingComment = true;

    this.api.put<Comment>(`comments/${this.editingComment!.id}`, data)
      .subscribe({
        next: () => {
          this.editingComment!.body = data.body;
          this.cancelCommentEditing();
          this.savingComment = false;
        },
        error: err => {
          this.savingComment = false;
          this.ns.notifyError(`Operation failed. ${err.error?.message ?? ''}`);
        }
      });
  }

  postReplyComment(comment: Comment) {
    if (this.replyCommentForm.invalid) {
      return;
    }

    const formValue = this.replyCommentForm.value;

    if (!formValue.body?.trim()) {
      return;
    }

    const data: CommentCreationData = {
      gameId: this.game.id,
      body: formValue.body?.trim(),
      parentId: comment.id
    }

    this.sendingReplyComment = true;

    this.api.post<Comment>('comments', data)
      .subscribe({
        next: comment => {
          this.comments.unshift(comment);
          this.cancelCommentReply();
          this.sendingReplyComment = false;

          this.ns.notifySuccess('Comment has been added.');
        },
        error: err => {
          this.sendingReplyComment = false;
          this.ns.notifyError(`Operation failed. ${err.error?.message ?? ''}`);
        }
      });
  }
}
