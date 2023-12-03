using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TamaPhysics : MonoBehaviour
{

    Rigidbody rb;

    bool kenCollision = false; // make sure tama actually collides with the ken
    float cupLandRotThreshold = 20f;
    float tamaCupSitOffset = 0.7f;
    bool cupSit = false;
    Collider currentCup;

    Transform kenTransform;

    Vector3 lastMousePos;
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

    void Update()
    {
        //Debug.Log("mouse delta: " + mouseDelta);

        if (cupSit)
        {
            if (Mathf.Abs(kenTransform.rotation.z) <= cupLandRotThreshold && Mathf.Abs(kenTransform.rotation.x) <= cupLandRotThreshold)
            {
                Transform currCupTransform = currentCup.transform;
                Vector3 cupOffset = currCupTransform.up.normalized * tamaCupSitOffset;
                transform.position = currCupTransform.position + cupOffset;
            }

            // if launched, break out of cup
            if (mouseDelta.y > tamaLaunchThreshold)
            {
                cupSit = false;
                kenCollision = false;
                currentCup = null;
                rb.velocity = mouseDelta * tamaLaunchMultiplier;
                Debug.Log("launch tama from cup");
            }
        }

        lastMousePos = Input.mousePosition;
    }

    private void OnCollisionEnter(Collision collision)
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

            if (Mathf.Abs(kenTransform.rotation.z) <= cupLandRotThreshold && Mathf.Abs(kenTransform.rotation.x) <= cupLandRotThreshold && kenCollision == true)
            {
                //Debug.Log("Stay: " + cupTransform.name);
                cupSit = true;
                currentCup = other;

            }


        }
    }

    //private void OnTriggerStay(Collider other)
    //{
        
    //    if (other.transform.CompareTag("Cup"))
    //    {
    //        Transform cupTransform = other.gameObject.transform;

    //        if (Mathf.Abs(kenTransform.rotation.z) <= cupLandRotThreshold && Mathf.Abs(kenTransform.rotation.x) <= cupLandRotThreshold && kenCollision == true) 
    //        {
    //            //Debug.Log("Stay: " + cupTransform.name);
    //            cupSit = true;
    //            currentCup = other;

    //        }


    //    }
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.transform.CompareTag("Cup"))
    //    {
    //        kenCollision = false;
    //        currentCup = null;
    //    }
    //}
}
