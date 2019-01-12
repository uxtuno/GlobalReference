using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(GlobalReference))]
public class GlobalReferenceEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		var target = serializedObject.targetObject as GameObject;
		var serializedGUIDProperty = serializedObject.FindProperty("serializedGUID");
		var serializedGuid = new byte[16];
		for (int i = 0; i < serializedGUIDProperty.arraySize; i++) {
			serializedGuid[i] = (byte)serializedGUIDProperty.GetArrayElementAtIndex(i).intValue;
		}

		var referenceGameObject = GUIDCollector.Instance.resolve(serializedGuid);

		if (referenceGameObject == null && serializedGuid.SequenceEqual(System.Guid.Empty.ToByteArray())) {
			EditorGUILayout.HelpBox("参照が壊れています。", MessageType.Warning);
		}
	}
}
