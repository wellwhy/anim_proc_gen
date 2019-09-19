using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controller : MonoBehaviour
{
    //CharacterController char_ctrl;
    GameObject camera;
    CharacterController char_ctrl;
    MeshRenderer viz;
    public float collision_padding = .0000f;
    public float max_slope_angle = 45f;
    public float near_padding = .0025f;
    public float step_height = .3f;
    smoothedFloat ang;
    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        char_ctrl = gameObject.GetComponent<CharacterController>();
        viz = gameObject.GetComponent<MeshRenderer>();
        ang = new smoothedFloat(0);
        ang.setSmoothValue(.1f);
    }

    void Update()
    {
        updatePlayer();
    }

    Vector3 input; //vector of movement to be applied (if collision allows) to the player
    Vector3 rawVelocity = Vector3.forward;
    Vector3 cam_pointing_vector; //a vector of the camera pointing to the player used to rotate WASD input with camera
    Vector3 faceForward;
    RaycastHit groundHit;
    float min = 0;
    public float max = 0;
    float t = 0;
    float ground_angle;
    public Vector3 lerped;
    float angleToInput, finalAngle;
    bool change = false;
    bool stopped = false;
    float startup = 0;
    bool jutMode = false;

    public Vector3 lastFrameVelocity = Vector3.zero;
    public Vector3 acceleration;
    void updatePlayer(){
        //the average person walks at 1.4 m/s, jogs at 2.2 m/s
                if(Input.GetKeyDown(KeyCode.R)){
            change = !change;
        }

        cam_pointing_vector = this.transform.position - camera.transform.position;
        if(Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0){ //prevents calculations when no input given
            if(lerped.magnitude == 0 && !stopped){
                stopped = true;
                startup = .001f;
            }
            //faceForward = Quaternion.AngleAxis(viz.transform.rotation.y,Vector3.up) * Vector3.forward; //????
            Debug.Log(this.transform.position.y);
            input = ( Quaternion.LookRotation(new Vector3(cam_pointing_vector.x,0f,cam_pointing_vector.z)) //Input starts as forwards,backwards,left,right like a plus sign +..
            * new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")) ).normalized;   //..It is rotated by multiplying it by the quaternion of the camera's angle
            if(Input.GetKey(KeyCode.LeftShift)){
                if(max == .91f)
                    t = (.91f/2.2f); //when switching to a new max, this will make the lerp not freak out
                max = 2.2f;
            }
            else{
                if(max != .91f)
                    max = Mathf.Clamp(max-.03f,.91f,2.2f); //linear transition between a new max
            }
            t = Mathf.Clamp01(t+.03f);
        }
        else{
            t = Mathf.Clamp01(t-.12f);
            //input = Vector3.zero;
        }
        if(stopped){
            startup = Mathf.Clamp01(startup*1.5f);
            if(startup == 1)
                stopped = false;
        }
        
        //rawVelocity.Normalize();
        //below is what i want in the if() statements
            //what is currently there is lerping of the Vectors, but I just need to lerp rotations (acceleration handled by lerped = rawVelocity~~~~)
        //I need to capture the position they are currently facing at the start of input and rotate
        //hard changes in velocity
        angleToInput = Vector3.SignedAngle(rawVelocity,input,Vector3.up);
        //ang.setTarget(angleToInput);
        if(Mathf.Abs(angleToInput) >= 170 && !jutMode){
            jutMode = true;
        }

        if(jutMode){
            if(Mathf.Abs(angleToInput) <= 10)
                jutMode = false;
            
            rawVelocity = Quaternion.AngleAxis((angleToInput*Mathf.Lerp(.1f,1,(1 - Mathf.Abs(angleToInput)/170f)) / 4f)*startup,Vector3.up) * (rawVelocity);
            lerped = rawVelocity * Mathf.Lerp(min,max,t) * (Mathf.Lerp(.01f,1,(1 - Mathf.Abs(angleToInput)/360f))) * startup;
        }
        else{
            rawVelocity = Quaternion.AngleAxis((angleToInput*Mathf.Lerp(.01f,1,(1 - Mathf.Abs(angleToInput)/180f)) / 5f)*startup,Vector3.up) * (rawVelocity);
            lerped = rawVelocity * Mathf.Lerp(min,max,t) * (Mathf.Lerp(.01f,1,(1 - Mathf.Abs(angleToInput)/360f))) * startup;
        }

        //if(Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0) //prevents calculations when no input given
            viz.transform.rotation = Quaternion.LookRotation(rawVelocity,Vector3.up); 

        acceleration = lerped - lastFrameVelocity;
        lastFrameVelocity = lerped;
        //viz.transform.rotation = Quaternion.LookRotation(rawVelocity,Vector3.up) * Quaternion.Euler(acceleration.magnitude*100f,0,0);
        if(Physics.SphereCast(new Vector3(this.transform.position.x, this.transform.position.y + .5f, this.transform.position.z),
                                    .499f, Vector3.down,out groundHit, 1.0025f, LayerMask.GetMask("Default")))
            lerped = Vector3.ProjectOnPlane(lerped,groundHit.normal);

        //Debug.DrawRay(this.transform.position + Vector3.up,lerped,Color.green,0f,false);
        //Debug.DrawRay(this.transform.position + Vector3.up,acceleration*10f,Color.red);
        applyGravity(); //-9.8 m/s upon movement.y
        //Quaternion.LookRotation(lerped,Vector3.up);
        lerped = lerped*Time.deltaTime;
        char_ctrl.Move(lerped);
    }

    bool near(float subject, float desired){
        if(subject >= desired - near_padding && subject <= desired + near_padding)
            return true;
        return false;
    }

    RaycastHit gravHit;
    void applyGravity(){
        if(!Physics.SphereCast(new Vector3(this.transform.position.x, this.transform.position.y + .5f, this.transform.position.z),
                            .5f, Vector3.down,out gravHit, 1.0025f, LayerMask.GetMask("Default"))){
            lerped += new Vector3(0f,-5.81f,0f);
            //Debug.Log("Falling!!");
        }
    }

    private class smoothedFloat {
        float targetValue;
        float currentValue;
        float smoothValue;

        public smoothedFloat(float targetValue){
            this.targetValue = targetValue;
            currentValue = 0;
            smoothValue = .5f;
        }

        public void setTarget(float newTarget){
            targetValue = newTarget;
        }

        public void setSmoothValue(float newValue){
            smoothValue = newValue;
        }

        public float smooth(){
            if(currentValue > targetValue)
                currentValue -= (currentValue - targetValue) * .01f;
            if(currentValue < targetValue)
                currentValue -= (currentValue - targetValue) * .01f;
            if(near(currentValue,targetValue)){
                currentValue = targetValue;
            }
            //Debug.Log(currentValue);
            return currentValue;
        }

        public bool reachedTarget(){
            if(currentValue == targetValue)
                return true;
            return false;
        }

        private bool near(float subject, float desired){
        if(subject >= desired - .0025f && subject <= desired + .0025f)
            return true;
        return false;
    }
    }
}

/*
fix gravity with acceleration
look at MGSV sticky slow startup acceleration
    -fix accel curve to be nicer and fix stopping velocity (only works wellish with constant input and mouse turns right now)
        -special case for 180?
look at MGSV shift "sprint mode" where boss ducks
    for a match to start, all players must hold/press shift and enter shift mode, ready to run on start
keyframe animation



make speed of rotation and jutMode accel cap smoothly translate to the value
the keys rotate the cahracter slower than the mosue (prob not)
smoothstep?
 */