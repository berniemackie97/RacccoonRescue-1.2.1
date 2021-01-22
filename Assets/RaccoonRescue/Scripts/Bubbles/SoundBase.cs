using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
[RequireComponent(typeof(AudioSource))]
public class SoundBase : MonoBehaviour
{
	public static SoundBase Instance;

	public AudioMixer mixer;

	public AudioClip click;
	public AudioClip[] powerup_click;
	public AudioClip powerup_fill;
	public AudioClip[] combo;
	public AudioClip[] swish;
	public AudioClip bug;
	public AudioClip bugDissapier;
	public AudioClip pops;
	public AudioClip boiling;
	public AudioClip hit;
	public AudioClip kreakWheel;
	public AudioClip spark;
	public AudioClip winSound;
	public AudioClip gameOver;
	public AudioClip scoringStar;
	public AudioClip scoring;
	public AudioClip alert;
	public AudioClip aplauds;
	public AudioClip OutOfMoves;
	public AudioClip Boom;
	public AudioClip black_hole;
	public AudioClip coins;
	public AudioClip lightning;

	public AudioClip[] baby;
	public AudioClip[] character;
	public AudioClip bubble_cannon;
	public AudioClip leaf;
	public AudioClip wave;

	//SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.click);

	// SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.baby[Random.Range(0, SoundBase.Instance.baby.Length)]);
	//SoundBase.Instance.PlaySound(SoundBase.Instance.timeOut);
	AudioSource audioSource;

	List<AudioClip> clipsPlaying = new List<AudioClip>();

	void Awake()
	{

		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
		DontDestroyOnLoad(gameObject);
		audioSource = GetComponent<AudioSource>();
	}

	public void PlaySound(AudioClip clip)
	{
		audioSource.PlayOneShot(clip);
	}

	public void PlaySoundsRandom(AudioClip[] clip)
	{
		SoundBase.Instance.PlaySound(clip[Random.Range(0, clip.Length)]);
	}

	public void PlayLimitSound(AudioClip clip)
	{
		if (clipsPlaying.IndexOf(clip) < 0) {
			clipsPlaying.Add(clip);
			PlaySound(clip);
			StartCoroutine(WaitForCompleteSound(clip));
		}
	}

	IEnumerator WaitForCompleteSound(AudioClip clip)
	{
		yield return new WaitForSeconds(0.01f);
		clipsPlaying.Remove(clipsPlaying.Find(x => clip));
	}

}
