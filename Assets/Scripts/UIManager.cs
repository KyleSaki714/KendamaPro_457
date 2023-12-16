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
    TextMeshProUGUI chainText;

    GameObject PauseMenu;

    GameObject _splashTextBillboard;
    [SerializeField]
    private float fadeSpeed = 0.31f;
    private Vector3 splashOffset = new Vector3 (3.68f, 3.97f, 0f);

    [SerializeField]
    List<Sprite> sprites = new List<Sprite>();

    // for drawing the string
    [SerializeField] private Transform[] points;
    [SerializeField] private LineController line;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").transform;
        highScoreText = canvas.GetChild(0).GetComponent<TextMeshProUGUI>();
        currScoreText = canvas.GetChild(1).GetComponent<TextMeshProUGUI>();
        livesText = canvas.GetChild(2).GetComponent<TextMeshProUGUI>();
        chainText = canvas.GetChild(3).GetComponent<TextMeshProUGUI>();
        PauseMenu = canvas.GetChild(4).gameObject;
        PauseMenu.SetActive(false);

        _splashTextBillboard = transform.GetChild(0).gameObject;
        _splashTextBillboard.SetActive(false);

    }

    void Start()
    {
        line.SetUpLine(points);
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

    public void UpdateTrickChain(List<string> tricks, int trickMultiplier)
    {
        string res = "[ ";

        if (tricks.Count > 0)
        {
            res += tricks[0] + " ( " + GetCupScore(tricks[0]) + " ) ";

        }

        if (tricks.Count > 1) 
        {
            for (int i = 1;  i < tricks.Count; i++)
            {
                res += "+ " + tricks[i] + " ( " + GetCupScore(tricks[i]) + " ) ";
            }

        }

        res += "] x " + trickMultiplier;

        chainText.text = res;

    }

    int GetCupScore(string cupName)
    {
        return cupName switch
        {
            "BigCup" => 100,
            "SmallCup" => 150,
            "BaseCup" => 125,
            _ => 0,
        };
    }

    public void ShowPauseMenu()
    {
        PauseMenu.SetActive(true);
    }
    public void HidePauseMenu()
    {
        PauseMenu.SetActive(false);
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
