using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour {

	public static SoundManager Instance;

	public AudioClip[] clips;
	private AudioSource[] sources;

	void Awake() {
		Instance = this;

		const float vol_music = 0.25f;
		const float vol_sfx = 0.25f;

		sources = new AudioSource[clips.Length];
		for (int i = 0; i < clips.Length; i++) {
			sources[i] = gameObject.AddComponent<AudioSource>();
			sources[i].clip = clips[i];
			sources[i].volume = vol_sfx;
			sources[i].playOnAwake = false;
		}
		sources[0].volume = vol_music;
		sources[0].loop = true;
		sources[0].Play();
	}

	public void SetMusicVol(Slider s) {
		sources[0].volume = s.value / 2;
	}

	public void SetSfxVol(Slider s) {
		for (int i = 1; i < sources.Length; i++) {
			sources[i].volume = s.value / 2;
		}
	}

	public void PlayGoldDing() {
		sources[1].Play();
	}

	public void PlayCrash(float x) {
		sources[2].panStereo = x;
		sources[2].Play();
	}

}
