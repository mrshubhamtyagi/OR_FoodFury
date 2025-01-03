using UnityEngine;
using UnityEngine.AI;

public class AIVehicleNavmesh : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform target;
    public Vector3[] corners;





    void Awake()
    {

    }

    private void Update()
    {
        SetPath();
    }


    [ContextMenu("SetPath")]
    void SetPath()
    {
        agent.SetDestination(target.position);

        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(target.position, path);
        corners = path.corners;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        for (int i = 0; i < corners.Length; i++)
        {
            Gizmos.color = i == 0 ? Color.yellow : i == 1 ? Color.red : Color.cyan;
            Gizmos.DrawWireSphere(corners[i], 2);
        }

        //foreach (var item in corners)
        //{
        //    Gizmos.DrawWireSphere(item, 2);
        //}
    }
}
