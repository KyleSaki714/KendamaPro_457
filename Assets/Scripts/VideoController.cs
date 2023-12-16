using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    int currVid;
    VideoPlayer vp;
    public TextMeshProUGUI caption;

    string[] vidNames = { "tutorial-swingupbigcup2", "rotate_ken", "rotate_basecup", "aroundtheworld" };
    string[] captions = { "Big cup", "Rotation", "Base cup", "Around the World" };

    private void Awake()
    {
        currVid = 0;
        vp = GetComponent<VideoPlayer>();
    }

    public void OnNextButton()
    {
        currVid++;
        string currName = vidNames[currVid % vidNames.Length];
        string currURL = $"https://kylesaki714.github.io/kendamapro_tutorialvids/{currName}.mp4";

        vp.url = currURL;
        caption.text = captions[currVid % vidNames.Length];
        Debug.Log(currURL);
    }
}