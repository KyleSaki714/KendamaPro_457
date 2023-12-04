using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TamaPhysics : MonoBehaviour
{

    Rigidbody rb;

    bool kenCollision = false; // make sure tama actually collides with the ken
    float cupLandRotThreshold = 40f;
    float tamaCupSitOffset = 0.7f;
    bool cupSit = false;
    Collider currentCup;

    Transform kenTransform;
    Vector3 kenEuler;

    Vector3 lastMousePos;
    Vector3 lastPosMouseDelta;
    public Vector3 mouseDelta
    {
	    get
	    {
		    return Input.mousePosition - lastMousePos;
	    }
    }
    [SerializeField]
    float tamaLaunchThreshold = 20f;
    [SerializeField]
    float tamaLaunchMultiplier = 1f;


    private void Awake()
    {
        kenTransform = GameObject.Find("Ken").transform;
    }

    private void Start()
    {
        lastMousePos = Input.mousePosition;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown((int) MouseButton.Left))
        {
            tamaLaunch();
        }
    }

    void FixedUpdate()
    {
        kenEuler = kenTransform.rotation.eulerAngles;

        Debug.Log("mousedelta: " + mouseDelta);
        Debug.Log("lastPosMouseDelta: " + lastPosMouseDelta);
        

        if (cupSit)
        {
            if (checkLandBigCup())
            {
                Transform currCupTransform = currentCup.transform;
                Vector3 cupOffset = currCupTransform.up.normalized * tamaCupSitOffset;
                //transform.position = currCupTransform.position + cupOffset;
                rb.MovePosition(currCupTransform.position + cupOffset);

                // if launched, break out of cup
                Debug.Log("mousedelta is 0: " + (mouseDelta == Vector3.zero));
                Debug.Log("lastPosMouseDelta.y > tamaLaunchThreshold: " + (lastPosMouseDelta.y > tamaLaunchThreshold));
                if (mouseDelta == Vector3.zero && lastPosMouseDelta.y > tamaLaunchThreshold)
                {
                    tamaLaunch();
                }
            }
            else
            {
                cupSit = false;
            }

        }

        lastPosMouseDelta = mouseDelta;
        lastMousePos = Input.mousePosition;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name == "ken_cups" ||  collision.gameObject.name == "ken_base")
        {
            kenCollision = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.transform.CompareTag("Cup"))
        {

            Transform cupTransform = other.gameObject.transform;

            if (Mathf.Abs(kenEuler.x) <= cupLandRotThreshold && kenCollision == true)
            {
                Debug.Log("trigger enter: " + cupTransform.name);
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
        cupSit = false;
        kenCollision = false;
        currentCup = null;
        rb.AddForce(new Vector3(0f, tamaLaunchMultiplier, 0f), ForceMode.Impulse);
        Debug.Log("launched tama from cup: " + (rb.velocity + lastPosMouseDelta.normalized * tamaLaunchMultiplier));
    }

}
