using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour {

	public static SoundPlayer shared;

	private AudioSource[] audioSources;

	void Awake() {
		shared = this;
		audioSources = GetComponentsInChildren<AudioSource>();
	}

	public void Play(string name, bool overlapped = false) {

		if (! overlapped) {
			foreach (AudioSource audioSource in audioSources) {
				if (audioSource.loop && audioSource.name != name) {
					audioSource.Stop();
				}
			}
		}
		
		foreach (AudioSource audioSource in audioSources) {
			if (audioSource.clip.name == name) {
				audioSource.Play();
			}
		}
	}

	public void SetSourceVolume(string name, float volume) {

		foreach (AudioSource audioSource in audioSources) {
			if (audioSource.clip.name == name) {
				audioSource.volume = volume;
			}
		}
	}
}
