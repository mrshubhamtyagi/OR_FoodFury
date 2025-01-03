using System;
using UnityEngine;

public class NetworkChecker : MonoBehaviour
{
    public static event Action<bool, NetworkReachability> OnNetworkCheckReachability;

    public int repeatRate = 0;


    public static NetworkChecker Instance { get; private set; }
    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        if (repeatRate > 0) StartInvoke();
    }

    public void StartInvoke() => InvokeRepeating("NetworkCheckInvoke", 0f, repeatRate);

    public bool CheckNetwork(out NetworkReachability _networkReachability)
    {
        _networkReachability = Application.internetReachability;
        print("Network Check - " + _networkReachability.ToString());
        return Application.internetReachability != NetworkReachability.NotReachable;
    }


    private void NetworkCheckInvoke()
    {
        bool _isWorking = CheckNetwork(out NetworkReachability _reachability);
        OnNetworkCheckReachability?.Invoke(_isWorking, _reachability);
    }

}