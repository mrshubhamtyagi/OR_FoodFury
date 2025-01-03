using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class AudioModule<T> : MonoBehaviour
{
    public string HintText;
    //public Action<T> OnValueChanged;
    public virtual void Apply(T value)
    {

    }
}
