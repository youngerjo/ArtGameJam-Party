using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundGroup {
	
	private string[] _clipNames;

	public string[] clipNames {
		get {
			return _clipNames;
		}
	}

	public SoundGroup(params string[] clipNames) {
		_clipNames = clipNames;
	}

	string[] GetRandomSequence() {

		List<string> names = new List<string>(clipNames);
		List<string> shuffled = new List<string>();

		while (names.Count > 0) {

			int index = Random.Range(0, names.Count);
			
			string name = names[index];
			shuffled.Add(name);

			names.RemoveAt(index);
		}

		return shuffled.ToArray();
	}
}
