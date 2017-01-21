using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationCenter: MonoBehaviour {

	public static NotificationCenter shared;

	private Dictionary<string, List<NotificationHandler>> table = new Dictionary<string, List<NotificationHandler>>();

	void Awake() {
		shared = this;
	}

	public void AddHandler(string notificationName, NotificationHandler handler) {

		List<NotificationHandler> handlers;

		if (table.ContainsKey(notificationName)) {
			handlers = table[notificationName];
		}
		else {
			handlers = new List<NotificationHandler>();
			table[notificationName] = handlers;
		}

		handlers.Add(handler);
	}

	public void RemoveAllHandlers(string notificationName) {
		table.Remove(notificationName);
	}

	public void RemoveHandler(NotificationHandler handler) {

		foreach (string key in table.Keys) {
			List<NotificationHandler> handlers = table[key];
			handlers.Remove(handler);
		}
	}

	public void PostNotification(Notification notification) {
		BroadcastNotification(notification);
	}

	private void BroadcastNotification(Notification notification) {

		Debug.Log("Broadcasting notification: " + notification.name);

		if (!table.ContainsKey(notification.name)) {
			return;
		}

		List<NotificationHandler> handlers = table[notification.name];

		foreach (NotificationHandler handler in handlers) {
			handler(notification);
		}
	}
}
