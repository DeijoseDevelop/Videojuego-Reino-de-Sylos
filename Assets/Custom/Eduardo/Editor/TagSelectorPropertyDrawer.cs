using UnityEditor;
using UnityEngine;

/// <summary>
/// Drawer que muestra un dropdown con todas las Tags disponibles para campos string marcados con [TagSelector].
/// Debe residir en una carpeta llamada "Editor".
/// </summary>
[CustomPropertyDrawer(typeof(TagSelectorAttribute))]
public class TagSelectorPropertyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (property.propertyType != SerializedPropertyType.String)
		{
			EditorGUI.PropertyField(position, property, label);
			return;
		}

		string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
		int currentIndex = Mathf.Max(0, System.Array.IndexOf(tags, string.IsNullOrEmpty(property.stringValue) ? "Untagged" : property.stringValue));
		int newIndex = EditorGUI.Popup(position, label.text, currentIndex, tags);
		if (newIndex >= 0 && newIndex < tags.Length)
		{
			property.stringValue = tags[newIndex];
		}
	}
}


