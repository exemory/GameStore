import {Injectable} from '@angular/core';
import {Game} from "../interfaces/game";
import {BehaviorSubject} from "rxjs";
import {MatDialog, MatDialogRef} from "@angular/material/dialog";
import {CartDialogComponent} from "../components/cart-dialog/cart-dialog.component";
import {CompletionOrderDialogComponent} from "../components/completion-order-dialog/completion-order-dialog.component";

@Injectable({
  providedIn: 'root'
})
export class CartService {

  private readonly cartItemsKey = "cartItems";

  private _items: { game: Game, quantity: number }[] = [];

  public items = new BehaviorSubject(this._items);
  public itemCount = new BehaviorSubject(0);
  public totalPrice = new BehaviorSubject(0);

  private cartDialog?: MatDialogRef<CartDialogComponent>;

  constructor(private dialog: MatDialog) {
    this._items = this.restoreCartItems();
    this.items.next(this._items);

    this.items.subscribe(items => {
      this.storeCartItems(items);
      this.itemCount.next(this.getItemsCount(items));
      this.totalPrice.next(this.getTotalPrice(items));
    });
  }

  private storeCartItems(items: { game: Game, quantity: number }[]) {
    const jsonItems = JSON.stringify(items);

    localStorage.setItem(this.cartItemsKey, jsonItems);
  }

  private restoreCartItems() {
    const items = localStorage.getItem(this.cartItemsKey);

    if (items == null) {
      return;
    }

    return JSON.parse(items);
  }

  private getTotalPrice(items: { game: Game, quantity: number }[]) {
    return items.reduce((total, item) => total + item.quantity * item.game.price, 0);
  }

  private getItemsCount(items: { game: Game, quantity: number }[]) {
    return items.reduce((sum, item) => sum + item.quantity, 0);
  }

  private getItemByGameId(gameId: string) {
    return this._items.find(g => g.game.id === gameId);
  }

  get isEmpty() {
    return this.itemCount.value === 0;
  }

  addGame(game: Game) {
    const item = this.getItemByGameId(game.id);

    if (item) {
      this.updateQuantity(item.game, item.quantity + 1);
      return
    }

    this._items.push({game, quantity: 1});
    this.items.next(this._items);
  }

  removeGame(game: Game) {
    const index = this._items.findIndex(i => i.game.id === game.id);
    if (index > -1) {
      this._items.splice(index, 1);
    }

    this.items.next(this._items);
  }

  updateQuantity(game: Game, quantity: number) {
    if (quantity <= 0 || quantity > 100) {
      return;
    }

    const item = this.getItemByGameId(game.id)

    if (item) {
      item.quantity = quantity;
    } else {
      this._items.push({game, quantity});
    }

    this.items.next(this._items);
  }

  increaseQuantity(game: Game, value: number) {
    const item = this.getItemByGameId(game.id);

    if (!item) {
      return;
    }

    this.updateQuantity(item.game, item.quantity + value);
  }

  getItem(game: Game) {
    return this.getItemByGameId(game.id);
  }

  clear() {
    this._items = [];
    this.items.next(this._items);
  }

  openCartDialog() {
    this.cartDialog = this.dialog.open(CartDialogComponent,
      {
        maxWidth: '550px',
        width: '100%',
        height: '100%',
        position: {right: "right"},
        autoFocus: false
      });
  }

  openCompletionOrderDialog() {
    const x = this.dialog.open(CompletionOrderDialogComponent,
      {
        maxWidth: '400px',
        width: '100%',
        autoFocus: false
      }
    );

    x.afterClosed().subscribe({
      next: orderConfirmed => {
        if (!orderConfirmed) {
          return;
        }

        this.clear();
        this.cartDialog?.close();
      }
    })
  }
}
