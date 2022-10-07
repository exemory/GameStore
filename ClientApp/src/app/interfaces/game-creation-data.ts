export interface GameCreationData {
  key: string,
  name: string,
  price: number,
  description: string,
  imageFileName: string,

  genreIds: string[],
  platformTypeIds: string[]
}
