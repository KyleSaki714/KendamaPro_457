using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    float secsSinceLaunch;
    [SerializeField]
    bool tamaIsInAir;
    [SerializeField]
    int currentScore;

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
        secsSinceLaunch = 0f;
    }


    // subscriptions

    private void OnEnable()
    {
        TamaPhysics.OnInAir += HandleTamaInAir;
        TamaPhysics.OnCupLand += HandleScoreChange;
    }

    private void OnDisable()
    {
        TamaPhysics.OnInAir -= HandleTamaInAir;
        TamaPhysics.OnCupLand += HandleScoreChange;

    }

    void HandleTamaInAir(bool isInAir)
    {
        tamaIsInAir = isInAir;
    }

    void HandleScoreChange(int score)
    {
        currentScore += score;
        UIManager.UpdateScore(currentScore);
    }
}

