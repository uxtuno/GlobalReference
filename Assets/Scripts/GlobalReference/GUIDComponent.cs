using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;

/// <summary>
/// オブジェクトと GUID を関連付けて保存するクラス
/// </summary>
[System.Serializable]
struct GUIDObject : ISerializationCallbackReceiver
{
	public GUIDObject(Object target, System.Guid guid)
	{
		Target = target;

		GUID = guid;
		SerializedGUID = guid.ToByteArray();
	}

	/// <summary>
	/// 関連付けるオブジェクト
	/// </summary>
	public Object Target;

	/// <summary>
	/// 関連付けるGUID
	/// </summary>
	public byte[] SerializedGUID;

	public System.Guid GUID { get; private set; }

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		GUID = new System.Guid(SerializedGUID);
	}
}

#if UNITY_EDITOR
[System.Serializable]
struct ShowGUIDObject
{
	public ShowGUIDObject(string name, string guid)
	{
		Name = name;
		GUID = guid;
	}
	public string Name;
	public string GUID;
}
#endif

/// <summary>
/// オブジェクトに対して GUID を割り当てるコンポーネント
/// 同じ GUID が割り当てられたオブジェクトが同時に複数存在しないように注意すること
/// このコンポーネントが割り当てられたゲームオブジェクトが複製された場合は、新しいGUIDが割り当てられる
/// </summary>
[ExecuteInEditMode]
public class GUIDComponent : MonoBehaviour, ISerializationCallbackReceiver
{
	[SerializeField, HideInInspector]
	List<GUIDObject> GUIDObjects = new List<GUIDObject>();

#if UNITY_EDITOR
	[SerializeField, DisableInInspector]
	ShowGUIDObject[] ShowGUIDObjects;
#endif

	/// <summary>
	/// GUID が登録済みかどうか
	/// 基本的に、Sceneがロードされたときなどに、
	/// 初回のみ登録を行うために使用
	/// </summary>
	[System.NonSerialized]
	bool registerGUID = false;

	void OnEnable()
	{
		if (!!registerGUID) {
			return;
		}
		registerGUID = true;

		// プレビューシーンでは登録しない (一時的なオブジェクトとして生成されるため)
		if (!EditorSceneManager.IsPreviewScene(gameObject.scene)) {
			foreach (var item in GUIDObjects.ToList()) { // 内部でGUIDObjectsの要素数が変化するため、現時点のキャッシュに対して処理する
				// ここで、GUIDがすでに登録済みであれば、自身が複製されたものとして新しいGUIDを割り当てる
				if (!GUIDCollector.Instance.registerGrobalReference(item.GUID, item.Target)) {
					GUIDObjects.RemoveAt(GUIDObjects.FindIndex(guidObject => guidObject.Target == item.Target));
					createGUID(item.Target);
				}
			}
		}
	}

	void OnValidate()
	{
		if (!registerGUID) {
			return;
		}

		// Undo などで値が変更された場合の処理
		if (!EditorSceneManager.IsPreviewScene(gameObject.scene)) {
			foreach (var item in GUIDObjects.ToList()) {
				// 二重で登録されてしまうことは無いため、全て登録しなおす
				GUIDCollector.Instance.registerGrobalReference(item.GUID, item.Target);
			}
		}
	}

	void OnDestroy()
	{
		if (!EditorSceneManager.IsPreviewScene(gameObject.scene)) {
			foreach (var item in GUIDObjects) {
				GUIDCollector.Instance.unregisterGrobalReference(item.GUID);
			}
		}
	}

	public void OnAfterDeserialize()
	{
	}

	public void OnBeforeSerialize()
	{
		foreach (var guidObject in GUIDObjects) {
			if (!guidObject.Target) {
				GUIDCollector.Instance.unregisterGrobalReference(guidObject.GUID);
			}
		}
		// 参照できなくなったターゲットの要素をシュリンクする
		GUIDObjects = GUIDObjects.Where(guidObject => guidObject.Target != null).ToList();

#if UNITY_EDITOR
		ShowGUIDObjects = GUIDObjects.Select(guidObject => {
			return new ShowGUIDObject(guidObject.Target.name + " (" + guidObject.Target.GetType().Name + ")", guidObject.GUID.ToString());
		}).ToArray();
#endif
	}

	/// <summary>
	/// 指定したオブジェクトに関連づけられた GUID を返す
	/// </summary>
	/// <param name="target"></param>
	/// <returns></returns>
	public System.Guid getGUID(Object target)
	{
		return GUIDObjects.FirstOrDefault(guidObject => guidObject.Target == target).GUID;
	}

	/// <summary>
	/// GUID を新たに作成して登録する
	/// </summary>
	/// <param name="target"></param>
	public void createGUID(Object target)
	{
		var guid = System.Guid.NewGuid();
		Debug.Log("Create New GUID [" + guid.ToString() + "]", gameObject);
		GUIDCollector.Instance.registerGrobalReference(guid, target);

		GUIDObjects.Add(new GUIDObject(target, guid));
		registerGUID = true;
	}
}
