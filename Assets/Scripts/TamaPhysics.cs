using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class TamaPhysics : MonoBehaviour
{
    bool debugSuspendTama = false;

    public static event Action<bool> OnInAir; // ball is currently in air
    public static event Action<int> OnCupLand; // ball is landed in cup
    public static event Action OnFail; // ball hits / rests upon collision, failed to land trick

    Rigidbody rb;

    bool kenCollision = false;
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

    [SerializeField][Range(0.0f, 10.0f)]
    float stringLength = 2f;
    Transform stringAnchor;

    Vector3 lastMousePos;
    Vector3 lastMouseDelta;
    Vector3 mouseDelta;

    [SerializeField]
    float tamaLaunchThreshold = 20f;
    [SerializeField]
    float tamaLaunchMultiplier = 1f;
    bool isLaunching;

    [SerializeField]
    bool justLandedCup;
    bool failClockLock = false;
    [SerializeField]
    float _failClockDuration = 0.2f;


    private void Awake()
    {
        kenTransform = GameObject.Find("Ken").transform;
        stringAnchor = GameObject.Find("StringAnchor").transform;
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
            transform.position = kenTransform.position + Vector3.up * 3f;
            rb.velocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            debugSuspendTama = !debugSuspendTama;
        }
    }

    void FixedUpdate()
    {
        if (debugSuspendTama)
        {
            rb.MovePosition(Vector3.up * 4f);
        }

        // restrict distance from string
        Vector3 clampedVector = Vector3.ClampMagnitude(rb.position - stringAnchor.position, stringLength);
        rb.MovePosition(stringAnchor.position + clampedVector);
        Debug.DrawLine(rb.position, stringAnchor.position, Color.white, 0.01f);

        // get kendama euler angles
        kenEuler = kenTransform.rotation.eulerAngles;

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
        }

        lastMouseDelta = mouseDelta;
        lastMousePos = Input.mousePosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnInAir?.Invoke(false);
        if (collision.gameObject.name == "ken_cups" || collision.gameObject.name == "ken_base")
        {
            kenCollision = true;
            StartCoroutine(CollisionClock());
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // if tama is actually touching the ken
        if (collision.gameObject.name == "ken_cups" ||  collision.gameObject.name == "ken_base")
        {
            kenCollision = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        failClockLock = false;
    }

    private void OnTriggerStay(Collider other)
    {
        // if tama is in the area of the cup
        if (other.transform.CompareTag("Cup"))
        {
            if (!isLaunching && kenCollision == true)
            {
                currentCup = other;
                string pinkpantheress = currentCup.name;

                // check ken rotation legal! (i.e. cant snap to cup tama upside down)
                cupSit = CheckLandCup(pinkpantheress);

                if (cupSit && currentCup != null && !justLandedCup)
                {
                    OnCupLand?.Invoke(GetCupScore(pinkpantheress));
                    justLandedCup = true;

                    GameManager.Instance.AudioManager.PlayCup();
                }
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

    int GetCupScore(string cupName)
    {
        return cupName switch
        {
            "BigCup" => 100,
            "SmallCup" => 150,
            "BaseCup" => 125,
            _ => 0,
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
        justLandedCup = false;
        
        // update the isInAir field of GameManager
        OnInAir?.Invoke(true);

        // unfreeze tama, allow it to rotate X and Y
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ;

        rb.AddForce(tamaLaunchMultiplier * lastMouseDelta.normalized, ForceMode.Impulse);
        Debug.Log("launched tama from cup: " + (tamaLaunchMultiplier * lastMouseDelta.normalized));
    }

    // upon collision, a clock starts.
    // _failClockDuration describes the time to intercept this clock until the collision is counted as a fail.
    // cupSit breaks this counter, intercepting the fail. 
    IEnumerator CollisionClock()
    {
        if (!failClockLock)
        {
            failClockLock = true;

            Debug.Log("fail clock started");
            
            float counter = 0f;
            while (counter < _failClockDuration)
            {
                counter += 0.01f;

                yield return new WaitForSeconds(0.01f);
                
                if (cupSit)
                {
                    Debug.Log("Evaded fail");
                    failClockLock = false;
                    yield break;
                }

            }

            Debug.Log("failed!!!");
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(Vector3.back * 5f, ForceMode.Impulse);
            OnFail?.Invoke();
            GameManager.Instance.AudioManager.PlayFail();

            // wait for a few seconds, then respawn?

            // failClockLock = false moved to OnCollisionExit
        }
    }
}
