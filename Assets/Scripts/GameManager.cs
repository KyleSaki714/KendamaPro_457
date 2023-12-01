using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // references
    private TamaEmitter tamaEmitter;
    private KenController kenController;
    private GameObject ground;

    // public fields
    public float mouseVertVel;
    public float mouseHoriVel;
    public bool verticalSwingOn;

    [Header("game logic")]
    
    // ball has just been launched in air and not fallen to the ground (lose)
    private bool isRallying;
    private float swingUpThreshold = 4.15f;
    [SerializeField]
    private float tamaSwingUpMultiplier = 1f;
    private bool hasSwungUp = false; // swing up has just been executed.

    private Tama currentTama;
    [SerializeField]
    private float tamaGroundTouchThreshold = 1f;

    [SerializeField]
    private float horizontalSpeed = 4.43f;
    [SerializeField]
    private float verticalSpeed = 2.81f;
    //Vector3 startRotation;

    private void Awake()
    {
        tamaEmitter = GameObject.Find("TamaEmitter").GetComponent<TamaEmitter>();
        kenController = GameObject.Find("Ken").GetComponent<KenController>();
        ground = GameObject.Find("Ground");
    }

    // Start is called before the first frame update
    void Start()
    {
        isRallying = false;
        verticalSwingOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (verticalSwingOn)
        {
            mouseVertVel = verticalSpeed * Input.GetAxis("Mouse Y");
            //Debug.Log(mouseVertVel);

            CheckSwing();
        }
        else
        {
            mouseHoriVel = horizontalSpeed * Input.GetAxis("Mouse X");
            //Debug.Log(mouseHoriVel);
            
        }

        // while in a rally (tama hasn't touched the ground)
        if (isRallying) 
        {
            // check for touching the ground
            if (currentTama.TamaGameObject.transform.position.y < ground.transform.position.y + tamaGroundTouchThreshold)
            {
                Debug.Log("tricks failed! rally over");
                isRallying=false;
            }
        }

        CheckSwitchRotation();
    }

    private void FixedUpdate()
    {
        
    }

    void CheckSwing()
    {
        // check swing up. must meet velocity, hasn't just swing up, and hasn't failed any tricks yet.
        if (mouseVertVel > swingUpThreshold && !hasSwungUp && !isRallying)
        {
            KenSwingUp();
            Debug.Log("Mouse Swing Up! " + mouseVertVel);
        }

        // check swing down (reset hasSwungUp)
        if (mouseVertVel < 0 && hasSwungUp)
        {
            hasSwungUp = false;
        }
    }

    void CheckSwitchRotation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (verticalSwingOn)
            {
                verticalSwingOn = false;
            }
            else
            {
                verticalSwingOn = true;
            }

        }
    }

    // is called upon the swing of the ken, and the launch of the tama.
    void KenSwingUp()
    {
        hasSwungUp = true;
        isRallying = true;

        // take the velocity from the mouse
        Vector3 tamaVelocity = new(0f, mouseVertVel * tamaSwingUpMultiplier, 0f);
        // pos from emitter
        currentTama = tamaEmitter.EmitTama(tamaEmitter.mass, tamaEmitter.scale, tamaEmitter.transform.position, tamaVelocity);
        
    }
}
