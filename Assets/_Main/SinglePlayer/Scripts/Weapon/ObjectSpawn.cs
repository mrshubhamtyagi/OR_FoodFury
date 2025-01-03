using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ObjectSpawn : MonoBehaviour
{
    public Vector3 offset;
    private void OnEnable()
    {
        transform.localPosition = offset;
    }
}
