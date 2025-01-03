using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObject : MonoBehaviour
{

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Reset()
    {
        // Reset any object-specific properties here
        // For example, reset position and rotation
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }
}

