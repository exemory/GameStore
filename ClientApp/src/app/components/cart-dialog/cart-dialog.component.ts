import {Component, OnInit} from '@angular/core';
import {MatDialogRef} from "@angular/material/dialog";
import {CartService} from "../../services/cart.service";
import {Game} from "../../interfaces/game";
import {environment as env} from "../../../environments/environment";
import {map} from "rxjs";

@Component({
  selector: 'app-cart',
  templateUrl: './cart-dialog.component.html',
  styleUrls: ['./cart-dialog.component.scss']
})
export class CartDialogComponent implements OnInit {

  cartTotalPrice = this.cart.items.pipe(
    map(i => i.reduce((total, item) => total + item.quantity * item.game.price, 0))
  );

  constructor(private dialogRef: MatDialogRef<CartDialogComponent>,
              public cart: CartService) {
  }

  ngOnInit(): void {
  }

  getGameImageUrl(game: Game): string {
    return `${env.apiUrl}games/${game.key}/image`;
  }

  closeCart() {
    this.dialogRef.close();
  }

  increaseQuantity(game: Game, value: number) {
    this.cart.increaseQuantity(game, value);
  }

  checkout() {
    this.cart.openCompletionOrderDialog();
  }
}
