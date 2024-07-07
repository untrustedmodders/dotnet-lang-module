using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace Plugify;

public static class NativeMethods
{
	public const string DllName = "dotnet-lang-module";

	#region Core functions

	[DllImport(DllName)]
	[return: MarshalAs(UnmanagedType.LPStr)]
	public static extern string GetBaseDir();
	[DllImport(DllName)]
	public static extern bool IsModuleLoaded([MarshalAs(UnmanagedType.LPStr)] string moduleName, int version, bool minimum);
	[DllImport(DllName)]
	public static extern bool IsPluginLoaded([MarshalAs(UnmanagedType.LPStr)] string pluginName, int version, bool minimum);
	[DllImport(DllName)]
	[return: MarshalAs(UnmanagedType.LPStr)]
	public static extern string? FindPluginResource(long pluginId, [MarshalAs(UnmanagedType.LPStr)] string path);

	#endregion
	
	#region String functions
	
	[DllImport(DllName)]
	public static extern nint AllocateString();

	[DllImport(DllName)]
	public static extern nint CreateString([MarshalAs(UnmanagedType.LPStr)] string? source);

	[DllImport(DllName)]
	[return: MarshalAs(UnmanagedType.LPStr)]
	public static extern string GetStringData(nint ptr);

	[DllImport(DllName)]
	public static extern int GetStringLength(nint ptr);

	[DllImport(DllName)]
	public static extern void ConstructString(nint ptr, [MarshalAs(UnmanagedType.LPStr)] string? source);

	[DllImport(DllName)]
	public static extern void AssignString(nint ptr, [MarshalAs(UnmanagedType.LPStr)] string? source);

	[DllImport(DllName)]
	public static extern void FreeString(nint ptr);

	[DllImport(DllName)]
	public static extern void DeleteString(nint ptr);

	#endregion

	#region CreateVector functions

	[DllImport(DllName)]
	public static extern nint CreateVectorBool([In] bool[] arr, int len);

	[DllImport(DllName, CharSet = CharSet.Ansi)]
	public static extern nint CreateVectorChar8([In] char[] arr, int len);

	[DllImport(DllName, CharSet = CharSet.Unicode)]
	public static extern nint CreateVectorChar16([In] char[] arr, int len);

	[DllImport(DllName)]
	public static extern nint CreateVectorInt8([In] sbyte[] arr, int len);

	[DllImport(DllName)]
	public static extern nint CreateVectorInt16([In] short[] arr, int len);

	[DllImport(DllName)]
	public static extern nint CreateVectorInt32([In] int[] arr, int len);

	[DllImport(DllName)]
	public static extern nint CreateVectorInt64([In] long[] arr, int len);

	[DllImport(DllName)]
	public static extern nint CreateVectorUInt8([In] byte[] arr, int len);

	[DllImport(DllName)]
	public static extern nint CreateVectorUInt16([In] ushort[] arr, int len);

	[DllImport(DllName)]
	public static extern nint CreateVectorUInt32([In] uint[] arr, int len);

	[DllImport(DllName)]
	public static extern nint CreateVectorUInt64([In] ulong[] arr, int len);

	[DllImport(DllName)]
	public static extern nint CreateVectorIntPtr([In] nint[] arr, int len);

	[DllImport(DllName)]
	public static extern nint CreateVectorFloat([In] float[] arr, int len);

	[DllImport(DllName)]
	public static extern nint CreateVectorDouble([In] double[] arr, int len);

	[DllImport(DllName)]
	public static extern nint CreateVectorString([MarshalAs(UnmanagedType.LPArray)] [In] string[] arr, int len);
	
	#endregion

	#region AllocateVector functions

	[DllImport(DllName)]
	public static extern nint AllocateVectorBool();

	[DllImport(DllName)]
	public static extern nint AllocateVectorChar8();

	[DllImport(DllName)]
	public static extern nint AllocateVectorChar16();

	[DllImport(DllName)]
	public static extern nint AllocateVectorInt8();

	[DllImport(DllName)]
	public static extern nint AllocateVectorInt16();

	[DllImport(DllName)]
	public static extern nint AllocateVectorInt32();

	[DllImport(DllName)]
	public static extern nint AllocateVectorInt64();

	[DllImport(DllName)]
	public static extern nint AllocateVectorUInt8();

	[DllImport(DllName)]
	public static extern nint AllocateVectorUInt16();

	[DllImport(DllName)]
	public static extern nint AllocateVectorUInt32();

	[DllImport(DllName)]
	public static extern nint AllocateVectorUInt64();

	[DllImport(DllName)]
	public static extern nint AllocateVectorIntPtr();

	[DllImport(DllName)]
	public static extern nint AllocateVectorFloat();

	[DllImport(DllName)]
	public static extern nint AllocateVectorDouble();

	[DllImport(DllName)]
	public static extern nint AllocateVectorString();
	
	#endregion
	
	#region GetVectorSize functions

	[DllImport(DllName)]
	public static extern int GetVectorSizeBool(nint ptr);

	[DllImport(DllName)]
	public static extern int GetVectorSizeChar8(nint ptr);

