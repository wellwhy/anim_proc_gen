using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoneOffset
{
    public Vector3 localPosition;
    public Quaternion localRotation;

    public BoneOffset(Vector3 localPosition, Quaternion localRotation){
        this.localPosition = localPosition;
        this.localRotation = localRotation;
    }
}
