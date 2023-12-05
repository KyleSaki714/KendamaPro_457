using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TamaPhysics : MonoBehaviour
{

    Rigidbody rb;

    bool kenCollision = false; // make sure tama actually collides with the ken
    float cupLandRotThreshold = 40f;
    [SerializeField]
    float tamaCupSitOffset = 0.7f;
    bool cupSit = false;
    Collider currentCup;

    Transform kenTransform;
    Vector3 kenEuler;
    KenController kenController;

    Vector3 lastMousePos;
    [SerializeField]
    Vector3 lastMouseDelta;
    [SerializeField]
    Vector3 mouseDelta;

    [SerializeField]
    float tamaLaunchThreshold = 20f;
    [SerializeField]
    float tamaLaunchMultiplier = 1f;
    [SerializeField]
    Vector3 tamaLaunchAngle;


    private void Awake()
    {
        kenTransform = GameObject.Find("Ken").transform;
    }

    private void Start()
    {
        lastMousePos = Input.mousePosition;
        rb = GetComponent<Rigidbody>();
        kenController = kenTransform.GetComponent<KenController>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown((int) MouseButton.Left))
        {
            tamaLaunch();
        }
        if (Input.GetMouseButtonDown((int) MouseButton.Right))
        {
            transform.position = kenTransform.position;
        }
    }

    void FixedUpdate()
    {
        // get kendama euler angles
        kenEuler = kenTransform.rotation.eulerAngles;

        // get launch angle for tama, based on kendama rotation angle (z axis)
        // add 90 degrees to offset
        tamaLaunchAngle = new Vector3(Mathf.Sin(kenTransform.rotation.eulerAngles.z),
                                      Mathf.Cos(kenTransform.rotation.eulerAngles.z),
                                      0f);

        // get the current mouse velocity
        mouseDelta = Input.mousePosition - lastMousePos;

        if (cupSit)
        {
            if (checkLandBigCup())
            {
                Transform currCupTransform = currentCup.transform;
                Vector3 cupOffset = currCupTransform.up.normalized * tamaCupSitOffset;
                rb.MovePosition(currCupTransform.position + cupOffset);
                rb.freezeRotation = true;

                // if launched, break out of cup
                if (mouseDelta == Vector3.zero && lastMouseDelta.y > tamaLaunchThreshold)
                {
                    kenController.pauseCollision(currentCup);
                    kenCollision = false;
                    cupSit = false;
                    currentCup = null;
                    rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ;
                    tamaLaunch();
                }
            }
            else
            {
                cupSit = false;
            }

        }

        lastMouseDelta = mouseDelta;
        lastMousePos = Input.mousePosition;
    }

    private void OnCollisionStay(Collision collision)
    {
        // if tama is actually touching the ken
        if (collision.gameObject.name == "ken_cups" ||  collision.gameObject.name == "ken_base")
        {
            kenCollision = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // if tama is in the area of the cup
        if (other.transform.CompareTag("Cup"))
        {
            if (kenCollision == true && Mathf.Abs(kenEuler.x) <= cupLandRotThreshold)
            {
                cupSit = true;
                currentCup = other;
            }
        }
    }

    bool checkLandBigCup()
    {
        if (kenEuler.z >= 0f && kenEuler.z <= 40f ||
            kenEuler.z > 360f - 40f && kenEuler.z < 360f)
        {
            //Debug.Log("Landed big cup");
            return true;
        }
        return false;
    }

    void tamaLaunch()
    {
        //rb.AddForce(tamaLaunchMultiplier * tamaLaunchAngle.normalized, ForceMode.Impulse);
        rb.AddForce(tamaLaunchMultiplier * Vector3.up, ForceMode.Impulse);
        Debug.Log("launched tama from cup: " + (tamaLaunchMultiplier * tamaLaunchAngle.normalized));
    }


}
