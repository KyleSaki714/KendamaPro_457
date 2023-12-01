using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KenController : MonoBehaviour
{

    // references
    GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Start()
    {

        //startRotation = new(0f, 90f, 90f);
        //transform.rotation = Quaternion.Euler(startRotation);
    }

    // Update is called once per frame
    void Update()
    {
        // Get the mouse delta. This is not in the range -1...1
        //float h = horizontalSpeed * Input.GetAxis("Mouse X");
        if (gameManager.verticalSwingOn)
        {
            transform.Rotate(-gameManager.mouseVertVel, 0f, 0f);
        }
        else
        {
            transform.Rotate(0f, 0f, -gameManager.mouseHoriVel);
        }


    }

    void FixedUpdate()
    {


        //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, -h);


    }


    

}
