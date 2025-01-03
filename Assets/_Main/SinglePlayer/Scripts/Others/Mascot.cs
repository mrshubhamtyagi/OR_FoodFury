using FoodFury;
using UnityEngine;

public class Mascot : MonoBehaviour
{
    [SerializeField] public Transform mascot;
    private float countdown = 5f;
    private bool startCountdown;

    private void OnEnable()
    {
        countdown = 5f;
    }

    private void OnBecameVisible() => startCountdown = true;

    private void Update()
    {
        if (!startCountdown)
            return;

        if (countdown < 0)
        {
            startCountdown = false;
            DestroyMascot();
        }
        else countdown -= Time.deltaTime;
    }
    public void DestroyMascot()
    {
        if (gameObject != null)
        {
            if (MascotSpawnManager.Instance != null)
                MascotSpawnManager.Instance.ResetMascot();
            Destroy(gameObject);

        }
    }


}
