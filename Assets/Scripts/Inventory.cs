using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

	public ItemDisplay[] itemDisplays;

    public int playernum;
    public int collecteditemnum = 0;

	private List<Item> items = new List<Item>();

	public bool AddItem(Item item) {
		items.Add(item);
		
		foreach (ItemDisplay display in itemDisplays) {
			if (display.isOn == false && item.colorType == display.colorType) {
				display.isOn = true;
                collecteditemnum++;
                if (collecteditemnum == 3)
                {
                    Notification notification = new Notification("DoorOn", playernum);
                    NotificationCenter.shared.PostNotification(notification);
                }
				return true;
			}
		}

		return false;
	}

	public void RemoveAllItems() {
		items.Clear();
        collecteditemnum = 0;
		foreach (ItemDisplay display in itemDisplays) {
			display.isOn = false;
		}
	}
}
