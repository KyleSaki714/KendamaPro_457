using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TamaEmitter : MonoBehaviour
{

    bool DEBUG = true;

    public GameObject emitter; // Emits spheres
    public GameObject tamaGameObject;

    // references to other objects in scene
    private Transform bigCupTransform;
    private GameObject gameManager;

    private bool decoyShown = false;

    [Header("Tama Attributes")]
    // User-defined public variables.
    // These define the properties of an emitter
    public float mass = 0.1f;
    public float scale = 1;
    //public float period = 0.5f;
    public Vector3 initialVelocity;
    public Vector3 constantF = new Vector3(0f, -9.8f, 0f);
    public float dragF = 0f;
    public int maxSpheres = 1;

    // Colliders in the scene
    private CustomCollider[] _colliders;

    // Emitted spheres
    private int _numSpheres;
    private int _sphereIndex;
    private List<Tama> _spheres;
    //private double _timeToEmit;
    
    // Forces
    private List<IForce> _forces;
    private ConstantForce _constantForce;
    private ViscousDragForce _viscousDragForce;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager");
        bigCupTransform = GameObject.Find("BigCup").gameObject.transform;
    }

    // Initialize data
    private void Start()
    {
        _numSpheres = 0;
        _sphereIndex = 0;
        _colliders = FindObjectsOfType<CustomCollider>();
        _spheres = new List<Tama>();

        _constantForce = new ConstantForce(constantF);
        _viscousDragForce = new ViscousDragForce(dragF);
        _forces = new List<IForce>
        {
            _constantForce,
            _viscousDragForce
        };

        // align x and z pos with the big cup
        //transform.position = new Vector3(bigCupTransform.position.x, transform.position.y, bigCupTransform.position.z);

        // show decoy
        ShowDecoy();
    }

    private void Update()
    {
        if (DEBUG)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                EmitTama();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
            }

        }
        transform.position = bigCupTransform.position;
    }

    // Emits spheres, compute their position and velocity, and check for collisions
    private void FixedUpdate()
    {
        //float deltaTime = Time.deltaTime;
        // Emit spheres
        //_timeToEmit -= deltaTime;
        //if (_timeToEmit <= 0.0) EmitSpheres();

        
        foreach (Tama sphere in _spheres) // For each sphere 
        {
            // Compute their position and velocity by solving the system of forces using Euler's method
            ComputeSphereMovement(sphere, _forces);
           
            foreach (CustomCollider customCollider in _colliders) // For each collider 
            {
                // Check for and handle collisions
                OnCollision(sphere, customCollider);
            }
        }



    }

    public Tama EmitTama()
    {
        Vector3 localVelocity = initialVelocity;

        // Get the world position of a sphere
        //Vector3 worldPos = emitter.transform.TransformPoint(localPos);
        Vector3 worldPos = transform.position;
        Vector3 worldVelocity = emitter.transform.TransformDirection(localVelocity);

        return EmitTama(mass, scale, worldPos, worldVelocity);
    }

    public Tama EmitTama(float mass, float scale, Vector3 worldPos, Vector3 worldVelocity)
    {
        HideDecoy();

        // Initialize a sphere 
        Tama sphere = new Tama(mass, scale, worldPos, worldVelocity, tamaGameObject);

        if (_numSpheres < maxSpheres)
        {
            // Add another sphere
            _spheres.Add(sphere);
            _numSpheres++;
        }
        else
        {
            Tama destroy = _spheres[_sphereIndex];
            Destroy(destroy.TamaGameObject);
            // Keep the number of sphere to a finite amount by just replacing the old sphere
            _spheres[_sphereIndex++] = sphere;
            // If the end is reached, reset the index to start remove the index-0 sphere
            if (_sphereIndex >= maxSpheres)
                _sphereIndex = 0;
        }

        return sphere;
    }

    public static void ComputeSphereMovement(Tama ball, List<IForce> forces)
    {
        // TODO: Calculate the ball's position and velocity by solving the system
        // of forces using Euler's method
        // (1) Calculate total forces

        Vector3 totalForce = Vector3.zero;
        foreach (IForce force in forces)
        {
            totalForce += force.GetForce(ball);
        }

        // (2) Solve the system of forces using Euler's method,
        //     and update the ball's position and velocity.
        float deltaTime = Time.deltaTime;

        Vector3 accel = totalForce / ball.Mass;
        Vector3 vel = ball.Velocity + deltaTime * accel;
        ball.Velocity= vel;
        // (position at time t for next frame) = (current position at current time t) + (delta) * (velocity at current time t)

        //forces.Clear();
        Vector3 pEuler = ball.Position + deltaTime * ball.Velocity;
        
        ball.Position = pEuler;

        // Update the transform of the actual game object
        ball.TamaGameObject.transform.position = pEuler;
    }

    public static bool OnCollision(Tama ball, CustomCollider customCollider)
    {
        Transform colliderTransform = customCollider.transform;
        Vector3 colliderSize = colliderTransform.lossyScale; // size of collider

        // Save current localScale value, and temporarily change the collider's
        // world scale to (1,1,1) for our calculations. (Don't modify this)
        Vector3 curLocalScale = colliderTransform.localScale;
        SetWorldScale(colliderTransform, Vector3.one);

        // Position and velocity of the ball in the the local frame of the collider
        Vector3 localPos = colliderTransform.InverseTransformPoint(ball.Position);
        Vector3 localVelocity = colliderTransform.InverseTransformDirection(ball.Velocity);

        float ballRadius = ball.Scale / 2.0f;
        float colliderRestitution = customCollider.restitution;

        // TODO: In the following if conditions assign these variables appropriately.
        bool collisionOccurred = false;      // if the ball collides with the collider.
        bool isEntering = false;             // if the ball is moving towards the collider.
        Vector3 normal = Vector3.zero;       // normal of the colliding surface.

        if (customCollider.CompareTag("SphereCollider"))
        {
            // Collision with a sphere collider
            float colliderRadius = colliderSize.x / 2f;  // We assume a sphere collider has the same x,y, and z scale values

            // TODO: Detect collision with a sphere collider.
            float distSphereToBall = localPos.magnitude;
            if (distSphereToBall < colliderRadius + ballRadius)
            {
                collisionOccurred = true;
            }

            Vector3 ballDir = localPos.normalized;
            if (collisionOccurred && Vector3.Dot(-ballDir, localVelocity.normalized) > 0)
            {
                isEntering = true;
            }

            normal = ballDir;

        }
        else if (customCollider.CompareTag("PlaneCollider"))
        {
            // Collision with a plane collider

            var planeHeight = colliderSize.x * 10; // height of plane, defined by the x-scale
            var planeWidth = colliderSize.z * 10; // width of plane, defined by the z-scale
            // Note: In Unity, a plane's actual size is its inspector values times 10.

            // TODO: Detect sphere collision with a plane collider

            float planeHalfHeight = planeHeight / 2f;
            float planeHalfWidth = planeWidth / 2f;

            // if ball is within the plane
            bool withinPlane = (localPos.x > -planeHalfHeight && localPos.x < planeHalfHeight &&
                localPos.z > -planeHalfWidth && localPos.z < planeHalfWidth);
            if (withinPlane)
            {
                normal = Vector3.up;
            }
            // edge case
            else if (!withinPlane &&
                    (localPos.x > -planeHalfHeight - 0.5f && localPos.x < planeHalfHeight + 0.5f &&
                        localPos.z > -planeHalfWidth - 0.5f && localPos.z < planeHalfWidth + 0.5f))
            {
                Vector3 edgePoint = Vector3.zero;

                if (localPos.x > planeHalfHeight)
                {
                    edgePoint.x = planeHalfHeight;
                    edgePoint.z = localPos.z;
                }
                else if (localPos.x < -planeHalfHeight)
                {
                    edgePoint.x = -planeHalfHeight;
                    edgePoint.z = localPos.z;
                }
                else if (localPos.z < -planeHalfWidth)
                {
                    edgePoint.z = -planeHalfWidth;
                    edgePoint.x = localPos.x;
                }
                else
                {
                    edgePoint.z = planeHalfWidth;
                    edgePoint.x = localPos.x;
                }

                normal = (localPos - edgePoint).normalized;
            }
            else
            {
                colliderTransform.localScale = curLocalScale;
                return false;
            }

            float dn = Vector3.Dot(localPos, normal);
            collisionOccurred = ballRadius - dn >= 0f;

            if (collisionOccurred)
            {
                float ballToSurface = Vector3.Dot(normal, ball.Velocity);
                isEntering = ballToSurface < 0f;
            }

            // Generally, when the sphere is moving on the plane, the restitution alone is not enough
            // to counter gravity and the ball will eventually sink. We solve this by ensuring that
            // the ball stays above the plane.
            if (collisionOccurred && isEntering)
            {
                // TODO: Follow these steps to ensure the sphere always on top of the plane.
                //   1. Find the new localPos of the ball that is always on the plane
                //   2. Convert the localPos to worldPos
                //   3. Update the sphere's position with the new value
                Vector3 d = (ballRadius - dn) * normal;
                Vector3 ballOnPlane = localPos + d;
                ball.Position = colliderTransform.TransformPoint(ballOnPlane);

            }
        }

        if (collisionOccurred && isEntering)
        {
            // The sphere needs to bounce.
            // TODO: Update the sphere's velocity, remember to bring the velocity to world space
            Vector3 V_Normal = Vector3.Dot(localVelocity, normal) * normal;
            Vector3 V_Tangential = localVelocity - V_Normal;

            Vector3 res_velocity = V_Tangential - colliderRestitution * V_Normal;
            ball.Velocity = colliderTransform.TransformDirection(res_velocity);
        }


        colliderTransform.localScale = curLocalScale; // Revert the collider scale back to former value
        return collisionOccurred;
    }

    // Set the world scale of an object
    public static void SetWorldScale(Transform transform, Vector3 worldScale)
    {
        transform.localScale = Vector3.one;
        Vector3 lossyScale = transform.lossyScale;
        transform.localScale = new Vector3(worldScale.x / lossyScale.x, worldScale.y / lossyScale.y,
            worldScale.z / lossyScale.z);
    }

    public void ShowDecoy()
    {
        decoyShown = true;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void HideDecoy()
    {
        decoyShown = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
