using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour {

	private class SequenceInfo {
		public string name;
		public AudioClip[] clips;
		public AudioSource audioSource;
		public bool isPlaying = false;

		public void PlayRandom() {
			isPlaying = true;

			int index = Random.Range(0, clips.Length);
			audioSource.clip = clips[index];
			audioSource.Play();
		}

		public void Stop() {
			isPlaying = false;
			
			audioSource.Stop();
		}
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
			if (info.isPlaying && !info.audioSource.isPlaying) {
				info.PlayRandom();
			}
		}
	}

	public void Stop(string name) {

		foreach (SequenceInfo info in sequenceInfos) {
			if (info.name == name) {
				info.Stop();
				Object.Destroy(info.audioSource.gameObject);
			}
		}

		sequenceInfos.RemoveAll(delegate (SequenceInfo info) {
			return !info.isPlaying;
		});

		foreach (AudioSource audioSource in audioSources) {
			if (audioSource.name == name) {
				audioSource.Stop();
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

	public void PlayRandomSequence(string groupName) {

		if (! nameToGroups.ContainsKey(groupName)) {
			return;
		}

		SoundGroup group = nameToGroups[groupName];
		
		GameObject channel = new GameObject("Audio Source");
		channel.transform.parent = this.gameObject.transform;

		AudioSource source = channel.AddComponent<AudioSource>();
		source.loop = false;
		source.playOnAwake = false;

		List<string> names = new List<string>(group.clipNames);
		List<AudioClip> clips = new List<AudioClip>();

		foreach (string name in names) {
			foreach (AudioClip clip in audioClips) {
				if (clip.name == name) {
					clips.Add(clip);
					break;
				}
			}
		}

		SequenceInfo info = new SequenceInfo();
		info.name = groupName;
		info.audioSource = source;
		info.clips = clips.ToArray();
		info.PlayRandom();

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
