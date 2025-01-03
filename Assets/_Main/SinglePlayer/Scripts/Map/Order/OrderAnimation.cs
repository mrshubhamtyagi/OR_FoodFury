using UnityEngine;

public class OrderAnimation : MonoBehaviour
{
    [SerializeField] private float lerpValue = 0.3f;
    [SerializeField] private Vector3 min;
    [SerializeField] private Vector3 max;
    Vector3 _finalPosition;

    void Start()
    {
    }

    void Update()
    {
        transform.localPosition = Vector3.Slerp(transform.localPosition, _finalPosition, lerpValue * Time.deltaTime);
        if (transform.localPosition.y > max.y - 0.25f) _finalPosition = min;
        else if (transform.localPosition.y < min.y + 0.25f) _finalPosition = max;
    }
}
