using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{

    // references
    public Transform kenTransform;
    public Transform environmentContainer;

    public static GameManager Instance { get; private set; }
    public AudioManager AudioManager { get; private set; }
    public UIManager UIManager { get; private set; }

    public bool isPaused = false;

    // game logic!
    [SerializeField]
    bool isChaining;
    [SerializeField]
    bool tamaIsInAir;
    int currentScore;
    int currentHighScore;

    List<string> currentTrickChain;


    // rotations
    [SerializeField]
    float currentRot;
    [SerializeField]
    float initialRot;
    [SerializeField]
    float changeInRot;
    [SerializeField]
    int currScoreMultiplier;
    [SerializeField]
    bool fullRotPossible = true;


    private void Awake()
    {
        // singleton
        // https://gamedevbeginner.com/singletons-in-unity-the-right-way/
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        else
        {
            Instance = this;
            AudioManager = GetComponentInChildren<AudioManager>();
            UIManager = GetComponentInChildren<UIManager>();
        }

        kenTransform = GameObject.Find("Ken").transform;
        environmentContainer = GameObject.Find("Environment").transform;
    }

    private void Start()
    {
        isChaining = false;
        tamaIsInAir = true;
        fullRotPossible = true;

        currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
        UIManager.UpdateHighScore(currentHighScore);

        currentTrickChain = new List<string>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Instance.isPaused = !Instance.isPaused;
            PauseGame();
        }
    }

    // https://gamedevbeginner.com/the-right-way-to-pause-the-game-in-unity/
    void PauseGame()
    {
        if (Instance.isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }


    // subscriptions

    private void OnEnable()
    {
        TamaPhysics.OnInAir += HandleTamaInAir;
        TamaPhysics.OnCupLand += HandleScoreChange;
        TamaPhysics.OnFail += HandleFailTrick;
        //KenController.OnKenRotating += KenController_OnKenRotating;
    }

    private void OnDisable()
    {
        TamaPhysics.OnInAir -= HandleTamaInAir;
        TamaPhysics.OnCupLand -= HandleScoreChange;
        TamaPhysics.OnFail -= HandleFailTrick;
        //KenController.OnKenRotating -= KenController_OnKenRotating;
    }

    //private void KenController_OnKenRotating(float rotValue)
    //{
    //    currentRot = rotValue;
    //    if (tamaIsInAir)
    //    {
    //        changeInRot = Mathf.Abs(currentRot - initialRot);
    //        CheckFullRotation();
    //    }
    //}

    //void CheckFullRotation()
    //{
    //    if (fullRotPossible && changeInRot > 330f)
    //    {
    //        currScoreMultiplier++;
    //        fullRotPossible = false;
    //    }

    //    if (Mathf.Abs(currentRot) < 30f)
    //    {
    //        fullRotPossible = true;
    //    }

    //}

    void HandleTamaInAir(bool isInAir)
    {
        tamaIsInAir = isInAir;
        //if (tamaIsInAir)
        //{
        //    initialRot = currentRot;
        //}
        //else
        //{
        //    currScoreMultiplier = 0;
        //    changeInRot = 0f;
        //}
    }

    void HandleScoreChange(string cupName)
    {
        int score = GetCupScore(cupName);

        currentTrickChain.Add(cupName);

        currScoreMultiplier++;

        currentScore += score * (currScoreMultiplier);
        if (currentScore > currentHighScore)
        {
            currentHighScore = currentScore;
            UIManager.UpdateHighScore(currentHighScore);

            PlayerPrefs.SetInt("HighScore", currentHighScore);
        }

        UIManager.UpdateScore(currentScore);
        UIManager.UpdateTrickChain(currentTrickChain, currScoreMultiplier);
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

    void HandleFailTrick()
    {

        currentScore = 0;
        currScoreMultiplier = 0;
        currentTrickChain.Clear();
        UIManager.UpdateTrickChain(currentTrickChain, currScoreMultiplier);

        // update score to 0
        UIManager.UpdateScore(0);
    }
}

