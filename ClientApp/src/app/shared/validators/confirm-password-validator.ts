import {AbstractControl, ValidationErrors, ValidatorFn} from "@angular/forms";

export function confirmPasswordValidator(passwordField: string, confirmPasswordField: string): ValidatorFn {
  return (form: AbstractControl): ValidationErrors | null => {
    const password = form.get(passwordField)?.value;
    const confirmPassword = form.get(confirmPasswordField)?.value;
    return password === confirmPassword ? null : {passwordMismatch: true};
  };
}
