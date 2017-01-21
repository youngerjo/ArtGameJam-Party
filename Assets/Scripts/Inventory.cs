using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

	public ItemDisplay[] itemDisplays;

	private List<Item> items = new List<Item>();

	public void AddItem(Item item) {
		items.Add(item);
		UpdateDisplays();
	}

	public void RemoveAllItems() {
		items.Clear();
		UpdateDisplays();
	}

	private void UpdateDisplays() {

		foreach (ItemDisplay display in itemDisplays) {
			display.isOn = false;
		}

		foreach (Item item in items) {
			foreach (ItemDisplay display in itemDisplays) {
				if (item.colorType == display.colorType) {
					display.isOn = true;
				}
			}
		}
	}
}
