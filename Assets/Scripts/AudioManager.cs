using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<AudioClip> cupSfx = new List<AudioClip>();
    public List<AudioClip> failSfx = new List<AudioClip>();
    public AudioClip natureAmbience; // https://freesound.org/people/klankbeeld/sounds/528752/
    AudioSource bgmPlayer;
    AudioSource sfxPlayer;

    private void Awake()
    {
        bgmPlayer = transform.GetChild(0).GetComponent<AudioSource>();
        sfxPlayer = transform.GetChild(1).GetComponent<AudioSource>();
        //sfxPlayer.spatialBlend = 1;
    }

    private void Start()
    {
        bgmPlayer.clip = natureAmbience;
        bgmPlayer.Play();
    }

    public void PlayCup()
    {
        sfxPlayer.clip = cupSfx[Random.Range(0, cupSfx.Count)];
        sfxPlayer.Play();
    }

    public void PlayFail()
    {
        sfxPlayer.clip = failSfx[Random.Range(0, failSfx.Count)];
        sfxPlayer.Play();
    }
}
