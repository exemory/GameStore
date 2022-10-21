import {Component, Inject, OnInit, ViewEncapsulation} from '@angular/core';
import {forkJoin, Subject} from "rxjs";
import {FormBuilder, Validators} from "@angular/forms";
import {Genre} from "../../../interfaces/genre";
import {PlatformType} from "../../../interfaces/platform-type";
import {HttpClient} from "@angular/common/http";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {NotificationService} from "../../../services/notification.service";
import {Game} from "../../../interfaces/game";
import {ImageUploadResult} from "../../../interfaces/image-upload-result";
import {GameWithDetails} from "../../../interfaces/game-with-details";
import {GameUpdateData} from "../../../interfaces/game-update-data";
import {environment as env} from "../../../../environments/environment";

@Component({
  selector: 'app-edit-game-dialog',
  templateUrl: './edit-game-dialog.component.html',
  styleUrls: ['./edit-game-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class EditGameDialogComponent implements OnInit {
  image?: File;
  imageId?: string;
  observableFile = new Subject<File | undefined>();

  readonly minPrice = 0;
  readonly maxPrice = 1000;

  form = this.fb.group({
    key: ['', Validators.pattern(/^[a-z0-9-]*$/i)],
    name: [''],
    price: ['', [Validators.required, Validators.min(this.minPrice), Validators.max(this.maxPrice)]],
    description: ['']
  });

  selectedGenres = <Genre[]>[];
  selectedPlatformTypes = <PlatformType[]>[];

  loading = true;
  imageUploadingInProgress = false;
  inProgress = false;

  genres!: Genre[];
  platformTypes!: PlatformType[];

  game!: GameWithDetails;

  constructor(@Inject(MAT_DIALOG_DATA) public gameToEdit: Game,
              private fb: FormBuilder,
              private api: HttpClient,
              private dialogRef: MatDialogRef<EditGameDialogComponent>,
              private ns: NotificationService) {
  }

  ngOnInit(): void {
    forkJoin([this.getGenres(), this.getPlatformTypes(), this.getGameWithDetails(this.gameToEdit.key)])
      .subscribe({
        next: ([genres, platformTypes, game]) => {
          this.genres = genres;
          this.platformTypes = platformTypes;
          this.game = game;

          this.form.setValue({
            key: game.key,
            name: game.name,
            price: game.price.toString(),
            description: game.description
          });

          this.selectedGenres = this.genres.filter(g => game.genres.includes(g.name));
          this.selectedPlatformTypes = this.platformTypes.filter(p => game.platformTypes.includes(p.type));

          this.loading = false;
        }, error: err => {
          this.ns.notifyError(`Loading data failed. ${err.error?.message ?? ''}`, true);
          this.dialogRef.close();
        }
      });
  }

  private getGenres() {
    return this.api.get<Genre[]>('genres');
  }

  private getPlatformTypes() {
    return this.api.get<PlatformType[]>('platformTypes');
  }

  private getGameWithDetails(gameKey: string) {
    return this.api.get<GameWithDetails>(`games/${gameKey}`);
  }

  onSubmit() {
    if (this.form.invalid) {
      return;
    }

    const data: GameUpdateData = {
      key: this.form.get('key')?.value!,
      name: this.form.get('name')?.value!,
      price: +this.form.get('price')?.value!,
      description: this.form.get('description')?.value!,
      imageFileName: this.imageId!,

      genreIds: this.selectedGenres.map(g => g.id),
      platformTypeIds: this.selectedPlatformTypes.map(p => p.id)
    }

    this.dialogRef.disableClose = true;
    this.inProgress = true;

    this.api.put<Game>(`games/${this.game.id}`, data)
      .subscribe({
        next: () => {
          this.ns.notifySuccess("Game has been updated.");
          this.dialogRef.close({
            data,
            genres: this.selectedGenres,
            platforms: this.selectedPlatformTypes
          });
        },
        error: err => {
          this.inProgress = false;
          this.dialogRef.disableClose = false;
          console.log(err);
          this.ns.notifyError(`Operation failed. ${err.error?.message ?? ''}`);
        }
      })
  }

  handleFileInput(event: any) {
    const file = event.target.files.item(0);

    if (!file) {
      return;
    }

    const hasExtension = file.name.includes('.');
    const extension = file.name.split('.').pop();

    if (!hasExtension || !extension || !env.supportedImageExtensions.includes(extension)) {
      this.ns.notifyError('Image format is unsupported.');
      return;
    }

    const formData = new FormData();
    formData.append('file', file, file.name);

    this.imageUploadingInProgress = true;

    this.api.post<ImageUploadResult>('games/images', formData)
      .subscribe({
        next: result => {
          this.image = file;
          this.imageId = result.imageFileName;
          this.imageUploadingInProgress = false;

          this.observableFile.next(file);
          this.form.updateValueAndValidity();
        },
        error: err => {
          this.image = undefined;
          this.imageId = undefined;
          this.imageUploadingInProgress = false;

          this.ns.notifyError(`Uploading file failed. ${err.error?.message ?? ''}`);

          this.observableFile.next(undefined);
          this.form.updateValueAndValidity();
        }
      });
  }

  get priceOutOfRangeError() {
    return `Price must be between ${this.minPrice} and ${this.maxPrice}`;
  }

  onGenreSelectionChanged(genres: Genre[]) {
    this.selectedGenres = genres;
  }

  onPlatformsSelectionChanged(platformTypes: PlatformType[]) {
    this.selectedPlatformTypes = platformTypes;
  }
}
