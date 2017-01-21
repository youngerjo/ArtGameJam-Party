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

	public void Play(string name) {

		foreach (AudioSource audioSource in audioSources) {
			if (audioSource.loop && audioSource.name != name) {
				audioSource.Stop();
			}
		}
		
		foreach (AudioSource audioSource in audioSources) {
			if (audioSource.clip.name == name) {
				audioSource.Play();
			}
		}
	}
}
