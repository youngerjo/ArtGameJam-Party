using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notification {

	private string _name;
	private object _userInfo;

	public string name {
		get {
			return _name;
		}
	}

	public object userInfo {
		get {
			return _userInfo;
		}
	}

	public Notification(string name, object userInfo) {
		_name = name;
		_userInfo = userInfo;
	}
}
