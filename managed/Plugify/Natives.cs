using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace Plugify;

public static class NativeMethods
{
	private const string DllName = "dotnet-lang-module";

	#region Core functions

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr GetMethodPtr(string methodName);
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern bool IsModuleLoaded(string moduleName, int version, bool minimum);
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern bool IsPluginLoaded(string pluginName, int version, bool minimum);
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern long GetPluginId(IntPtr pluginHandle);
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.LPStr)]
	public static extern string GetPluginName(IntPtr pluginHandle);
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.LPStr)]
	public static extern string GetPluginFullName(IntPtr pluginHandle);
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.LPStr)]
	public static extern string GetPluginDescription(IntPtr pluginHandle);
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.LPStr)]
	public static extern string GetPluginVersion(IntPtr pluginHandle);
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.LPStr)]
	public static extern string GetPluginAuthor(IntPtr pluginHandle);
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.LPStr)]
	public static extern string GetPluginWebsite(IntPtr pluginHandle);
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.LPStr)]
	public static extern string GetPluginBaseDir(IntPtr pluginHandle);
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void GetPluginDependencies(IntPtr pluginHandle, [In, Out] string[] deps);
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetPluginDependenciesSize(IntPtr pluginHandle);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.LPStr)]
	public static extern string FindPluginResource(IntPtr pluginHandle, string path);

	#endregion
	
	#region String functions
	
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr AllocateString();

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr CreateString(string source);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.LPStr)]
	public static extern string GetStringData(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetStringLength(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void AssignString(IntPtr ptr, string source);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void FreeString(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void DeleteString(IntPtr ptr);

	#endregion

	#region CreateVector functions

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr CreateVectorBool([In] bool[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr CreateVectorChar8([In] char[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr CreateVectorChar16([In] ushort[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr CreateVectorInt8([In] sbyte[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr CreateVectorInt16([In] short[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr CreateVectorInt32([In] int[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr CreateVectorInt64([In] long[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr CreateVectorUInt8([In] byte[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr CreateVectorUInt16([In] ushort[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr CreateVectorUInt32([In] uint[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr CreateVectorUInt64([In] ulong[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr CreateVectorUIntPtr([In] UIntPtr[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr CreateVectorFloat([In] float[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr CreateVectorDouble([In] double[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr CreateVectorString([In] string[] arr, int len);
	
	#endregion

	#region AllocateVector functions

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr AllocateVectorBool();

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr AllocateVectorChar8();

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr AllocateVectorChar16();

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr AllocateVectorInt8();

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr AllocateVectorInt16();

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr AllocateVectorInt32();

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr AllocateVectorInt64();

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr AllocateVectorUInt8();

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr AllocateVectorUInt16();

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr AllocateVectorUInt32();

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr AllocateVectorUInt64();

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr AllocateVectorUIntPtr();

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr AllocateVectorFloat();

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr AllocateVectorDouble();

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr AllocateVectorString();
	
	#endregion
	
	#region GetVectorSize functions

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetVectorSizeBool(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetVectorSizeChar8(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetVectorSizeChar16(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetVectorSizeInt8(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetVectorSizeInt16(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetVectorSizeInt32(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetVectorSizeInt64(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetVectorSizeUInt8(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetVectorSizeUInt16(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetVectorSizeUInt32(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetVectorSizeUInt64(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetVectorSizeUIntPtr(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetVectorSizeFloat(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetVectorSizeDouble(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetVectorSizeString(IntPtr ptr);
	
	#endregion

	#region GetVectorData functions

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void GetVectorDataBool(IntPtr ptr, [In, Out] bool[] arr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void GetVectorDataChar8(IntPtr ptr, [In, Out] char[] arr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void GetVectorDataChar16(IntPtr ptr, [In, Out] char[] arr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void GetVectorDataInt8(IntPtr ptr, [In, Out] sbyte[] arr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void GetVectorDataInt16(IntPtr ptr, [In, Out] short[] arr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void GetVectorDataInt32(IntPtr ptr, [In, Out] int[] arr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void GetVectorDataInt64(IntPtr ptr, [In, Out] long[] arr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void GetVectorDataUInt8(IntPtr ptr, [In, Out] byte[] arr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void GetVectorDataUInt16(IntPtr ptr, [In, Out] ushort[] arr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void GetVectorDataUInt32(IntPtr ptr, [In, Out] uint[] arr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void GetVectorDataUInt64(IntPtr ptr, [In, Out] ulong[] arr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void GetVectorDataUIntPtr(IntPtr ptr, [In, Out] IntPtr[] arr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void GetVectorDataFloat(IntPtr ptr, [In, Out] float[] arr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void GetVectorDataDouble(IntPtr ptr, [In, Out] double[] arr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void GetVectorDataString(IntPtr ptr, [In, Out] string[] arr);

	#endregion

	#region AssignVector Functions
	
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void AssignVectorBool(IntPtr ptr, [In] bool[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void AssignVectorChar8(IntPtr ptr, [In] char[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void AssignVectorChar16(IntPtr ptr, [In] ushort[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void AssignVectorInt8(IntPtr ptr, [In] sbyte[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void AssignVectorInt16(IntPtr ptr, [In] short[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void AssignVectorInt32(IntPtr ptr, [In] int[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void AssignVectorInt64(IntPtr ptr, [In] long[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void AssignVectorUInt8(IntPtr ptr, [In] byte[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void AssignVectorUInt16(IntPtr ptr, [In] ushort[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void AssignVectorUInt32(IntPtr ptr, [In] uint[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void AssignVectorUInt64(IntPtr ptr, [In] ulong[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void AssignVectorUIntPtr(IntPtr ptr, [In] UIntPtr[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void AssignVectorFloat(IntPtr ptr, [In] float[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void AssignVectorDouble(IntPtr ptr, [In] double[] arr, int len);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void AssignVectorString(IntPtr ptr, [MarshalAs(UnmanagedType.LPStr)] [In] string[] arr, int len);
	
	#endregion
	
	#region DeleteVector functions

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void DeleteVectorBool(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void DeleteVectorChar8(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void DeleteVectorChar16(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void DeleteVectorInt8(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void DeleteVectorInt16(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void DeleteVectorInt32(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void DeleteVectorInt64(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void DeleteVectorUInt8(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void DeleteVectorUInt16(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void DeleteVectorUInt32(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void DeleteVectorUInt64(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void DeleteVectorUIntPtr(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void DeleteVectorFloat(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void DeleteVectorDouble(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void DeleteVectorString(IntPtr ptr);

	#endregion

	#region FreeVectorData functions

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void FreeVectorBool(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void FreeVectorChar8(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void FreeVectorChar16(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void FreeVectorInt8(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void FreeVectorInt16(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void FreeVectorInt32(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void FreeVectorInt64(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void FreeVectorUInt8(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void FreeVectorUInt16(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void FreeVectorUInt32(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void FreeVectorUInt64(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void FreeVectorUIntPtr(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void FreeVectorFloat(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void FreeVectorDouble(IntPtr ptr);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void FreeVectorString(IntPtr ptr);

	#endregion
}