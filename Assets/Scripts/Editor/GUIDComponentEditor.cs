using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GUIDComponent))]
public class GUIDComponentEditor : Editor
{
	SerializedProperty guidProperty;

	private void OnEnable()
	{
		guidProperty = serializedObject.FindProperty("showGUID");
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		//if (guidProperty.stringValue == System.Guid.Empty.ToString()) {
		//	EditorGUILayout.HelpBox("GUIDが不正です。\n再生成を行うなど、修正を試みてください。", MessageType.Error);
		//}
	}
}
