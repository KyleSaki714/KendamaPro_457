using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    Transform canvas;
    TextMeshProUGUI highScoreText;
    TextMeshProUGUI currScoreText;
    TextMeshProUGUI livesText;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").transform;
        highScoreText = canvas.GetChild(0).GetComponent<TextMeshProUGUI>();
        currScoreText = canvas.GetChild(1).GetComponent<TextMeshProUGUI>();
        livesText = canvas.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    // text changing methods

    // https://youtu.be/Y7GjVFFSMuI?si=B4C-MBbe3vMnkzo-
    public void UpdateScore(float scoreTotal)
    {
        currScoreText.text = $"Score: {scoreTotal}";
    }
    public void UpdateHighScore(float scoreTotal)
    {
        highScoreText.text = $"High Score: {scoreTotal}";

    }

}
