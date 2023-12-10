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

    GameObject _splashTextBillboard;
    [SerializeField]
    private float fadeSpeed = 0.31f;
    private Vector3 splashOffset = new Vector3 (3.68f, 3.97f, 0f);

    [SerializeField]
    List<Sprite> sprites = new List<Sprite>();

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").transform;
        highScoreText = canvas.GetChild(0).GetComponent<TextMeshProUGUI>();
        currScoreText = canvas.GetChild(1).GetComponent<TextMeshProUGUI>();
        livesText = canvas.GetChild(2).GetComponent<TextMeshProUGUI>();

        _splashTextBillboard = transform.GetChild(0).gameObject;
        _splashTextBillboard.SetActive(false);
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

    public void ShowSplashText()
    {
        StartCoroutine(ShowSplashHelper());
    }

    IEnumerator ShowSplashHelper()
    {


        // place billboard at ken position
        _splashTextBillboard.transform.position = GameManager.Instance.kenTransform.position + splashOffset;
        
        _splashTextBillboard.SetActive(true);

        // select and set random image
        _splashTextBillboard.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Count)];

        yield return new WaitForSeconds(0.3f);


        Color oldCOlor = _splashTextBillboard.GetComponent<SpriteRenderer>().color;

        while (_splashTextBillboard.GetComponent<SpriteRenderer>().color.a > 0)
        {
            // reduce transparency
            // https://owlcation.com/stem/How-to-fade-out-a-GameObject-in-Unity
            Color objColor = _splashTextBillboard.GetComponent<SpriteRenderer>().color;
            float fadeAmount = objColor.a - (fadeSpeed);
            objColor = new Color(objColor.r, objColor.g, objColor.b, fadeAmount);
            _splashTextBillboard.GetComponent<SpriteRenderer>().color = objColor;

            yield return new WaitForSeconds(0.01f);
        }

        _splashTextBillboard.SetActive(false);

        _splashTextBillboard.GetComponent<SpriteRenderer>().color = oldCOlor;
    }
}
