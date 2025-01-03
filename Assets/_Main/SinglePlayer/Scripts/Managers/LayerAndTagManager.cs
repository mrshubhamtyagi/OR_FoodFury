using UnityEngine;

[DefaultExecutionOrder(-9)]
public class LayerAndTagManager : MonoBehaviour
{
    [field: SerializeField] public LayerMask LayerPlayer { get; private set; }
    [field: SerializeField] public LayerMask LayerCityCollider { get; private set; }
    [field: SerializeField] public LayerMask LayerRoadTile { get; private set; }
    [field: SerializeField] public LayerMask LayerObstacle { get; private set; }
    [field: SerializeField] public LayerMask LayerGround { get; private set; }

    [field: SerializeField] public string TagPlayer { get; private set; }
    [field: SerializeField] public string TagDamageCollider { get; private set; }

    public static LayerAndTagManager Instance { get; private set; }
    private void Awake() => Instance = this;

}
