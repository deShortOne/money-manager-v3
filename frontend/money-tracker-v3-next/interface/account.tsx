
export interface Account {
    id: number,
    name: string,
    doesUserOwnAccount: boolean,
}

export interface NewAccountDto {
    name: string,
    doesUserOwnAccount: boolean,
}
