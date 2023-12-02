using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KenController : MonoBehaviour
{

    private Vector3 _screenPosition;
    private Vector3 _worldPosition;
    private Plane plane = new Plane(Vector3.back, 0);

    // Update is called once per frame
    void Update()
    {
        _screenPosition = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(_screenPosition);

        if (plane.Raycast(ray, out float distance))
        {
            _worldPosition = ray.GetPoint(distance);
        }

        transform.position = _worldPosition;

        //startRotation = new(0f, 90f, 90f);
        //transform.rotation = Quaternion.Euler(startRotation);

    }

    void FixedUpdate()
    {


        //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, -h);


    }


    

}
