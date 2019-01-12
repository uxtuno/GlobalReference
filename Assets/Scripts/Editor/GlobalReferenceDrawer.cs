using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using UnityEditor.SceneManagement;

[CustomPropertyDrawer(typeof(GlobalReference), true)]
public class GlobalReferenceDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		position.height = EditorGUIUtility.singleLineHeight;

		var target = property.serializedObject.targetObject as GameObject;
		var serializedGUIDProperty = property.FindPropertyRelative("SerializedGUID");
		var serializedGuid = new byte[16];
		for (int i = 0; i < serializedGUIDProperty.arraySize; i++) {
			serializedGuid[i] = (byte)serializedGUIDProperty.GetArrayElementAtIndex(i).intValue;
		}

		// GUID から参照を解決する
		var referenceGameObject = GUIDCollector.Instance.resolve(serializedGuid);

		var defaultGUIColor = GUI.color;
		// 参照が壊れている (正常に参照できなかった)
		if (referenceGameObject == null && !serializedGuid.SequenceEqual(System.Guid.Empty.ToByteArray())) {
			GUI.color = Color.red;
		}

		// GlobalReferenceTypeBindingAttribute 属性が指定されている場合は、特定の型のみをバインドできるように処理する
		var targetField = property.serializedObject.targetObject.GetType().GetField(property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		System.Type bindingType = typeof(GameObject);
		if (targetField != null) {
			var bindingAttribute = targetField.GetCustomAttribute<GlobalReferenceTypeBindingAttribute>();
			bindingType = bindingAttribute != null ? bindingAttribute.BindType : typeof(GameObject);
		}

		using (var check = new EditorGUI.ChangeCheckScope()) {
			var modified = false;

			var bindObject = EditorGUI.ObjectField(position, label, referenceGameObject, bindingType, true);

			if (check.changed) {
				var guidComponent = conditionalFindGUIDComponent(bindObject);
				if (!guidComponent) {
					guidComponent = conditionalAddGUIDComponent(bindObject);

					// 正常にコンポーネントを追加できた
					if (!!guidComponent) {
						modified = true;
					}
				}

				if (!!guidComponent) {
					// 関連する GUID が存在しなければ、新しいGUIDを作成する
					var guid = guidComponent.getGUID(bindObject);
					if (guid == System.Guid.Empty) {
						guidComponent.createGUID(bindObject);
						modified = true;
					}
					setGUIDToProperty(serializedGUIDProperty, guidComponent.getGUID(bindObject));
				}
				else {
					setGUIDToProperty(serializedGUIDProperty, System.Guid.Empty);
				}
			}

			if (!!modified) {
				if (bindObject is GameObject) {
					EditorSceneManager.MarkSceneDirty(((GameObject)bindObject).scene);
				} else if (bindObject is Component) {
					EditorSceneManager.MarkSceneDirty(((Component)bindObject).gameObject.scene);
				}
			}
		}

		// 元の状態に戻す
		GUI.color = defaultGUIColor;
	}

	/// <summary>
	/// 指定されたオブジェクトから GUIDComponent を取得する
	/// </summary>
	/// <param name="target"></param>
	/// <returns></returns>
	GUIDComponent conditionalFindGUIDComponent(Object target)
	{
		if (target is GameObject) {
			return ((GameObject)target).GetComponent<GUIDComponent>();
		}
		else if (target is Component) {
			return ((Component)target).GetComponent<GUIDComponent>();
		}

		return null;
	}

	/// <summary>
	/// 指定されたオブジェクトに対して GUIDComponent を追加する
	/// 
	/// </summary>
	/// <param name="target"></param>
	/// <returns></returns>
	GUIDComponent conditionalAddGUIDComponent(Object target)
	{
		if (target is GameObject) {
			return ((GameObject)target).AddComponent<GUIDComponent>();
		}
		else if (target is Component) {
			return ((Component)target).gameObject.AddComponent<GUIDComponent>();
		}

		return null;
	}

	/// <summary>
	/// GUID をSerializedProperty に対して設定する
	/// ここで指定する SerializedProperty は、byte 配列であること
	/// </summary>
	/// <param name="property"></param>
	/// <param name="guid"></param>
	void setGUIDToProperty(SerializedProperty property, System.Guid guid)
	{
		var guidToByteArray = guid.ToByteArray();

		property.ClearArray();
		for (int i = 0; i < 16; ++i) {
			property.InsertArrayElementAtIndex(i);
			var element = property.GetArrayElementAtIndex(i);
			element.intValue = guidToByteArray[i];
		}
	}

	/// <summary>
	/// 強制的に表示を更新
	/// </summary>
	/// <param name="property"></param>
	/// <returns></returns>
	public override bool CanCacheInspectorGUI(SerializedProperty property)
	{
		return false;
	}
}
