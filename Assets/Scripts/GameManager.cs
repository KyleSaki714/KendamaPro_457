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
    private KenController kenController;
    private GameObject ground;

    public static GameManager Instance { get; private set; }
    public AudioManager AudioManager { get; private set; }
    public UIManager UIManager { get; private set; }


    // game logic!
    [SerializeField]
    bool isRally;
    [SerializeField]
    bool tamaIsInAir;
    int currentScore;
    int currentHighScore;

    // rotations
    [SerializeField]
    float currentRot;
    [SerializeField]
    float initialRot;
    [SerializeField]
    float changeInRot;
    [SerializeField]
    int currFullRotations;
    [SerializeField]
    bool fullRotPossible;


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

        kenController = GameObject.Find("Ken").GetComponent<KenController>();
        ground = GameObject.Find("Ground");
    }

    private void Start()
    {
        isRally = false;
        tamaIsInAir = true;

        currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
        UIManager.UpdateHighScore(currentHighScore);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        }
    }


    // subscriptions

    private void OnEnable()
    {
        TamaPhysics.OnInAir += HandleTamaInAir;
        TamaPhysics.OnCupLand += HandleScoreChange;
        TamaPhysics.OnFail += HandleFailTrick;
        KenController.OnKenRotating += KenController_OnKenRotating;
    }

    private void OnDisable()
    {
        TamaPhysics.OnInAir -= HandleTamaInAir;
        TamaPhysics.OnCupLand -= HandleScoreChange;
        TamaPhysics.OnFail -= HandleFailTrick;
        KenController.OnKenRotating -= KenController_OnKenRotating;
    }

    private void KenController_OnKenRotating(float rotValue)
    {
        currentRot = rotValue;
        if (tamaIsInAir)
        {
            changeInRot = Mathf.Abs(currentRot - initialRot);
            CheckFullRotation();
        }
    }

    void CheckFullRotation()
    {
        if (changeInRot > 330f)
        {
            currFullRotations++;
            fullRotPossible = false;
        }
    }

    void HandleTamaInAir(bool isInAir)
    {
        tamaIsInAir = isInAir;
        if (tamaIsInAir)
        {
            initialRot = currentRot;
        }
        else
        {
            changeInRot = 0f;
            currFullRotations = 0;
        }
    }

    void HandleScoreChange(int score)
    {
        currentScore += score;
        if (currentScore > currentHighScore)
        {
            currentHighScore = currentScore;
            UIManager.UpdateHighScore(currentHighScore);

            PlayerPrefs.SetInt("HighScore", currentHighScore);
        }

        UIManager.UpdateScore(currentScore);
    }

    void HandleFailTrick()
    {
        currentScore = 0;
        UIManager.UpdateScore(currentScore);
    }
}

