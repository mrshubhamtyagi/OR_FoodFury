using System;
using UnityEngine;
using static Enums;

namespace FoodFury
{
    [DefaultExecutionOrder(-12)]
    public class PlayerPrefsManager : MonoBehaviour
    {
        [field: SerializeField] public int SfxVol { get; private set; }
        [field: SerializeField] public int MusicVol { get; private set; }
        [field: SerializeField] public int HapticsState { get; private set; }
        [field: SerializeField] public GraphicsMode Graphics { get; private set; }
        [field: SerializeField] public ServerMode ServerMode { get; private set; }

        private string PlayerIdKey => "PlayerId";
        private string ViaKey => "Via";
        private string ServerModeKey => "ServerMode";

        private string MusicKey => "FoodFuryMusic";
        private string SfxKey => "FoodFurySfx";
        private string HapticsKey => "FoodFuryHaptics";
        private string GraphicsKey => "FoodFuryGraphics";
        private string ControlKey => "FoodFuryControl";

        public string Via { get; private set; }
        public string PlayerId { get; private set; }

        public static PlayerPrefsManager Instance;

        void Awake()
        {
            if (Instance != null) return;

            Instance = this;
            ReadPlayerPrefs();

            if (ServerMode != GameData.Instance.serverMode)
                RemoveLoginData();
        }

        public void RemoveLoginData()
        {
            RemovePlayerId();
            RemoveVia();
        }

        [ContextMenu("ResetPlayerPrefs")]
        private void ResetPlayerPrefs() => PlayerPrefs.DeleteAll();


        private void ReadPlayerPrefs()
        {
            PlayerId = PlayerPrefs.GetString(PlayerIdKey, "");
            MusicVol = PlayerPrefs.GetInt(MusicKey, 2);
            SfxVol = PlayerPrefs.GetInt(SfxKey, 5);
            HapticsState = PlayerPrefs.GetInt(HapticsKey, 1);
            //GameData.Instance.Controls = Enum.Parse<Enums.InputType>(PlayerPrefs.GetString(ControlKey, Enums.InputType.Arrows.ToString()));
            Graphics = Enum.Parse<GraphicsMode>(PlayerPrefs.GetString(GraphicsKey, GraphicsMode.Balanced.ToString()));
            Via = PlayerPrefs.GetString(ViaKey, "");
            ServerMode = (ServerMode)Enum.Parse(typeof(ServerMode),
                PlayerPrefs.GetString(ServerModeKey, GameData.Instance.serverMode.ToString()));
        }

        public void SavePlayerId(string _id) => PlayerPrefs.SetString(PlayerIdKey, PlayerId = _id);

        public void RemovePlayerId()
        {
            PlayerId = "";
            PlayerPrefs.DeleteKey(PlayerIdKey);
        }

        public void SaveMusicVol(int _value)
        {
            MusicVol = _value;
            PlayerPrefs.SetInt(MusicKey, _value);
            PlayerPrefs.Save();
        }

        public void SaveSFXVol(int _value)
        {
            SfxVol = _value;
            PlayerPrefs.SetInt(SfxKey, _value);
            PlayerPrefs.Save();
        }

        public void SaveHaptics(int _value)
        {
            HapticsState = _value;
            PlayerPrefs.SetInt(HapticsKey, _value);
            PlayerPrefs.Save();
        }

        public void SwitchGraphics(GraphicsMode _mode) => Graphics = _mode;

        public void SaveGraphics()
        {
            PlayerPrefs.SetString(GraphicsKey, Graphics.ToString());
            PlayerPrefs.Save();
        }

        [ContextMenu("Save Server Mode")]
        public void SaveServerMode()
        {
            ServerMode = GameData.Instance.serverMode;
            PlayerPrefs.SetString(ServerModeKey, ServerMode.ToString());
        }

        public void SaveVia(string _via) => PlayerPrefs.SetString(ViaKey, _via);
        public void RemoveVia() => PlayerPrefs.DeleteKey(ViaKey);
    }
}