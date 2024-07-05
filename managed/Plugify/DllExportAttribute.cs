namespace Plugify;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.ReturnValue)]
public sealed class MarshalAttribute(ValueType valueType) : Attribute
{
    public ValueType Value = valueType;
}