using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KeyPose
{
    public BoneOffset[] boneOffsets;

    public KeyPose(){
    }

    public KeyPose(int size){
        boneOffsets = new BoneOffset[size];
    }
    
    public KeyPose(Transform[] boneTransforms, bool flip){
        boneOffsets = new BoneOffset[boneTransforms.Length];
        for(int i = 0; i < boneOffsets.Length; i++)
        {
            boneOffsets[i] = new BoneOffset(boneTransforms[i].localPosition, boneTransforms[i].localRotation);
        }
        //1 2 3 4 5 6 7 8 9 10     12 13 14 15     18 19 20 21 22 23 24

        if(flip){
            for(int i = 0; i < boneOffsets.Length; i++){
                string boneName = boneTransforms[i].gameObject.name;
                string boneNamePlain = boneName.Remove(boneName.Length-2);
                if(boneName.Contains(".L")){
                    BoneOffset temp = boneOffsets[i];
                    int r_index = findBoneTransformIndex(boneNamePlain + ".R", ref boneTransforms);
                    boneOffsets[i] = boneOffsets[r_index];
                    boneOffsets[i].localPosition = new Vector3(boneOffsets[i].localPosition.x*-1,boneOffsets[i].localPosition.y,boneOffsets[i].localPosition.z);
                    boneOffsets[i].localRotation = new Quaternion(boneOffsets[i].localRotation.x*-1,boneOffsets[i].localRotation.y,boneOffsets[i].localRotation.z,boneOffsets[i].localRotation.w*-1);

                    boneOffsets[r_index] = temp;
                    boneOffsets[r_index].localPosition = new Vector3(boneOffsets[r_index].localPosition.x*-1,boneOffsets[r_index].localPosition.y,boneOffsets[r_index].localPosition.z);
                    boneOffsets[r_index].localRotation = new Quaternion(boneOffsets[r_index].localRotation.x*-1,boneOffsets[r_index].localRotation.y,boneOffsets[r_index].localRotation.z,boneOffsets[r_index].localRotation.w*-1);
                }
            }
        }
    }

    private int findBoneTransformIndex(string boneName, ref Transform[] boneTransforms){
        for(int i = 0; i < boneTransforms.Length; i++){
            if(boneTransforms[i].gameObject.name.Equals(boneName))
                return i;
        }
        return -1; //shouldn't fail
    }

    public static KeyPose lerp(KeyPose start, KeyPose end, float amount){
        KeyPose result = new KeyPose(start.boneOffsets.Length);
        for(int i = 0; i < start.boneOffsets.Length; i++){
            result.boneOffsets[i] = new BoneOffset(Vector3.Lerp(start.boneOffsets[i].localPosition,end.boneOffsets[i].localPosition,amount),
                                                   Quaternion.Lerp(start.boneOffsets[i].localRotation,end.boneOffsets[i].localRotation,amount));
        }
        return result;
    }
}
