using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGlobalReference : MonoBehaviour
{
	[SerializeField, GlobalReferenceTypeBinding(typeof(MonoBehaviour))]
	GlobalReference reference1;

	[SerializeField, GlobalReferenceTypeBinding(typeof(Camera))]
	GlobalReference reference2;

	[SerializeField, GlobalReferenceTypeBinding(typeof(FixedJoint2D))]
	GlobalReference reference3;

	[SerializeField]
	GlobalReference reference4;
}
