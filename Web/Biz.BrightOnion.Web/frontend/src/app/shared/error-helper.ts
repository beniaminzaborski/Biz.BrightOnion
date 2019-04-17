export class ErrorHelper {

  public static getErrorMessage(error: any): string {
    // console.log('error', error.error.message);
    let errorMessage: string = '';
    if (error.error.message)
      errorMessage = error.error.message;
    else if (error.statusText)
      errorMessage = error.statusText;
    else if (error.error.message)
      errorMessage = error.error.message;

    if (error.status == 403)
      errorMessage = 'You do not have enought priviliges';

    return errorMessage;
  }

}
