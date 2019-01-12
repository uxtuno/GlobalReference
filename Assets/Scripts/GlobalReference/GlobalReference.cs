using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シーン ↔ アセット 間の参照をGUIDベースで解決するクラス
/// GameObject と、MonoBehaviour を継承したクラスのオブジェクトのみバインドすることが可能
/// 基本的にはGameObjectの参照をインスペクタ上に公開して設定させる場合と同様の方法で使用できる
/// 
/// インスペクタ上でオブジェクトの参照をバインドした際、そのオブジェクトに対してGUIDが割り振られる (そのオブジェクトが所属するGameObujectのコンポーネントとして登録される)
/// バインドされたオブジェクトを取得する場合は、resolve() を呼び出して参照を解決する
/// この時、バインドしたオブジェクトがSceneに存在していれば参照が解決されて、そのオブジェクトを取得することが出来る
/// </summary>
[System.Serializable]
public class GlobalReference : ISerializationCallbackReceiver
{
	/// <summary>
	/// GUID のシリアライズ時の形式
	/// Editor スクリプト上で設定される
	/// </summary>
	[SerializeField]
	byte[] SerializedGUID;

	/// <summary>
	/// GUID の実行時に扱う形式
	/// </summary>
	System.Guid guid;

	/// <summary>
	/// デシリアライズされた際に状態を更新
	/// </summary>
	public void OnAfterDeserialize()
	{
		if (SerializedGUID != null && SerializedGUID.Length == 16) {
			guid = new System.Guid(SerializedGUID);
		}
	}

	/// <summary>
	/// GUIDをシリアライズ
	/// </summary>
	public void OnBeforeSerialize()
	{
		SerializedGUID = guid.ToByteArray();
	}

	/// <summary>
	/// GUID により、参照を解決する
	/// 解決できなかった場合は null を返す
	/// </summary>
	/// <returns>解決されたオブジェクト</returns>
	public Object resolve()
	{
		return GUIDCollector.Instance.resolve(guid);
	}

	/// <summary>
	/// GUID により、参照を解決する
	/// 解決できなかった場合は null を返す
	/// </summary>
	/// <typeparam name="T">オブジェクトを受け取る型</typeparam>
	/// <returns>解決されたオブジェクト</returns>
	public T resolve<T>() where T : Object
	{
		return GUIDCollector.Instance.resolve(guid) as T;
	}
}
