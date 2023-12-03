using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TamaPhysics : MonoBehaviour
{

    bool kenCollision = false; // make sure tama actually collides with the ken
    float cupLandRotThreshold = 20f;
    [SerializeField]
    float tamaCupSitOffset = 0.5f;
    bool cupSit = false;
    Collider currentCup;

    Transform kenTransform;

    private void Awake()
    {
        kenTransform = GameObject.Find("Ken").transform;
    }

    void Update()
    {
        if (cupSit)
        {
            if (Mathf.Abs(kenTransform.rotation.z) <= cupLandRotThreshold && Mathf.Abs(kenTransform.rotation.x) <= cupLandRotThreshold)
            {
                Transform currCupTransform = currentCup.transform;
                Vector3 cupOffset = currCupTransform.up.normalized * tamaCupSitOffset;
                transform.position = currCupTransform.position + cupOffset;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "ken_cups" ||  collision.gameObject.name == "ken_base")
        {
            kenCollision = true;
        }
    }

    private void OnTriggerStay(Collider other)
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
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Cup"))
        {
            kenCollision = false;
            currentCup = null;
        }
    }
}
