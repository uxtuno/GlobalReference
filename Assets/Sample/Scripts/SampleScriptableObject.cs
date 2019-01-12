using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SampleScriptableObject : ScriptableObject
{
	[MenuItem("Tool/CreateScriptableObject")]
	static void create()
	{
		var instance = CreateInstance<SampleScriptableObject>();
		AssetDatabase.CreateAsset(instance, "Assets/SampleScriptableObject.asset");
		AssetDatabase.Refresh();
	}

	/// <summary>
	/// 何も指定しなければ GameObject をバインドできる ObjectField をインスペクタ上に表示
	/// </summary>
	public GlobalReference GameObjectReference;

	/// <summary>
	/// バインド出来る型を Rigidbody のみに制限
	/// </summary>
	[GlobalReferenceTypeBinding(typeof(Rigidbody))]
	public GlobalReference RigidBodyReference;

}
