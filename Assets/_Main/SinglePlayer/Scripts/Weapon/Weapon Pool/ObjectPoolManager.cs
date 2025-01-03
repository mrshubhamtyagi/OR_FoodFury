using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public GameObject prefab;
    public int poolSize = 10;

    private List<GameObject> objectPool = new List<GameObject>();

    private void Start()
    {
        // Create the object pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.transform.parent = transform;
            obj.SetActive(false);
            obj.AddComponent<PoolableObject>(); // Add PoolableObject script to objects
            objectPool.Add(obj);
        }
    }

    public GameObject GetObjectFromPool()
    {
        foreach (GameObject obj in objectPool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.parent = null;
                // obj.SetActive(true);
                return obj;
            }
        }

        // If no inactive objects found, create a new one
        GameObject newObj = Instantiate(prefab);
        newObj.AddComponent<PoolableObject>(); // Add PoolableObject script to new object
        objectPool.Add(newObj);
        return newObj;
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        obj.transform.parent = transform;
        obj.GetComponent<PoolableObject>().Reset();
        obj.GetComponent<PoolableObject>().Deactivate();
    }
}
