using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class boneMovement : MonoBehaviour
{
    Transform[] boneTransforms, left, right;
    int left_count, right_count;
    KeyPose testPose, testPose2, testPose3, testPose4;
    KeyPose testPose_w, testPose2_w, testPose3_w, testPose4_w;
    KeyPose current, goingTo, lerpedPose_run;
    KeyPose current_w, goingTo_w, lerpedPose_w;
    KeyPose stand, standLerp;
    KeyPose finalLerped;
    int pose = 1;
    bool endslow = false;
    bool linear = false;
    public AnimationCurve bicubic, singleBicubic;
    GameObject player;
    controller player_controller;
    float distTravelled;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player_controller = player.GetComponent<controller>();
        boneTransforms = gameObject.GetComponentsInChildren<Transform>();
        string path = Application.dataPath + "/Poses/";
        testPose4_w = JsonUtility.FromJson<KeyPose>(File.ReadAllText(path + "ArmatureWalkPass.json"));
        testPose3_w = JsonUtility.FromJson<KeyPose>(File.ReadAllText(path + "ArmatureWalkReach.json"));
        testPose2_w = JsonUtility.FromJson<KeyPose>(File.ReadAllText(path + "ArmatureWalkPass_f.json"));
        testPose_w = JsonUtility.FromJson<KeyPose>(File.ReadAllText(path + "ArmatureWalkReach_f.json"));


        testPose4 = JsonUtility.FromJson<KeyPose>(File.ReadAllText(path + "ArmaturePass.json"));
        testPose3 = JsonUtility.FromJson<KeyPose>(File.ReadAllText(path + "ArmatureReach.json"));
        testPose2 = JsonUtility.FromJson<KeyPose>(File.ReadAllText(path + "ArmaturePass_f.json"));
        testPose = JsonUtility.FromJson<KeyPose>(File.ReadAllText(path + "ArmatureReach_f.json"));

        stand = JsonUtility.FromJson<KeyPose>(File.ReadAllText(path + "ArmatureStand.json"));

        current = testPose4;
        goingTo = testPose;

        current_w = testPose4_w;
        goingTo_w = testPose_w;
        // left = new Transform[10];
        // right = new Transform[10];

        // foreach(Transform boneTransform in boneTransforms){
        //     if(boneTransform.gameObject.name.Contains(".L")){
        //         left[left_count] = boneTransform;
        //         left_count++;
        //     }
        //     if(boneTransform.gameObject.name.Contains(".R")){
        //         right[right_count] = boneTransform;
        //         right_count++;
        //     }
        // }

    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        distTravelled += player_controller.lerped.magnitude;
        //        Debug.Log(distTravelled);
        if (distTravelled >= .25f){
            pose++;
            if(pose == 5)
                pose = 1;
            distTravelled = 0;

            switch(pose){
                case 1:
                //liner
                    endslow = true;
                    current = testPose4;
                    goingTo = testPose;

                    current_w = testPose4_w;
                    goingTo_w = testPose_w;
                    break;
                case 2:
                //lin
                    endslow = false;
                    current = testPose;
                    goingTo = testPose2;

                    current_w = testPose_w;
                    goingTo_w = testPose2_w;
                    break;
                case 3:
                //end slow
                    endslow = true;
                    current = testPose2;
                    goingTo = testPose3;

                    current_w = testPose2_w;
                    goingTo_w = testPose3_w;
                    break;
                case 4:
                //start slow
                    endslow = false;
                    current = testPose3;
                    goingTo = testPose4;

                    current_w = testPose3_w;
                    goingTo_w = testPose4_w;
                    break;
            }
            //Debug.Log(pose + " " + endslow);
        }
                if(endslow){
                    lerpedPose_run = KeyPose.lerp(current,goingTo,singleBicubic.Evaluate(distTravelled/.25f));
                    lerpedPose_w = KeyPose.lerp(current_w,goingTo_w,singleBicubic.Evaluate(distTravelled/.25f));
                }
                else{
                    lerpedPose_run = KeyPose.lerp(current,goingTo,bicubic.Evaluate(distTravelled/.25f));
                    lerpedPose_w = KeyPose.lerp(current_w,goingTo_w,bicubic.Evaluate(distTravelled/.25f));
                }
                standLerp = KeyPose.lerp(stand,lerpedPose_w,bicubic.Evaluate((player_controller.lerped.magnitude/Time.deltaTime) / 1f));
                finalLerped = KeyPose.lerp(standLerp,lerpedPose_run,bicubic.Evaluate((player_controller.lerped.magnitude/Time.deltaTime) / 2.2f));
                //Debug.Log(player_controller.lerped.magnitude/Time.deltaTime);
        for (int i = 0; i < boneTransforms.Length; i++){
                    boneTransforms[i].localPosition = finalLerped.boneOffsets[i].localPosition;
                    boneTransforms[i].localRotation = finalLerped.boneOffsets[i].localRotation;
        }
    }
}

/*
Make every animation that requires mirroring (like running) export a name.json and name_m.json
find all transforms with an .L and .R and...
    export the normal json
    export a json with these transforms swapped, as well as flipping one of the axes (idk which one) and all or one axis of rotation (Quaternion so idk?)




 */
