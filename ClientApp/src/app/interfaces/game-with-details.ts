export interface GameWithDetails {
  id: string,
  key: string,
  name: string,
  price: number,
  description: string,

  genres: string[],
  platformTypes: string[];
}
