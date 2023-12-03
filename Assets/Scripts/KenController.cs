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

    private Rigidbody kenrb;

    [SerializeField]
    private float kenRotationVel = 5f;

    private void Start()
    {
        kenrb = GetComponentInChildren<Rigidbody>();

    }

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
        if (Input.GetKeyDown(KeyCode.A))
        {
            Quaternion deltaRotation = Quaternion.Euler(kenRotationVel * Time.fixedDeltaTime * Vector3.right);
            kenrb.MoveRotation(kenrb.rotation * deltaRotation);
            //Vector3 newRot = new Vector3(kenrb.rotation.x + kenRotationVel, kenrb.rotation.y, kenrb.rotation.z);
            //kenrb.MoveRotation(Quaternion.Euler(newRot));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Quaternion deltaRotation = Quaternion.Euler(-kenRotationVel * Time.fixedDeltaTime * Vector3.right);
            kenrb.MoveRotation(kenrb.rotation * deltaRotation);
            //Vector3 newRot = new Vector3(kenrb.rotation.x - kenRotationVel, kenrb.rotation.y, kenrb.rotation.z);
            //kenrb.MoveRotation(Quaternion.Euler(newRot));
        }

        //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, -h);


    }


    

}
