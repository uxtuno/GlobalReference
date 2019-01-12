using System;

/// <summary>
/// GlobalReference に対してバインドできる型を制限するための属性
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class GlobalReferenceTypeBindingAttribute : Attribute
{
	public Type BindType { get; }
	public GlobalReferenceTypeBindingAttribute(Type bindType)
	{
		BindType = bindType;
	}
}
