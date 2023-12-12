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

    [SerializeField]
    bool collidingWithKen = false;
    [SerializeField]
    bool cupSit = false;
    [SerializeField]
    Collider currentCup;

    private float bigCupSit = 0.7f;
    private float smallCupSit = 0.5f;
    private float baseCupSit = 0.5f;
    private float spikeSit = 0.3f;

    Transform kenTransform;
    Vector3 kenEuler;
    KenController kenController;
    bool kenCollisionPaused = false;
    float _rekcDistOffset = 2.85f;

    bool _freeBall = false;

    float stringLengthShort = 8.57f;
    [SerializeField]
    float stringLengthLong = 12.73f;
    Transform stringAnchor;
    float stringPullForceMin = 0.1f;
    [SerializeField]
    float stringPullForceMax = 0.76f;
    // must be within this range to count being at the end of string,
    // this is so force is added for more frames when we yank.
    float _stringEndThreshold = 1.46f; 

    Vector3 lastMousePos;
    Vector3 lastMouseDelta;
    [SerializeField]
    Vector3 mouseDelta;
    [SerializeField]
    float mouseSpeed;
    // pizza slice range of the radius from the tama to stringAnchor where we want yanking to be possible.
    float yankRange = 20f;
    // mouse speed threshold for yanking the tama from the end of the string.
    float yankThreshold = 2.16f;
    float yankForceMultiplier = 1.9f;
    float tamaSpeed;
    float _tamaMaxSpeed = 30f;

    float tamaLaunchThreshold = 2.5f;
    float tamaLaunchMultiplier = 300f;
    [SerializeField]
    bool isLaunching;

    [SerializeField]
    bool justLandedCup;
    bool failClockActive = false;
    int failClockFrameCountStart;
    float _failClockDuration = 0.2f;
    [SerializeField]
    int failClockFrameWindow = 40;


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

        // get kendama euler angles
        kenEuler = kenTransform.rotation.eulerAngles;

        // get the current mouse velocity
        mouseDelta = Input.mousePosition - lastMousePos;
        mouseSpeed = mouseDelta.magnitude;

        tamaSpeed = rb.velocity.magnitude;

        ClampVelocity();

        CheckReeenableGimmicks();

        CheckFailClock();

        CheckString();

        CheckCupSit();

        lastMouseDelta = mouseDelta;
        lastMousePos = Input.mousePosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnInAir?.Invoke(false);
        if (collision.gameObject.name == "ken_cups" || collision.gameObject.name == "ken_base")
        {
            collidingWithKen = true;
            failClockActive = true;
            failClockFrameCountStart = Time.frameCount;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // if tama is actually touching the ken
        if (collision.gameObject.name == "ken_cups" ||  collision.gameObject.name == "ken_base")
        {
            collidingWithKen = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // if tama is in the area of the cup
        if (other.transform.CompareTag("Cup"))
        {
            if (!isLaunching && collidingWithKen == true)
            {
                currentCup = other;
                string pinkpantheress = currentCup.name;

                // check ken rotation legal! (i.e. cant snap to cup tama upside down)
                cupSit = CheckLandCup(pinkpantheress);
                Debug.Log("cupsit = " + cupSit);

                if (cupSit && currentCup != null && !justLandedCup)
                {
                    OnCupLand?.Invoke(GetCupScore(pinkpantheress));
                    justLandedCup = true;

                    GameManager.Instance.AudioManager.PlayCup();
                    GameManager.Instance.UIManager.ShowSplashText();
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        OnInAir?.Invoke(true);
        collidingWithKen = false;
    }

    // so crazy amounts of forces arent applied
    void ClampVelocity()
    {
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, _tamaMaxSpeed);
    }

    // checks to reenable ken collision, after having disabled it previously
    // checks to reenable the ball to snap to z = 0
    void CheckReeenableGimmicks()
    {
        // different ideas: distance, y position, timer
        //float distToKen = Vector3.Distance(rb.position, kenTransform.position);

        if (kenCollisionPaused && rb.position.y > kenTransform.position.y + _rekcDistOffset)
        {
            kenController.ResumeAllCollision();
            kenCollisionPaused = false;
        }

        // reenable the ball to snap to z = 0
        if (_freeBall && rb.position.y < kenTransform.position.y - _rekcDistOffset)
        {
            // restrict z
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
            rb.MovePosition(rb.position);
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ;
            
            _freeBall = false;
        }
    }

    void CheckFailClock()
    {
        if (failClockActive)
        {
            int framesSinceStart = Time.frameCount - failClockFrameCountStart;
            //Debug.Log("Failclock running... " + framesSinceStart);

            if (justLandedCup && framesSinceStart < failClockFrameWindow)
            {
                //Debug.Log("Evaded fail");
                failClockActive = false;
            }
            else if (framesSinceStart >= failClockFrameWindow)
            {
                Debug.Log("failed!!!");
                rb.constraints = RigidbodyConstraints.None;
                rb.AddForce(Vector3.forward * 5f * Mathf.Round(UnityEngine.Random.Range(-1, 1)), ForceMode.Impulse);
                OnFail?.Invoke();
                GameManager.Instance.AudioManager.PlayFail();

                _freeBall = true;

                failClockActive = false;
            }

        }
    }

    void CheckString()
    {
        // below ken = shorter string, above ken = longer string (for more airtime)
        float stringLength = stringLengthShort;
        if (rb.position.y > kenTransform.position.y + _rekcDistOffset)
        {
            stringLength = stringLengthLong;
        }    

        // restrict distance from string
        Vector3 tamaDistAlongString = Vector3.ClampMagnitude(rb.position - stringAnchor.position, stringLength);

        bool underKen = rb.position.y < kenTransform.position.y;
        if (tamaDistAlongString.magnitude > stringLength - _stringEndThreshold)
        {
            float stringPullForce = Mathf.Clamp(mouseSpeed, stringPullForceMin, stringPullForceMax);
            Vector3 tamaToKenDirection = (stringAnchor.position - rb.position).normalized;
            Vector3 pullForce = tamaToKenDirection * stringPullForce;

            // restrict yank force to only some domain of angles under ken
            float hangAngle = Vector3.Dot(tamaToKenDirection, Vector3.right) * Mathf.Rad2Deg;
            bool withinYankRange = hangAngle < yankRange && hangAngle > -yankRange;

            // when the string is yanked, apply force relative to yank power
            if (underKen && withinYankRange && mouseSpeed > yankThreshold)
            {
                Vector3 yankForce = pullForce * (mouseSpeed * yankForceMultiplier);
                rb.AddForce(yankForce, ForceMode.Impulse);
                //Debug.Log("yanked! hangAngle: " + hangAngle);

                if (kenCollisionPaused == false)
                {
                    kenController.PauseAllCollision();
                    kenCollisionPaused = true;
                }
            }

            // add force, relative to mouse speed, in the direction of the pull
            // (to mimic being at the end of a string)
            rb.AddForce(pullForce, ForceMode.VelocityChange);
        }

        // restrict string position
        rb.MovePosition(stringAnchor.position + tamaDistAlongString);
    }

    // check that tama sits in cup (snaps like a magnet)
    void CheckCupSit()
    {
        // check to launch in the cup
        if (cupSit)
        {
            Transform currCupTransform = currentCup.transform;
            Vector3 cupOffset = currCupTransform.up.normalized * GetCupSitOffset(currentCup.name);
            rb.MovePosition(currCupTransform.position + cupOffset);
            rb.freezeRotation = true;

            // if launched, break out of cup
            //  mouseDelta == Vector3.zero
            if (!isLaunching && mouseSpeed < 10f && lastMouseDelta.y > tamaLaunchThreshold)
            {
                StartCoroutine(TamaLaunchLockAcquire());
            }
        }
    }

    // make sure landing is possible for the given cup. or spike.
    // TODO: velocity constraint
    bool CheckLandCup(string cupName)
    {
        // TODO: WARNING: DID NOT IMPLEMENT FLIPPING ACROSS X OF THE KEN YET
        Vector3 kenEuler = kenTransform.rotation.eulerAngles;

        return cupName switch
        {
            "BigCup" => (kenEuler.z >= 0f - 40f && kenEuler.z <= 0f + 40f) ||
                        (kenEuler.z >= 360f - 40f && kenEuler.z <= 360f + 40f),
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

            TamaLaunch();

            yield return new WaitForSeconds(1f);
            isLaunching = false;
        }
    }

    // will only execute if isLaunching is true
    void TamaLaunch()
    {
        // ask the ken to pause collision for its cup trigger for a bit
        kenController.pauseCollCollision(currentCup);

        // reset this stuff
        collidingWithKen = false;
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
        if (!failClockActive && !cupSit)
        {
            failClockActive = true;

            Debug.Log("fail clock started");

            float counter = 0f;
            while (counter < _failClockDuration)
            {
                if (cupSit || rb.position.y < kenTransform.position.y)
                {
                    Debug.Log("Evaded fail");
                    failClockActive = false;
                    yield break;
                }

                yield return new WaitForSeconds(0.01f);
                counter += 0.01f;

            }

            if (failClockActive)
            {
                Debug.Log("failed!!!");
                rb.constraints = RigidbodyConstraints.None;
                rb.AddForce(Vector3.forward * 5f * Mathf.Round(UnityEngine.Random.Range(-1, 1)), ForceMode.Impulse);
                OnFail?.Invoke();
                GameManager.Instance.AudioManager.PlayFail();

                _freeBall = true;

                failClockActive = false;

            }

        }
    }
}
