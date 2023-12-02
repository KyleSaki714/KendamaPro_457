using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // references
    private KenController kenController;
    private GameObject ground;

    public static GameManager Instance { get; private set; }
    public AudioManager AudioManager { get; private set; }
    public UIManager UIManager { get; private set; }

    private void Awake()
    {
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
}

