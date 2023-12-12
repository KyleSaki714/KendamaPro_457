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

    string[] vidNames = { "tutorial-swingupbigcup2", "rotate_ken", "rotate_basecup", "aroundtheworld" };

    private void Awake()
    {
        currVid = 0;
        totalVids = videos.Length;
        vp = GetComponent<VideoPlayer>();
    }

    public void OnNextButton()
    {
        currVid++;
        string currName = vidNames[currVid % vidNames.Length];
        string currURL = $"https://kylesaki714.github.io/kendamapro_tutorialvids/{currName}.mp4";

        vp.url = currURL;
        Debug.Log(currURL);
    }
}
