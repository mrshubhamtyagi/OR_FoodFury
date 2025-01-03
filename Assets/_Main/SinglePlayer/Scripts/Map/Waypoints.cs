using UnityEditor;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    [field: SerializeField] public Vector3 SpawnPosition { get; private set; }
    [field: SerializeField] public Vector3 SpawnRotation { get; private set; }

    [HideInInspector] public float nextTargetRadius = 4;

    [Header("-----Gizmo")]
    public bool drawGizmo = false;
    public bool drawSelected = false;
    public Color gizmoColor = Color.white;
    public float radius = 2;


    private void OnDrawGizmosSelected()
    {
        if (!drawSelected || transform.childCount == 0) return;

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.GetChild(0).position, radius);
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            if (!transform.GetChild(i).gameObject.activeSelf) continue;

            Vector3 _from = transform.GetChild(i).position;
            Vector3 _to = transform.GetChild(i + 1).position;
            Gizmos.DrawLine(_from, _to);

            Gizmos.DrawWireSphere(_to, radius);

#if UNITY_EDITOR
            Handles.DrawWireDisc(_to, Vector2.up, nextTargetRadius);
#endif
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmo || transform.childCount == 0) return;

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.GetChild(0).position, radius);
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            if (!transform.GetChild(i).gameObject.activeSelf) continue;

            Vector3 _from = transform.GetChild(i).position;
            Vector3 _to = transform.GetChild(i + 1).position;
            Gizmos.DrawLine(_from, _to);

            Gizmos.DrawWireSphere(_to, radius);

#if UNITY_EDITOR
            Handles.DrawWireDisc(_to, Vector2.up, nextTargetRadius);
#endif
        }
    }
}