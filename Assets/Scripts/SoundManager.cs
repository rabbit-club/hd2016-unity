using UnityEngine;
using System.Collections;
using System.Linq;

public class SoundManager : MonoBehaviour {

	private AudioSource audioSource;
	[SerializeField]
	OggDownloader oggDownloader;

	void Start() {
		audioSource = GetComponent<AudioSource>();
	}

	public void playSound() {
		if (audioSource == null)
			return;
		if (oggDownloader.filePath != null && oggDownloader.filePath != "") {
			audioSource.PlayOneShot(audioSource.clip);
		}
	}
}
