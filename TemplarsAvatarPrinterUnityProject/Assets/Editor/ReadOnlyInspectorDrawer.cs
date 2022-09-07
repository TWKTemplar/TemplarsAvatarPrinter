using UnityEngine;
using UnityEditor;
[CustomPropertyDrawer(typeof(ReadOnlyInspectorAttribute))]
public class ReadOnlyInspectorDrawer : PropertyDrawer
{
    //The ReadOnlyInspector class is a property drawer I add to my projects that can both
    //show varables while also showing that they are READONLY without needing to tag everything as such

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true;
    }
}
