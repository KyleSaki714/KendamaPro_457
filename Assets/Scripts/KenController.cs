using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KenController : MonoBehaviour
{

    [SerializeField]
    private float horizontalSpeed = 4.43f;
    [SerializeField]
    private float verticalSpeed = 2.81f;
    //Vector3 startRotation;

    // Start is called before the first frame update
    void Start()
    {
        //startRotation = new(0f, 90f, 90f);
        //transform.rotation = Quaternion.Euler(startRotation);
    }

    // Update is called once per frame
    void Update()
    {
        // Get the mouse delta. This is not in the range -1...1
        float h = horizontalSpeed * Input.GetAxis("Mouse X");
        float v = verticalSpeed * Input.GetAxis("Mouse Y");

        //Debug.Log("horizontalSpeed: " + horizontalSpeed);
        //Debug.Log("verticalSpeed: " + verticalSpeed);

        transform.Rotate(-v, 0f, -h);
        //transform.Rotate(0f, 0f, 0f);

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }
    }
}
