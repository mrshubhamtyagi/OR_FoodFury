using UnityEngine;

namespace FoodFury
{
    public class DrivingTestCollision : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (TutorialManager.Instance.result == TutorialManager.ChallengeReseult.Pending)
            {
                if (collision.collider.CompareTag("Player"))
                {
                    TutorialManager.Instance.OnStepFailed();
                    collision.gameObject.GetComponent<Rider>().OnVehicleDamage?.Invoke(0.5f);
                }
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            // Booster
            if (other.CompareTag("Player") && other.TryGetComponent(out Rider _rider))
            {
                TutorialScreen.Instance.ToggleBooster(true);
                _rider.Vehicle.SetMaxSpeed(_rider.Vehicle.VehicleConfig.Speed * 1.5f);
                gameObject.SetActive(false);
            }
        }
    }
}
