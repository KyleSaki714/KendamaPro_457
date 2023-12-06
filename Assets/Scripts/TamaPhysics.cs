using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class TamaPhysics : MonoBehaviour
{

    public static event Action<bool> OnInAir;

    Rigidbody rb;

    bool kenCollision = false; // make sure tama actually collides with the ken
    float cupLandRotThreshold = 40f;
    bool cupSit = false;
    Collider currentCup;

    private float bigCupSit = 0.7f;
    private float smallCupSit = 0.5f;
    private float baseCupSit = 0.5f;
    [SerializeField]
    private float spikeSit = 0.3f;

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
    bool isLaunching;


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
            StartCoroutine(TamaLaunchLockAcquire());
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

        if (cupSit && currentCup != null)
        {

            Transform currCupTransform = currentCup.transform;
            Vector3 cupOffset = currCupTransform.up.normalized * GetCupSitOffset(currentCup.name);
            rb.MovePosition(currCupTransform.position + cupOffset);
            rb.freezeRotation = true;

            // if launched, break out of cup
            if (!isLaunching && mouseDelta == Vector3.zero && lastMouseDelta.y > tamaLaunchThreshold)
            {
                StartCoroutine(TamaLaunchLockAcquire());
            }

            // check rotation of ken?

        }

        lastMouseDelta = mouseDelta;
        lastMousePos = Input.mousePosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnInAir?.Invoke(false);    
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
            if (!isLaunching && kenCollision == true)
            {
                currentCup = other;
                // check ken rotation legal! (cant snap tama upside down)

                cupSit = CheckLandCup(currentCup.name);
            }
        }
    }

    // make sure landing is possible for the given cup. or spike.
    // TODO: velocity constraint
    bool CheckLandCup(string cupName)
    {
        // TODO: WARNING: DID NOT IMPLEMENT FLIPPING ACROSS X OF THE KEN YET
        return cupName switch
        {
            "BigCup" => kenEuler.z >= 0f && kenEuler.z <= 40f || kenEuler.z > 360f - 40f && kenEuler.z < 360f,
            "SmallCup" => kenEuler.z >= 180f - 40f && kenEuler.z <= 180f + 40f,
            "BaseCup" => kenEuler.z >= 90f - 40f && kenEuler.z <= 90f + 50f,
            _ => false,
        };
    }

    float GetCupSitOffset(string cupName)
    {
        return cupName switch
        {
            "BigCup" => bigCupSit,
            "SmallCup" => smallCupSit,
            "BaseCup" => baseCupSit,
            _ => bigCupSit,
        };
    }

    // restrict to only one launch. only allows one tama to be launched every 1 second
    IEnumerator TamaLaunchLockAcquire()
    {
        if (!isLaunching)
        {
            isLaunching = true;

            tamaLaunch();

            yield return new WaitForSeconds(1f);
            isLaunching = false;
        }
    }

    // will only execute if isLaunching is true
    void tamaLaunch()
    {
        // ask the ken to pause collision for its cup trigger for a bit
        kenController.pauseCollision(currentCup);

        // reset this stuff
        kenCollision = false;
        cupSit = false;
        currentCup = null;
        
        // update the isInAir field of GameManager
        OnInAir?.Invoke(true);

        // unfreeze tama, allow it to rotate X and Y
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ;

        rb.AddForce(tamaLaunchMultiplier * Vector3.up, ForceMode.Impulse);
        Debug.Log("launched tama from cup: " + (tamaLaunchMultiplier * tamaLaunchAngle.normalized));
    }
}
