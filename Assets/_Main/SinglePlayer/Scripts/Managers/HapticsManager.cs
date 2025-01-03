#if !UNITY_WEBGL
using CandyCoded.HapticFeedback;
#endif
namespace FoodFury
{
    public class HapticsManager
    {
        public static void LightHaptic()
        {
            if (PlayerPrefsManager.Instance.HapticsState == 0) return;
#if !UNITY_WEBGL
            HapticFeedback.LightFeedback();
#endif
        }
        public static void MediumHaptic()
        {
            if (PlayerPrefsManager.Instance.HapticsState == 0) return;
#if !UNITY_WEBGL
            HapticFeedback.MediumFeedback();
#endif
        }
        public static void StrongHaptic()
        {
            if (PlayerPrefsManager.Instance.HapticsState == 0) return;
#if !UNITY_WEBGL
            HapticFeedback.HeavyFeedback();
#endif
        }
    }

}
