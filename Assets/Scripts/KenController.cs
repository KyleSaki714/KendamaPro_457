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

    private Rigidbody cups;
    private Rigidbody spikeBase;

    [SerializeField]
    private float kenRotationVel = 5f;

    private void Start()
    {
        cups = transform.Find("model/lowpoly_kendama/ken_cups").GetComponent<Rigidbody>();
        spikeBase = transform.Find("model/lowpoly_kendama/ken_base").GetComponent<Rigidbody>();
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

        if (Input.GetKey(KeyCode.A))
        {
            transform.rotation = transform.rotation * Quaternion.Euler(new Vector3(0f, 0f, kenRotationVel));
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.rotation = transform.rotation * Quaternion.Euler(new Vector3(0f, 0f, -kenRotationVel));
        }

        float zrot = transform.rotation.eulerAngles.z;
        


        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.rotation = Quaternion.Euler(new Vector3(-180f, 0f, 0f));
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }

    // ask the ken to pause collision for a given collider.
    // danger: collider being passed in is assumed to be one of the colliders in ken
    public void pauseCollision(Collider coll)
    {
        StartCoroutine(TempDisableCollision(coll));
    }

    // disable collision for half a second
    IEnumerator TempDisableCollision(Collider collider)
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("temp disable collission");
        collider.enabled = false;
        yield return new WaitForSeconds(0.1f);
        Debug.Log("colision enabled");
        collider.enabled = true;

    }
}
