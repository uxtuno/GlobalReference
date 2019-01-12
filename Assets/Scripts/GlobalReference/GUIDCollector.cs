using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// GUID を集約するクラス
/// </summary>
public class GUIDCollector
{
	static GUIDCollector _instance;

	/// <summary>
	/// 唯一のインスタンス
	/// </summary>
	public static GUIDCollector Instance
	{
		get
		{
			if (_instance == null) {
				_instance = new GUIDCollector();
			}
			return _instance;
		}
	}

	/// <summary>
	/// 現在ロードされている Scene に存在する GUID と、オブジェクトを対応づける連想配列
	/// </summary>
	Dictionary<System.Guid, Object> globalReferenceMap = new Dictionary<System.Guid, Object>();

	/// <summary>
	/// 全ての参照を返す
	/// </summary>
	public IReadOnlyDictionary<System.Guid, Object> allReference { get { return globalReferenceMap; } }

	/// <summary>
	/// GUID が登録された際のコールバック
	/// </summary>
	public event System.Action<System.Guid> onRegisterGUID;

	/// <summary>
	/// GUID が登録解除された際のコールバック
	/// </summary>
	public event System.Action<System.Guid> onUnregisterGUID;

	/// <summary>
	/// GUID を登録する
	/// </summary>
	/// <param name="guid">オブジェクトを一意に識別するGUID</param>
	/// <param name="target">GUIDと関連付けるオブジェクト</param>
	/// <returns>true = 登録成功</returns>
	public bool registerGrobalReference(System.Guid guid, Object target)
	{
		if (globalReferenceMap.ContainsKey(guid)) {
			return false;
		}

		globalReferenceMap.Add(guid, target);
		onRegisterGUID?.Invoke(guid);
		return true;
	}

	/// <summary>
	/// GUID の登録を解除する
	/// </summary>
	/// <param name="guid">解除する GUID</param>
	public void unregisterGrobalReference(System.Guid guid)
	{
		if (globalReferenceMap.ContainsKey(guid)) {
			globalReferenceMap.Remove(guid);
		}

		onUnregisterGUID?.Invoke(guid);
	}

	/// <summary>
	/// GUID から参照を解決する
	/// </summary>
	/// <param name="guid">GUID</param>
	/// <returns>解決されたオブジェクト</returns>
	public Object resolve(byte[] guid)
	{
		var resolveGUID = new System.Guid(guid);
		if (!globalReferenceMap.ContainsKey(resolveGUID)) {
			return null;
		}
		return globalReferenceMap[resolveGUID];
	}

	/// <summary>
	/// GUID から参照を解決する
	/// </summary>
	/// <param name="guid">GUID</param>
	/// <returns>解決されたオブジェクト</returns>
	public Object resolve(System.Guid guid)
	{
		return globalReferenceMap[guid];
	}
}
