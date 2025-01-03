using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public Vector3 offset;
    [Range(0.1f, 10f)] public float speed;

    private Camera cam;
    private Rigidbody playerRB;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        //playerRB = target.GetComponent<Rigidbody>();
    }

    public void SetOffset(Vector2 _offset) => offset = new Vector3(0, _offset.x, _offset.y);
    public void SetFOV(int _value) => cam.fieldOfView = _value;

    void FixedUpdate()
    {
        FollowTarget();
    }

    //public void CamerRotation() => moveOnly = !moveOnly;

    private void FollowTarget()
    {
        Vector3 playerForward = (playerRB.linearVelocity + target.transform.forward).normalized;
        transform.position = Vector3.Lerp(transform.position, target.position + target.transform.TransformVector(offset) + playerForward * (-5f), speed * Time.fixedDeltaTime);
        transform.LookAt(target);
    }

}