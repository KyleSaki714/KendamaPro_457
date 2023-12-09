using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //public List<AudioSource> cupSfx = new List<AudioSource>();
    //public List<AudioSource> failSfx = new List<AudioSource>();

    public AudioClip cupClip;
    AudioSource cupTest;

    // Start is called before the first frame update
    void Start()
    {
        cupTest = gameObject.AddComponent<AudioSource>();
        cupTest.clip = cupClip;
    }

    public void PlayCup()
    {
        cupTest.Play();
        //cupSfx[Random.Range(0, cupSfx.Count)].Play();
    }

    public void PlayFail()
    {

    }
}
