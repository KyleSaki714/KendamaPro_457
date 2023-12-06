using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class KenController : MonoBehaviour
{

    private Vector3 _screenPosition;
    private Vector3 _worldPosition;
    private Plane plane = new Plane(Vector3.back, 0);

    private MeshCollider cups;
    private MeshCollider spikeBase;

    [SerializeField]
    private float kenRotationVel = 2.5f;
    [SerializeField]
    private float rotValue = 0f;
    [SerializeField]
    private float newSnap;

    // values for rotation lerp
    private float oldRotSnapVal;
    [SerializeField]
    float lerpDuration = 0.1f;
    float lerpValue;

    private void Start()
    {
        cups = transform.Find("model/lowpoly_kendama/ken_cups").GetComponent<MeshCollider>();
        spikeBase = transform.Find("model/lowpoly_kendama/ken_base").GetComponent<MeshCollider>();

        oldRotSnapVal = rotValue;
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
            //transform.rotation = transform.rotation * Quaternion.Euler(new Vector3(0f, 0f, kenRotationVel));
            rotValue += kenRotationVel;
        }
        if (Input.GetKey(KeyCode.D))
        {
            //transform.rotation = transform.rotation * Quaternion.Euler(new Vector3(0f, 0f, -kenRotationVel));
            rotValue -= kenRotationVel;
        }

        

        // Clamp to prevent value from going to -360 to 0 sometimes
        rotValue = Mathf.Clamp(rotValue % 360f, -359f, 359f);
        //float zrot = transform.rotation.eulerAngles.z;
        newSnap = Mathf.Round(rotValue / 90f) * 90f;


        if (oldRotSnapVal != newSnap)
        {
            // avoid lerping from 360 to 0
            if (Mathf.Abs(oldRotSnapVal) == 360f && newSnap == 0f)
            {
                // reset rotation to 0
                transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 0f, 0f));
            }
            else
            {
                StartCoroutine(RotateKenByLerping(oldRotSnapVal, newSnap));
            }
        }

        oldRotSnapVal = newSnap;

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
        if (collider != null)
        {
            //Debug.Log("temp disable collission");
            collider.enabled = false;
            cups.enabled = false;
            spikeBase.enabled = false;
            yield return new WaitForSeconds(0.1f);
            //Debug.Log("colision enabled");
            collider.enabled = true;
            cups.enabled = true;
            spikeBase.enabled = true;

        }
    }

    // lerp z rotation value for smooth animation
    // stolen from: https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/#right_way_to_use_lerp
    // thanks man!
    IEnumerator RotateKenByLerping(float startValue, float endValue)
    {
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {
            lerpValue = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 0f, lerpValue));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        lerpValue = endValue;
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 0f, lerpValue));
    }
}
