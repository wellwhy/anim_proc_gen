using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class SerializePose : MonoBehaviour
{
    public AnimationClip poseClip;
    public bool createFlippedKeyframe = true;
    
    string path, path_f;
    Transform[] boneTransforms, boneTransforms_f;
    KeyPose pose, pose_f;
    Animation parent;

    void Start()
    {
        Regex removeCharsFromFilename = new Regex("[^a-zA-Z0-9 -]");
        path = Application.dataPath + "/Poses/" + removeCharsFromFilename.Replace(poseClip.name,"") + ".json";
        path_f = Application.dataPath + "/Poses/" + removeCharsFromFilename.Replace(poseClip.name,"") + "_f.json";

        boneTransforms = gameObject.GetComponentsInChildren<Transform>();
        // boneTransforms_f = (Transform[]) boneTransforms.Clone();

        parent = gameObject.GetComponentInParent<Animation>();
        parent.clip = poseClip;

        poseClip.SampleAnimation(parent.gameObject,1f);
        pose = new KeyPose(boneTransforms, false);
        pose_f = new KeyPose(boneTransforms, true);


        storeBoneTransforms();
    }

    void storeBoneTransforms(){
        string json = JsonUtility.ToJson(pose);
        string json_f = JsonUtility.ToJson(pose_f);
        if(!File.Exists(path)){
            File.WriteAllText(path,json);
        }
        if(!File.Exists(path_f)){
            File.WriteAllText(path_f,json_f);
        }
        // else{
        //     string filez = File.ReadAllText(path);
        //     KeyPose testPose = JsonUtility.FromJson<KeyPose>(filez);
        //     for(int i = 0; i < pose.boneOffsets.Length; i++)
        //     {
        //         float b = testPose.boneOffsets[i].x;
        //         float n = pose.boneOffsets[i].x;
        //         if(b!=n)
        //             Debug.Log("FUK");
        //     }
        // }
    }
}