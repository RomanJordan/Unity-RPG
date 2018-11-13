using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {

    public Sound[] sounds;

	// Use this for initialization
	void Awake () {
		foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
	}

    void Start () {
        //Gettting the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        //Storing the current scene in a string
        string sceneName = currentScene.name;

        //Whatever the current scene is... play the appropriate overworld theme
        if (sceneName == "PalletTown") {
            Play("PalletTownTheme01");
        }
    }
	
	public void Play(string sound) {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }
}
