import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  ViewChild
} from '@angular/core';
import {CdkDragDrop, moveItemInArray} from "@angular/cdk/drag-drop";
import {MatChipInputEvent} from "@angular/material/chips";
import {MatAutocompleteSelectedEvent} from "@angular/material/autocomplete";
import {FormControl} from "@angular/forms";
import {COMMA, ENTER} from "@angular/cdk/keycodes";

@Component({
  selector: 'app-chips-input',
  templateUrl: './chips-input.component.html',
  styleUrls: ['./chips-input.component.scss']
})
export class ChipsInputComponent<Item> implements OnInit, OnChanges {

  @Input() items!: Item[];
  @Input() nameProperty!: string;
  @Input() label!: string;
  @Input() placeholder!: string;

  @Output() selectionChanged = new EventEmitter<Item[]>();

  @ViewChild('input') input?: ElementRef<HTMLInputElement>;

  separatorKeysCodes: number[] = [ENTER, COMMA];

  selectedItems = <Item[]>[];
  filteredItems!: Item[];
  inputControl = new FormControl('');

  constructor() {
    this.inputControl.valueChanges.subscribe(
      {
        next: () => {
          this.filterItems();
        }
      }
    );
  }

  ngOnInit(): void {
  }

  onSelectedItemsChanged() {
    this.filterItems();
    this.selectionChanged.emit(this.selectedItems);
  }

  ngOnChanges(changes: SimpleChanges) {
    if (this.items) {
      this.filterItems();
    }
  }

  getItemName(item: Item) {
    // @ts-ignore
    return item[this.nameProperty];
  }

  onRemove(item: Item) {
    const index = this.selectedItems.indexOf(item);

    if (index >= 0) {
      this.selectedItems.splice(index, 1);
    }

    this.onSelectedItemsChanged();
  }

  onAdd(event: MatChipInputEvent) {
    const value = event.value?.trim().toLowerCase();

    if (!value) {
      return;
    }

    const item = this.items.find(i => this.getItemName(i).toLowerCase() === value);

    if (!item) {
      return;
    }

    this.addItemToSelected(item);
  }

  onSelect(event: MatAutocompleteSelectedEvent): void {
    this.addItemToSelected(event.option.value);
  }

  private addItemToSelected(item: Item) {
    const alreadyExists = this.selectedItems.includes(item);

    if (alreadyExists) {
      return;
    }

    this.selectedItems.push(item);

    if (this.input) {
      this.input.nativeElement.value = '';
    }
    this.inputControl.setValue(null);

    this.onSelectedItemsChanged();
  }

  private filterItems() {
    this.filteredItems = this.items.filter(i => !this.selectedItems.includes(i));

    const filterValue = this.input?.nativeElement.value?.trim().toLowerCase();

    if (!filterValue) {
      return;
    }

    this.filteredItems = this.filteredItems.filter(i => this.getItemName(i).toLowerCase().includes(filterValue));
  }

  drop(event: CdkDragDrop<Item[]>) {
    moveItemInArray(this.selectedItems, event.previousIndex, event.currentIndex);
    this.onSelectedItemsChanged();
  }
}
