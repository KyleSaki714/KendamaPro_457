using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class KenController : MonoBehaviour
{

    public static event Action<float> OnKenRotating; // ken is now rotating from having stopped. sends the angle

    private Vector3 _screenPosition;
    private Vector3 _worldPosition;
    private Plane plane = new Plane(Vector3.back, 0);

    private MeshCollider cupsCollider;
    private MeshCollider baseCollider;

    [SerializeField]
    private float kenRotationVel = 2.5f;
    [SerializeField]
    private float rotValue = 0f;
    [SerializeField]
    private bool isRotating = false;
    [SerializeField]
    private float newRotSnapVal;

    // values for rotation lerp
    private float oldRotSnapVal;
    [SerializeField]
    float lerpDuration = 0.1f;
    float lerpValue;

    [SerializeField]
    bool trackChange = false; // begin to track change from current angle
    [SerializeField]
    float trickMultiplierRot = 0f;

    private void OnEnable()
    {
        TamaPhysics.OnInAir += TamaPhysics_OnInAir;
    }

    private void OnDisable()
    {
        TamaPhysics.OnInAir -= TamaPhysics_OnInAir;

    }

    private void Start()
    {
        cupsCollider = transform.Find("model/lowpoly_kendama/ken_cups").GetComponent<MeshCollider>();
        baseCollider = transform.Find("model/lowpoly_kendama/ken_base").GetComponent<MeshCollider>();

        oldRotSnapVal = rotValue;
    }

    // Update is called once per frame
    void Update()
    {
        // ken to mouse

        _screenPosition = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(_screenPosition);

        if (plane.Raycast(ray, out float distance))
        {
            _worldPosition = ray.GetPoint(distance);
        }

        transform.position = _worldPosition;

        // rotation tracking for trick multiplier

        // ken rotation

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            rotValue = newRotSnapVal;
            isRotating = false;
        }

        if (Input.GetKey(KeyCode.A))
        {

            if (isRotating)
            {
                rotValue += kenRotationVel;
            }
            else
            {
                rotValue += 80f;
                isRotating = true;
            }

            if (trackChange)
            {
                OnKenRotating?.Invoke(rotValue);
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (isRotating)
            {
                rotValue -= kenRotationVel;
            }
            else
            {
                isRotating = true;
                rotValue -= 80f;
            }

            if (trackChange)
            {
                OnKenRotating?.Invoke(rotValue);
            }
        }

        rotValue = Mathf.Clamp(rotValue % 360f, -359f, 359f);
        newRotSnapVal = Mathf.Round(rotValue / 90f) * 90f;


        if (oldRotSnapVal != newRotSnapVal)
        {
            Debug.Log("newRotSnapVal: " + newRotSnapVal + " oldRotSnapVal: " + oldRotSnapVal);
            
            // avoid lerping from 360 to 0
            if (Mathf.Abs(oldRotSnapVal) == 360f && newRotSnapVal == 0f)
            {
                // reset rotation to 0
                transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 0f, 0f));
            }
            else
            {
                StartCoroutine(RotateKenByLerping(oldRotSnapVal, newRotSnapVal));

            }
        }
        
        oldRotSnapVal = newRotSnapVal;
    }

    private void TamaPhysics_OnInAir(bool inAir)
    {
        // start tracking rotation
        trackChange = inAir;
    }

    // ask the ken to pause collision for a given collider.
    // danger: collider being passed in is assumed to be one of the colliders in ken
    public void pauseCollCollision(Collider coll)
    {
        StartCoroutine(TempDisableCollision(coll));
    }

    public void PauseAllCollision()
    {
        SetAllCollision(false);
    }

    public void ResumeAllCollision()
    {
        SetAllCollision(true);
    }

    // toggle collision for all collision boxes in the ken.
    void SetAllCollision(bool isEnabled)
    {
        //Debug.Log("ken colision enabled is " + isEnabled);
        cupsCollider.enabled = isEnabled;
        baseCollider.enabled = isEnabled;

        BoxCollider[] cupColliders = transform.GetComponentsInChildren<BoxCollider>();
        
        foreach (BoxCollider coll in cupColliders)
        {
            coll.enabled = isEnabled;
        }
    }

    // disable collision for ken and cup coll boxe for half a second
    IEnumerator TempDisableCollision(Collider collider)
    {
        if (collider != null)
        {
            //Debug.Log("temp disable collission");
            collider.enabled = false;
            cupsCollider.enabled = false;
            baseCollider.enabled = false;
            yield return new WaitForSeconds(0.3f);
            //Debug.Log("colision enabled");
            collider.enabled = true;
            cupsCollider.enabled = true;
            baseCollider.enabled = true;

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