	[DllImport(DllName)]
	public static extern int GetVectorSizeChar16(nint ptr);

	[DllImport(DllName)]
	public static extern int GetVectorSizeInt8(nint ptr);

	[DllImport(DllName)]
	public static extern int GetVectorSizeInt16(nint ptr);

	[DllImport(DllName)]
	public static extern int GetVectorSizeInt32(nint ptr);

	[DllImport(DllName)]
	public static extern int GetVectorSizeInt64(nint ptr);

	[DllImport(DllName)]
	public static extern int GetVectorSizeUInt8(nint ptr);

	[DllImport(DllName)]
	public static extern int GetVectorSizeUInt16(nint ptr);

	[DllImport(DllName)]
	public static extern int GetVectorSizeUInt32(nint ptr);

	[DllImport(DllName)]
	public static extern int GetVectorSizeUInt64(nint ptr);

	[DllImport(DllName)]
	public static extern int GetVectorSizeIntPtr(nint ptr);

	[DllImport(DllName)]
	public static extern int GetVectorSizeFloat(nint ptr);

	[DllImport(DllName)]
	public static extern int GetVectorSizeDouble(nint ptr);

	[DllImport(DllName)]
	public static extern int GetVectorSizeString(nint ptr);
	
	#endregion

	#region GetVectorData functions

	[DllImport(DllName)]
	public static extern void GetVectorDataBool(nint ptr, [In, Out] bool[] arr);

	[DllImport(DllName, CharSet = CharSet.Ansi)]
	public static extern void GetVectorDataChar8(nint ptr, [In, Out] char[] arr);

	[DllImport(DllName, CharSet = CharSet.Unicode)]
	public static extern void GetVectorDataChar16(nint ptr, [In, Out] char[] arr);

	[DllImport(DllName)]
	public static extern void GetVectorDataInt8(nint ptr, [In, Out] sbyte[] arr);

	[DllImport(DllName)]
	public static extern void GetVectorDataInt16(nint ptr, [In, Out] short[] arr);

	[DllImport(DllName)]
	public static extern void GetVectorDataInt32(nint ptr, [In, Out] int[] arr);

	[DllImport(DllName)]
	public static extern void GetVectorDataInt64(nint ptr, [In, Out] long[] arr);

	[DllImport(DllName)]
	public static extern void GetVectorDataUInt8(nint ptr, [In, Out] byte[] arr);

	[DllImport(DllName)]
	public static extern void GetVectorDataUInt16(nint ptr, [In, Out] ushort[] arr);

	[DllImport(DllName)]
	public static extern void GetVectorDataUInt32(nint ptr, [In, Out] uint[] arr);

	[DllImport(DllName)]
	public static extern void GetVectorDataUInt64(nint ptr, [In, Out] ulong[] arr);

	[DllImport(DllName)]
	public static extern void GetVectorDataIntPtr(nint ptr, [In, Out] nint[] arr);

	[DllImport(DllName)]
	public static extern void GetVectorDataFloat(nint ptr, [In, Out] float[] arr);

	[DllImport(DllName)]
	public static extern void GetVectorDataDouble(nint ptr, [In, Out] double[] arr);

	[DllImport(DllName)]
	public static extern void GetVectorDataString(nint ptr, [MarshalAs(UnmanagedType.LPArray)] [In, Out] string[] arr);

	#endregion

	#region ConstructVector Functions
	
	[DllImport(DllName)]
	public static extern void ConstructVectorBool(nint ptr, [In] bool[] arr, int len);

	[DllImport(DllName, CharSet = CharSet.Ansi)]
	public static extern void ConstructVectorChar8(nint ptr, [In] char[] arr, int len);

	[DllImport(DllName, CharSet = CharSet.Unicode)]
	public static extern void ConstructVectorChar16(nint ptr, [In] char[] arr, int len);

	[DllImport(DllName)]
	public static extern void ConstructVectorInt8(nint ptr, [In] sbyte[] arr, int len);

	[DllImport(DllName)]
	public static extern void ConstructVectorInt16(nint ptr, [In] short[] arr, int len);

	[DllImport(DllName)]
	public static extern void ConstructVectorInt32(nint ptr, [In] int[] arr, int len);

	[DllImport(DllName)]
	public static extern void ConstructVectorInt64(nint ptr, [In] long[] arr, int len);

	[DllImport(DllName)]
	public static extern void ConstructVectorUInt8(nint ptr, [In] byte[] arr, int len);

	[DllImport(DllName)]
	public static extern void ConstructVectorUInt16(nint ptr, [In] ushort[] arr, int len);

	[DllImport(DllName)]
	public static extern void ConstructVectorUInt32(nint ptr, [In] uint[] arr, int len);

	[DllImport(DllName)]
	public static extern void ConstructVectorUInt64(nint ptr, [In] ulong[] arr, int len);

	[DllImport(DllName)]
	public static extern void ConstructVectorIntPtr(nint ptr, [In] nint[] arr, int len);

	[DllImport(DllName)]
	public static extern void ConstructVectorFloat(nint ptr, [In] float[] arr, int len);

	[DllImport(DllName)]
	public static extern void ConstructVectorDouble(nint ptr, [In] double[] arr, int len);

