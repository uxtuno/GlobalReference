using UnityEngine;

public class SampleScript : MonoBehaviour
{
	/// <summary>
	/// GlobalReferenceTypeBinding で指定した型によってバインド可能な型を制限できる
	/// 複数の同じ型のコンポーネントがアタッチされていると、GetComponent<T> が返すほうのコンポーネントがバインドされ、
	/// 見ただけではどのコンポーネントがバインドされているかがわからないため、避けること
	/// </summary>
	[SerializeField, GlobalReferenceTypeBinding(typeof(MonoBehaviour))]
	GlobalReference reference1;

	[SerializeField, GlobalReferenceTypeBinding(typeof(Camera))]
	GlobalReference reference2;

	[SerializeField, GlobalReferenceTypeBinding(typeof(Rigidbody))]
	GlobalReference reference3;

	[SerializeField]
	GlobalReference reference4;

	void Start()
	{
		Debug.Log(reference1.resolve<MonoBehaviour>().GetType());
		Debug.Log(reference2.resolve<Camera>().GetType());
		Debug.Log(reference3.resolve<Rigidbody>().GetType());
		Debug.Log(reference4.resolve<GameObject>().GetType());
	}
}
