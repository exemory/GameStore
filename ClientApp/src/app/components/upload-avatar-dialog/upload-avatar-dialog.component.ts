import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {MatDialogRef} from "@angular/material/dialog";
import {NotificationService} from "../../services/notification.service";
import {environment as env} from "../../../environments/environment"

@Component({
  selector: 'app-upload-avatar-dialog',
  templateUrl: './upload-avatar-dialog.component.html',
  styleUrls: ['./upload-avatar-dialog.component.scss']
})
export class UploadAvatarDialogComponent implements OnInit {

  image?: File;
  imageSrc?: string | ArrayBuffer | null;

  inProgress = false;

  constructor(private api: HttpClient,
              private dialogRef: MatDialogRef<UploadAvatarDialogComponent>,
              private ns: NotificationService) {
  }

  ngOnInit(): void {
  }

  onFileSelect(event: any) {
    if (!event.target.files.length) {
      return;
    }

    const file = event.target.files.item(0) as File;

    this.onFileInput(file);
  }

  onFileInput(file: File) {
    if (!file) {
      return;
    }

    const hasExtension = file.name.includes('.');
    const extension = file.name.split('.').pop();

    if (!hasExtension || !extension || !env.supportedImageExtensions.includes(extension)) {
      this.ns.notifyError('Image format is unsupported.');
      return;
    }

    this.image = file;

    this.updateImagePreview();
  }

  private updateImagePreview() {
    if (!this.image) {
      return;
    }

    const reader = new FileReader();
    reader.onload = e => this.imageSrc = reader.result;

    reader.readAsDataURL(this.image);
  }

  upload() {
    if (!this.image) {
      return;
    }

    const formData = new FormData();
    formData.append('file', this.image, this.image.name);

    this.inProgress = true;
    this.dialogRef.disableClose = true;

    this.api.post('avatar', formData)
      .subscribe({
        next: () => {
          this.dialogRef.close(true);

          this.ns.notifySuccess('Avatar has been changed.');
        },
        error: err => {
          this.inProgress = false;
          this.dialogRef.disableClose = false;

          this.ns.notifyError(`Uploading file failed. ${err.error?.message ?? ''}`);
        }
      });
  }

  onFileDropped(fileList: FileList) {
    if (!fileList.length) {
      return;
    }

    this.onFileInput(fileList[0]);
  }
}
