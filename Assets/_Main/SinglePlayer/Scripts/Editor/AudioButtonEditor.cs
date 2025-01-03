using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(AudioButton))]
public class AudioButtonEditor : ButtonEditor
{
    public override void OnInspectorGUI()
    {
        AudioButton _button = (AudioButton)target;
        //  _button.clip = (AudioClip)EditorGUILayout.ObjectField("AudioClip", _button.clip, typeof(AudioClip), true);

        base.OnInspectorGUI();
    }
}
