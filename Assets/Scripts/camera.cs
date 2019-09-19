using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    //CharacterController char_ctrl;
    GameObject player;
    public Component camera_comp;
    public float sensitivity = 2.5f;
    public float camera_distance = 5f;
    private float current_camera_distance;
    public float lower_cam_angle_limit = -50f;
    public float upper_cam_angle_limit = 50f;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        current_camera_distance = camera_distance;
        camera_comp = this.GetComponent(typeof(Camera));
        this.transform.position = this.transform.position + (new Vector3(0f,0f,-1*camera_distance)); //Start camera behind player
    }

    void LateUpdate()
    {
        updateCamera();
    }

    private float currentX;
    private float currentY;
    void updateCamera(){
        currentX += Input.GetAxisRaw("Mouse X");
        if(Mathf.Abs(currentX) >= 360f) //Loops angle of rotation to maintain (-360 , 360)
            currentX = Mathf.Abs(currentX) - 360f;
        currentY += Input.GetAxisRaw("Mouse Y");
        currentY = Mathf.Clamp(currentY,lower_cam_angle_limit,upper_cam_angle_limit);
        Quaternion rotation = Quaternion.Euler(currentY,currentX,0f);
        if(Input.GetKey(KeyCode.Mouse4))
            current_camera_distance -= 1.0f*Time.deltaTime;
        else if(Input.GetKey(KeyCode.Mouse3))
            current_camera_distance += 1.0f*Time.deltaTime;
        else
            maintainCamDistance();
        this.transform.position = player.transform.position - (rotation*new Vector3(0f,0f,current_camera_distance)); //moves camera with target after Move() was executed
        this.transform.LookAt(player.transform);
        Debug.DrawRay(this.transform.position,(this.transform.position - this.transform.position)*-1,Color.blue,0f,false);
        Debug.DrawRay(this.transform.position,(this.transform.position - this.transform.position).normalized,Color.blue,0f,false);
        this.transform.position += Vector3.up;
    }

    void maintainCamDistance(){ //Keeps the camera "camera_distance" meters away from the player, with a clamp
        // Debug.Log(current_camera_distance);
        if(!(current_camera_distance == camera_distance)){
            if(Mathf.Abs(current_camera_distance - camera_distance) <= .001f) //If it gets really close, just set it back, it's impossible to tell (maxDelta)
                current_camera_distance = camera_distance; //[if it ends up overshooting when maintaining, the above line + the step interval is why]
            else{
                if(current_camera_distance < camera_distance) //Too close    (move camera outward to camera_distance)
                    current_camera_distance += 1.0f*Time.deltaTime;
                else if(current_camera_distance > camera_distance) //Too far away    (move camera inward to camera_distance)
                    current_camera_distance -= 1.0f*Time.deltaTime;
            }
        }
    }
}

