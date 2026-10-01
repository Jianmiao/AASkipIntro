// The host's IL2CPP metadata includes an empty NullableAttribute. Supply the
// compiler-only attribute constructors locally without modifying host files.
#pragma warning disable CS0436
namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
internal sealed class NullableAttribute : Attribute
{
    public readonly byte[] NullableFlags;
    public NullableAttribute(byte value) => NullableFlags = new[] { value };
    public NullableAttribute(byte[] values) => NullableFlags = values;
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
internal sealed class NullableContextAttribute : Attribute
{
    public readonly byte Flag;
    public NullableContextAttribute(byte value) => Flag = value;
}
