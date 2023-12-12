using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    int currVid;
    int totalVids;
    public VideoClip[] videos;
    VideoPlayer vp;

    private void Awake()
    {
        currVid = 0;
        totalVids = videos.Length;
        vp = GetComponent<VideoPlayer>();
    }

    public void OnNextButton()
    {
        if (currVid + 1 == totalVids)
        {
            currVid = 0;
        }
        else
        {
            currVid++;
        }
        Debug.Log("currvid: " + currVid + " total : " + totalVids);
        Debug.Log(vp.clip);
        vp.clip = videos[currVid];
    }
}
