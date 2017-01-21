using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

	public ItemDisplay[] itemDisplays;

	private List<Item> items = new List<Item>();

	public bool AddItem(Item item) {
		items.Add(item);
		
		foreach (ItemDisplay display in itemDisplays) {
			if (display.isOn == false && item.colorType == display.colorType) {
				display.isOn = true;
				return true;
			}
		}

		return false;
	}

	public void RemoveAllItems() {
		items.Clear();
		
		foreach (ItemDisplay display in itemDisplays) {
			display.isOn = false;
		}
	}
}
