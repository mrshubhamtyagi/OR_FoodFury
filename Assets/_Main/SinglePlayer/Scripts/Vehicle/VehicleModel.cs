using UnityEngine;

public class VehicleModel : MonoBehaviour
{
    [field: SerializeField] public int VehicleID { get; private set; }
    public Transform handle;
    public Transform body;
    public Transform primaryWheel;
    public EngineSoundSO normalEngineSound;
    public EngineSoundSO brokenEngineSound;
    //public Transform otherWheel;
}
