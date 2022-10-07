import {Observable} from "rxjs";
import {AbstractControl, ValidationErrors, ValidatorFn} from "@angular/forms";

export function requiredFileValidator(observableFile: Observable<File | undefined>): ValidatorFn {
  let file: File | undefined;

  observableFile.subscribe({
    next: newFile => {
      file = newFile;
    }
  })

  return (form: AbstractControl): ValidationErrors | null => {
    return !!file ? null : {fileRequired: true};
  };
}
