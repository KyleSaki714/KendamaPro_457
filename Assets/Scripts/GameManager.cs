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

    [SerializeField]
    private bool tamaInAir = true;

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


    // subscriptions

    private void OnEnable()
    {
        TamaPhysics.OnInAir += HandleTamaInAir;
    }

    private void OnDisable()
    {
        TamaPhysics.OnInAir -= HandleTamaInAir;
    }

    void HandleTamaInAir(bool isInAir)
    {
        tamaInAir = isInAir;
    }

    void HandleScoreChange()
    {

    }
}

