
export interface Result<T> {
    hasError: boolean;
    errorMessage: string;
    hasFailed: boolean;
    item?: T;
}

export class ErrorResult<T> implements Result<T> {
    hasError = true;
    constructor(public errorMessage: string, public hasFailed: boolean) {}
}

export class SuccessResult<T> implements Result<T> {
    hasError = false;
    errorMessage = "";
    hasFailed = false;
    constructor(public item: T) {}
}
