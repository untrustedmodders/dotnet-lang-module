using System.Runtime.InteropServices;
using System.Reflection;

namespace Plugify.Interop;

using static ManagedHost;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal readonly struct InternalCall
{
	private readonly nint namePtr;
	public readonly nint NativeFunctionPtr;

	public string? Name => Marshal.PtrToStringAuto(namePtr);
}

internal static class InternalCallsManager
{
	[UnmanagedCallersOnly]
	private static unsafe void SetInternalCalls(InternalCall* internalCallsArrayPtr, int length, Bool32 warnOnMissing)
	{
		try
		{
			for (int i = 0; i < length; i++)
			{
				var internalCall = internalCallsArrayPtr[i];
				var name = internalCall.Name;

				if (name == null)
				{
					LogMessage($"Cannot register internal at index '{i}' call with null name!", MessageLevel.Error);
					continue;
				}

				var fieldNameStart = name.IndexOf('@');
				var fieldNameEnd = name.IndexOf(",", fieldNameStart, StringComparison.CurrentCulture);
				var fieldName = name.Substring(fieldNameStart + 1, fieldNameEnd - fieldNameStart - 1);
				var containingTypeName = name.Remove(fieldNameStart, fieldNameEnd - fieldNameStart);

				var type = TypeInterface.FindType(containingTypeName);
				
				if (type == null)
				{
					if (warnOnMissing)
					{
						LogMessage($"Cannot register internal call '{name}', failed to type '{containingTypeName}'.", MessageLevel.Error);
					}
					continue;
				}

				var bindingFlags = BindingFlags.Static | BindingFlags.NonPublic;
				var field = type.GetFields(bindingFlags).FirstOrDefault(field => field.Name == fieldName);

				if (field == null)
				{
					LogMessage($"Cannot register internal '{name}', failed to find it in type '{containingTypeName}'", MessageLevel.Error);
					continue;
				}

				if (!field.FieldType.IsFunctionPointer)
				{
					LogMessage($"Field '{name}' is not a function pointer type!", MessageLevel.Error);
					continue;
				}

				field.SetValue(null, internalCall.NativeFunctionPtr);
			}
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}
}