	[DllImport(DllName)]
	public static extern void ConstructVectorString(nint ptr, [MarshalAs(UnmanagedType.LPArray)] [In] string[] arr, int len);

	#endregion

	#region AssignVector Functions
	
	[DllImport(DllName)]
	public static extern void AssignVectorBool(nint ptr, [In] bool[] arr, int len);

	[DllImport(DllName, CharSet = CharSet.Ansi)]
	public static extern void AssignVectorChar8(nint ptr, [In] char[] arr, int len);

	[DllImport(DllName, CharSet = CharSet.Unicode)]
	public static extern void AssignVectorChar16(nint ptr, [In] char[] arr, int len);

	[DllImport(DllName)]
	public static extern void AssignVectorInt8(nint ptr, [In] sbyte[] arr, int len);

	[DllImport(DllName)]
	public static extern void AssignVectorInt16(nint ptr, [In] short[] arr, int len);

	[DllImport(DllName)]
	public static extern void AssignVectorInt32(nint ptr, [In] int[] arr, int len);

	[DllImport(DllName)]
	public static extern void AssignVectorInt64(nint ptr, [In] long[] arr, int len);

	[DllImport(DllName)]
	public static extern void AssignVectorUInt8(nint ptr, [In] byte[] arr, int len);

	[DllImport(DllName)]
	public static extern void AssignVectorUInt16(nint ptr, [In] ushort[] arr, int len);

	[DllImport(DllName)]
	public static extern void AssignVectorUInt32(nint ptr, [In] uint[] arr, int len);

	[DllImport(DllName)]
	public static extern void AssignVectorUInt64(nint ptr, [In] ulong[] arr, int len);

	[DllImport(DllName)]
	public static extern void AssignVectorIntPtr(nint ptr, [In] nint[] arr, int len);

	[DllImport(DllName)]
	public static extern void AssignVectorFloat(nint ptr, [In] float[] arr, int len);

	[DllImport(DllName)]
	public static extern void AssignVectorDouble(nint ptr, [In] double[] arr, int len);

	[DllImport(DllName)]
	public static extern void AssignVectorString(nint ptr, [MarshalAs(UnmanagedType.LPArray)] [In] string[] arr, int len);
	
	#endregion
	
	#region DeleteVector functions

	[DllImport(DllName)]
	public static extern void DeleteVectorBool(nint ptr);

	[DllImport(DllName)]
	public static extern void DeleteVectorChar8(nint ptr);

	[DllImport(DllName)]
	public static extern void DeleteVectorChar16(nint ptr);

	[DllImport(DllName)]
	public static extern void DeleteVectorInt8(nint ptr);

	[DllImport(DllName)]
	public static extern void DeleteVectorInt16(nint ptr);

	[DllImport(DllName)]
	public static extern void DeleteVectorInt32(nint ptr);

	[DllImport(DllName)]
	public static extern void DeleteVectorInt64(nint ptr);

	[DllImport(DllName)]
	public static extern void DeleteVectorUInt8(nint ptr);

	[DllImport(DllName)]
	public static extern void DeleteVectorUInt16(nint ptr);

	[DllImport(DllName)]
	public static extern void DeleteVectorUInt32(nint ptr);

	[DllImport(DllName)]
	public static extern void DeleteVectorUInt64(nint ptr);

	[DllImport(DllName)]
	public static extern void DeleteVectorIntPtr(nint ptr);

	[DllImport(DllName)]
	public static extern void DeleteVectorFloat(nint ptr);

	[DllImport(DllName)]
	public static extern void DeleteVectorDouble(nint ptr);

	[DllImport(DllName)]
	public static extern void DeleteVectorString(nint ptr);

	#endregion

	#region FreeVectorData functions

	[DllImport(DllName)]
	public static extern void FreeVectorBool(nint ptr);

	[DllImport(DllName)]
	public static extern void FreeVectorChar8(nint ptr);

	[DllImport(DllName)]
	public static extern void FreeVectorChar16(nint ptr);

	[DllImport(DllName)]
	public static extern void FreeVectorInt8(nint ptr);

	[DllImport(DllName)]
	public static extern void FreeVectorInt16(nint ptr);

	[DllImport(DllName)]
	public static extern void FreeVectorInt32(nint ptr);

	[DllImport(DllName)]
	public static extern void FreeVectorInt64(nint ptr);

	[DllImport(DllName)]
	public static extern void FreeVectorUInt8(nint ptr);

	[DllImport(DllName)]
	public static extern void FreeVectorUInt16(nint ptr);

	[DllImport(DllName)]
	public static extern void FreeVectorUInt32(nint ptr);

	[DllImport(DllName)]
	public static extern void FreeVectorUInt64(nint ptr);

	[DllImport(DllName)]
	public static extern void FreeVectorIntPtr(nint ptr);

	[DllImport(DllName)]
	public static extern void FreeVectorFloat(nint ptr);

	[DllImport(DllName)]
	public static extern void FreeVectorDouble(nint ptr);

	[DllImport(DllName)]
	public static extern void FreeVectorString(nint ptr);

	#endregion
}
