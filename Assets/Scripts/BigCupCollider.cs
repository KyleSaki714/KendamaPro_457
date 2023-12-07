using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCupCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.transform.name);
        Debug.Log("Enter");
    }

    void OnCollisionStay(Collision collision)
    {
        Debug.Log("Stay");
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("Exit");
    }
}
