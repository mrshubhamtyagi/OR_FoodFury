using UnityEngine;

namespace FoodFury
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private Rider rider;
        [SerializeField] private int currentStep = 0;

        public ChallengeReseult result = ChallengeReseult.Pending;
        public enum ChallengeReseult { Pending, Success, Failed }


        [Header("-----Steps Info")]
        [SerializeField] public TutorialStep[] steps;


        public static TutorialManager Instance;
        private void Awake() => Instance = this;

        void Start()
        {
            foreach (var item in steps)
                item.stepParent.SetActive(false);

            InitStep();
        }


        private void InitStep()
        {
            rider.Vehicle.enabled = false;
            rider.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            rider.Vehicle.rotationAngleY = steps[currentStep].riderRotation.y;
            rider.transform.SetPositionAndRotation(steps[currentStep].riderPosition, Quaternion.Euler(steps[currentStep].riderRotation));
            rider.Vehicle.SetMaxSpeed(rider.Vehicle.VehicleData.initial.speed);

            steps[currentStep].stepParent.SetActive(true);
            steps[currentStep].finishingPoint.OnReached += OnStepCompleted;

            Invoke("EnableVehicle", 1);
            Loader.Instance.HideLoader();
        }

        private void EnableVehicle() => rider.Vehicle.enabled = true;


        [ContextMenu("OnStepCompleted")]
        public void OnStepCompleted()
        {
            result = ChallengeReseult.Success;
            AnalyticsManager.Instance.FireLessonCompletedEvent(currentStep);
            rider.Vehicle.VehicleSound.ToggleSound(false);
            TutorialScreen.Instance.OnChallengeCompleted();
            TutorialScreen.Instance.TogglePassedScreen(true, () =>
            {
                steps[currentStep].finishingPoint.OnReached -= OnStepCompleted;
                steps[currentStep].stepParent.SetActive(false);

                currentStep++;
                if (currentStep == steps.Length) Invoke("OnTutorialsCompleted", 2);
                else
                {
                    InitStep();
                    TutorialScreen.Instance.InvokeTapToContinue(3);
                }
            });
        }
        [ContextMenu("OnStepSkipped")]

        public void OnStepSkipped()

        {

            result = ChallengeReseult.Success;

            AnalyticsManager.Instance.FireLessonSkippedEvent(currentStep);

            rider.Vehicle.VehicleSound.ToggleSound(false);

            TutorialScreen.Instance.OnChallengeCompleted();

            TutorialScreen.Instance.TogglePassedScreen(true, () =>
            {
                steps[currentStep].finishingPoint.OnReached -= OnStepCompleted;
                steps[currentStep].stepParent.SetActive(false);
                currentStep++;
                if (currentStep == steps.Length) Invoke("OnTutorialsCompleted", 2);
                else
                {
                    InitStep();
                    TutorialScreen.Instance.InvokeTapToContinue(3);
                }
            });
        }

        [ContextMenu("OnStepFailed")]
        public void OnStepFailed()
        {
            result = ChallengeReseult.Failed;
            AnalyticsManager.Instance.FireLessonFailedEvent(currentStep);
            steps[currentStep].finishingPoint.OnReached -= OnStepCompleted;
            steps[currentStep].stepParent.SetActive(false);

            TutorialScreen.Instance.OnChallengeFailed();
            TutorialScreen.Instance.ToggleFailedScreen(true, () =>
            {
                InitStep();
                TutorialScreen.Instance.InvokeTapToContinue(3);
            });
        }



        [ContextMenu("OnTutorialsCompleted")]
        private void OnTutorialsCompleted()
        {
            Debug.Log("OnTutorialsCompleted");
            AnalyticsManager.Instance.FireLicenseRecievedEvent();
            TutorialScreen.Instance.ShowCongratulations();
        }


    }




    [System.Serializable]
    public class TutorialStep
    {
        [TextArea] public string stepInfo;
        public Vector3 riderPosition;
        public Vector3 riderRotation;
        public GameObject stepParent;
        public FinishingPoint finishingPoint;
        public Renderer nft;
        public GameObject tag;

        [Header("-----Order")]
        public Texture2D orderThumb;
        public string dishName;
        public int orderTimeout;
    }
}