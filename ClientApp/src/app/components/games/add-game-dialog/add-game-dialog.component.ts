import {Component, OnInit, ViewEncapsulation} from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";
import {Genre} from "../../../interfaces/genre";
import {PlatformType} from "../../../interfaces/platform-type";
import {HttpClient} from "@angular/common/http";
import {MatDialogRef} from "@angular/material/dialog";
import {NotificationService} from "../../../services/notification.service";
import {forkJoin, Subject} from "rxjs";
import {ImageUploadResult} from "../../../interfaces/image-upload-result";
import {GameCreationData} from "../../../interfaces/game-creation-data";
import {Game} from "../../../interfaces/game";
import {requiredFileValidator} from "../../../shared/validators/required-file-validator";
import {environment as env} from "../../../../environments/environment";

@Component({
  selector: 'app-add-game-dialog',
  templateUrl: './add-game-dialog.component.html',
  styleUrls: ['./add-game-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AddGameDialogComponent implements OnInit {
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
  }, {validators: requiredFileValidator(this.observableFile)});

  selectedGenres = <Genre[]>[];
  selectedPlatformTypes = <PlatformType[]>[];

  loading = true;
  imageUploadingInProgress = false;
  inProgress = false;

  genres!: Genre[];
  platformTypes!: PlatformType[];

  constructor(private fb: FormBuilder,
              private api: HttpClient,
              private dialogRef: MatDialogRef<AddGameDialogComponent>,
              private ns: NotificationService) {
  }

  ngOnInit(): void {
    forkJoin([this.getGenres(), this.getPlatformTypes()])
      .subscribe({
        next: ([genres, platformTypes]) => {
          this.genres = genres;
          this.platformTypes = platformTypes;
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

  onSubmit() {
    if (this.form.invalid) {
      return;
    }

    const data: GameCreationData = {
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

    this.api.post<Game>('games', data)
      .subscribe({
        next: game => {
          this.ns.notifySuccess("Game has been added.");
          this.dialogRef.close(game);
        },
        error: err => {
          this.inProgress = false;
          this.dialogRef.disableClose = false;
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
