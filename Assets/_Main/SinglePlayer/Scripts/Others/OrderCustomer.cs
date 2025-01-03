using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderCustomer : MonoBehaviour
{
    [SerializeField] private Transform parent;
    private void Start()
    {
        int i = Random.Range(0, parent.childCount);
        parent.GetChild(i).gameObject.SetActive(true);
    }
}
