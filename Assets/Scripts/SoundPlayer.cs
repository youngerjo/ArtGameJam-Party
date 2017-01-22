using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour {

	private class SequenceInfo {
		public string name;
		public string[] clipNames;
		public AudioSource audioSource;
		public int currentIndex = 0;
	}

	public static SoundPlayer shared;
	public AudioClip[] audioClips;

	private AudioSource[] audioSources;
	private List<SequenceInfo> sequenceInfos = new List<SequenceInfo>();
	
	private Dictionary<string, SoundGroup> nameToGroups = new Dictionary<string, SoundGroup>();

	void Awake() {
		shared = this;
		audioSources = GetComponentsInChildren<AudioSource>();
	}

	void Update() {
		foreach (SequenceInfo info in sequenceInfos) {
			if (! info.audioSource.isPlaying) {
				info.currentIndex++;
				
			}
		}
	}

	public void StopAllLoop() {
			foreach (AudioSource audioSource in audioSources) {
				if (audioSource.loop && audioSource.name != name) {
					audioSource.Stop();
				}
			}
	}

	public void Play(string name, bool overlapped = false) {

		if (! overlapped) {
			StopAllLoop();
		}
		
		foreach (AudioSource audioSource in audioSources) {
			if (audioSource.clip.name == name) {
				audioSource.Play();
			}
		}
	}

	public void PlayRandomSequence(string groupName, bool looped, bool overlapped = false) {

		if (! nameToGroups.ContainsKey(groupName)) {
			return;
		}

		if (! overlapped) {
			StopAllLoop();
		}

		SoundGroup group = nameToGroups[groupName];
		
		GameObject channel = new GameObject("Audio Source");
		channel.transform.parent = this.gameObject.transform;

		AudioSource source = channel.AddComponent<AudioSource>();

		SequenceInfo info = new SequenceInfo();
		info.name = groupName;
		info.clipNames = group.clipNames;
		info.audioSource = source;
		info.currentIndex = 0;

		sequenceInfos.Add(info);
	}

	public void SetSourceVolume(string name, float volume) {

		foreach (AudioSource audioSource in audioSources) {
			if (audioSource.clip.name == name) {
				audioSource.volume = volume;
			}
		}
	}

	public void RegisterGroup(string name, SoundGroup group) {
		nameToGroups[name] = group;
	}
}
