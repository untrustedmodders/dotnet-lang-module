﻿using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Plugify;

internal static class Marshalling
{
	internal static unsafe object?[]? MarshalParameterArray(nint paramsPtr, int parameterCount, MethodBase methodInfo)
    {
        ParameterInfo[] parameterInfos = methodInfo.GetParameters();
        int numParams = parameterInfos.Length;

        if (numParams == 0 || paramsPtr == nint.Zero || numParams != parameterCount)
        {
            return null;
        }

        var parameters = new object?[numParams];

        int paramsOffset = 0;

        for (int i = 0; i < numParams; i++)
        {
            // params is stored as void**
            nint paramPtr = *(nint*)((byte*)paramsPtr + paramsOffset);
            paramsOffset += nint.Size;

            ParameterInfo parameterInfo = parameterInfos[i];
            Type paramType = parameterInfo.ParameterType;

            bool useAnsi = paramType.IsChar() && TypeUtils.IsUseAnsi(parameterInfo.GetCustomAttributes(false));
            
            if (paramType.IsEnum)
            {
                Type underlyingType = Enum.GetUnderlyingType(paramType);
                parameters[i] = Enum.ToObject(paramType, MarshalPointer(paramPtr, underlyingType, useAnsi));
            } 
            else
            {
                parameters[i] = MarshalPointer(paramPtr, paramType, useAnsi);
            }
        }

        return parameters;
    }

    internal static unsafe void MarshalParameterRefs(nint paramsPtr, int parameterCount, MethodInfo methodInfo, object?[]? parameters)
    {
        if (parameters == null)
        {
            return;
        }

        ParameterInfo[] parameterInfos = methodInfo.GetParameters();
        int numParams = parameterInfos.Length;

        if (numParams != parameterCount)
        {
	        return;
        }

        int paramsOffset = 0;

        for (int i = 0; i < numParams; i++)
        {
            // params is stored as void**
            nint paramPtr = *(nint*)((byte*)paramsPtr + paramsOffset);
            paramsOffset += nint.Size;

            ParameterInfo parameterInfo = parameterInfos[i];
            Type paramType = parameterInfo.ParameterType;
            
            if (!paramType.IsByRef)
            {
                continue;
            }

            object? paramValue = parameters[i];
            if (paramValue == null)
            {
	            throw new Exception("Reference cannot be null");
            }
            
            bool useAnsi = paramType.IsChar() && TypeUtils.IsUseAnsi(parameterInfo.GetCustomAttributes(false));

            MarshalReturnValue(paramValue, paramType, paramPtr, useAnsi);
        }
    }

    internal static unsafe void MarshalReturnValue(object paramValue, Type paramType, nint outValue, bool useAnsi = false)
    {
	    if (paramType.IsEnum)
	    {
		    // If returnType is an enum we need to get the underlying type and cast the value to it
		    paramType = Enum.GetUnderlyingType(paramType);
		    paramValue = Convert.ChangeType(paramValue, paramType);
	    }
	    
        switch (TypeUtils.ConvertToValueType(paramType))
        {
	        case ValueType.Bool:
		        *(byte*)outValue = (byte)((bool)paramValue ? 1 : 0);
		        return;
	        case ValueType.Char8:
	        case ValueType.Char16:
		        if (useAnsi)
		        {
			        *(byte*)outValue = (byte)(char)paramValue;
		        }
		        else
		        {
			        *(short*)outValue = (short)(char)paramValue;
		        }
		        return;
	        case ValueType.Int8:
		        *(sbyte*)outValue = (sbyte)paramValue;
		        return;
	        case ValueType.Int16:
		        *(short*)outValue = (short)paramValue;
		        return;
	        case ValueType.Int32:
		        *(int*)outValue = (int)paramValue;
		        return;
	        case ValueType.Int64:
		        *(long*)outValue = (long)paramValue;
		        return;
	        case ValueType.UInt8:
		        *(byte*)outValue = (byte)paramValue;
		        return;
	        case ValueType.UInt16:
		        *(ushort*)outValue = (ushort)paramValue;
		        return;
	        case ValueType.UInt32:
		        *(uint*)outValue = (uint)paramValue;
		        return;
	        case ValueType.UInt64:
		        *(ulong*)outValue = (ulong)paramValue;
		        return;
	        case ValueType.Pointer:
		        *(nint*)outValue = (nint)paramValue;
		        return;
	        case ValueType.Float:
		        *(float*)outValue = (float)paramValue;
		        return;
	        case ValueType.Double:
		        *(double*)outValue = (double)paramValue;
		        return;
	        case ValueType.String:
		        NativeMethods.AssignString(outValue, (string)paramValue);
		        return;
	        case ValueType.ArrayBool:
		        var arrBool = (bool[])paramValue;
		        NativeMethods.AssignVectorBool(outValue, arrBool, arrBool.Length);
		        return;
	        case ValueType.ArrayChar8:
	        case ValueType.ArrayChar16:
			    var arrChar = (char[])paramValue;
		        if (useAnsi)
		        {
			        NativeMethods.AssignVectorChar8(outValue, arrChar, arrChar.Length);
		        }
		        else
		        {
			        NativeMethods.AssignVectorChar16(outValue, arrChar, arrChar.Length);
		        }
		        return;
	        case ValueType.ArrayInt8:
		        var arrInt8 = (sbyte[])paramValue;
		        NativeMethods.AssignVectorInt8(outValue, arrInt8, arrInt8.Length);
		        return;
	        case ValueType.ArrayInt16:
		        var arrInt16 = (short[])paramValue;
		        NativeMethods.AssignVectorInt16(outValue, arrInt16, arrInt16.Length);
		        return;
	        case ValueType.ArrayInt32:
		        var arrInt32 = (int[])paramValue;
		        NativeMethods.AssignVectorInt32(outValue, arrInt32, arrInt32.Length);
		        return;
	        case ValueType.ArrayInt64:
		        var arrInt64 = (long[])paramValue;
		        NativeMethods.AssignVectorInt64(outValue, arrInt64, arrInt64.Length);
		        return;
	        case ValueType.ArrayUInt8:
		        var arrUInt8 = (byte[])paramValue;
		        NativeMethods.AssignVectorUInt8(outValue, arrUInt8, arrUInt8.Length);
		        return;
	        case ValueType.ArrayUInt16:
		        var arrUInt16 = (ushort[])paramValue;
		        NativeMethods.AssignVectorUInt16(outValue, arrUInt16, arrUInt16.Length);
		        return;
	        case ValueType.ArrayUInt32:
		        var arrUInt32 = (uint[])paramValue;
		        NativeMethods.AssignVectorUInt32(outValue, arrUInt32, arrUInt32.Length);
		        return;
	        case ValueType.ArrayUInt64:
		        var arrUInt64 = (ulong[])paramValue;
		        NativeMethods.AssignVectorUInt64(outValue, arrUInt64, arrUInt64.Length);
		        return;
	        case ValueType.ArrayPointer:
		        var arrIntPtr = (nint[])paramValue;
		        NativeMethods.AssignVectorIntPtr(outValue, arrIntPtr, arrIntPtr.Length);
		        return;
	        case ValueType.ArrayFloat:
		        var arrFloat = (float[])paramValue;
		        NativeMethods.AssignVectorFloat(outValue, arrFloat, arrFloat.Length);
		        return;
	        case ValueType.ArrayDouble:
		        var arrDouble = (double[])paramValue;
		        NativeMethods.AssignVectorDouble(outValue, arrDouble, arrDouble.Length);
		        return;
	        case ValueType.ArrayString:
		        var arrString = (string[])paramValue;
		        NativeMethods.AssignVectorString(outValue, arrString, arrString.Length);
		        return;
	        case ValueType.Vector2:
	        case ValueType.Vector3:
	        case ValueType.Vector4:
	        case ValueType.Matrix4x4:
		        Marshal.StructureToPtr(paramValue, outValue, false);
		        return;
        }

        if (paramType.IsValueType)
        {
            Marshal.StructureToPtr(paramValue, outValue, false);
            return;
        }

        throw new NotImplementedException($"Parameter type {paramType.Name} not implemented");
    }

    internal static unsafe object MarshalPointer(nint inValue, Type paramType, bool useAnsi = false)
    {
	    ValueType valueType = TypeUtils.ConvertToValueType(paramType);
	    switch (valueType)
	    {
		    case ValueType.Bool:
			    return *(byte*)inValue == 1;
		    case ValueType.Char8:
		    case ValueType.Char16:
			    if (useAnsi)
			    {
				    return (char)*(byte*)inValue;
			    }
			    else
			    {
				    return (char)*(short*)inValue;
			    }
		    case ValueType.Int8:
			    return *(sbyte*)inValue;
		    case ValueType.Int16:
			    return *(short*)inValue;
		    case ValueType.Int32:
			    return *(int*)inValue;
		    case ValueType.Int64:
			    return *(long*)inValue;
		    case ValueType.UInt8:
			    return *(byte*)inValue;
		    case ValueType.UInt16:
			    return *(ushort*)inValue;
		    case ValueType.UInt32:
			    return *(uint*)inValue;
		    case ValueType.UInt64:
			    return *(ulong*)inValue;
		    case ValueType.Pointer:
			    return *(nint*)inValue;
		    case ValueType.Float:
			    return *(float*)inValue;
		    case ValueType.Double:
			    return *(double*)inValue;
		    case ValueType.Function:
			    return GetDelegateForFunctionPointer(inValue, paramType);
		    case ValueType.String:
			    return NativeMethods.GetStringData(inValue);
		    case ValueType.ArrayBool:
			    var arrBool = new bool[NativeMethods.GetVectorSizeBool(inValue)];
			    NativeMethods.GetVectorDataBool(inValue, arrBool);
			    return arrBool;
		    case ValueType.ArrayChar8:
		    case ValueType.ArrayChar16:
			    if (useAnsi)
			    {
				    var arrChar = new char[NativeMethods.GetVectorSizeChar8(inValue)];
				    NativeMethods.GetVectorDataChar8(inValue, arrChar);
				    return arrChar;
			    }
			    else
			    {
				    var arrChar = new char[NativeMethods.GetVectorSizeChar16(inValue)];
				    NativeMethods.GetVectorDataChar16(inValue, arrChar);
				    return arrChar;
			    }
		    case ValueType.ArrayInt8:
			    var arrInt8 = new sbyte[NativeMethods.GetVectorSizeInt8(inValue)];
			    NativeMethods.GetVectorDataInt8(inValue, arrInt8);
			    return arrInt8;
		    case ValueType.ArrayInt16:
			    var arrInt16 = new short[NativeMethods.GetVectorSizeInt16(inValue)];
			    NativeMethods.GetVectorDataInt16(inValue, arrInt16);
			    return arrInt16;
		    case ValueType.ArrayInt32:
			    var arrInt32 = new int[NativeMethods.GetVectorSizeInt32(inValue)];
			    NativeMethods.GetVectorDataInt32(inValue, arrInt32);
			    return arrInt32;
		    case ValueType.ArrayInt64:
			    var arrInt64 = new long[NativeMethods.GetVectorSizeInt64(inValue)];
			    NativeMethods.GetVectorDataInt64(inValue, arrInt64);
			    return arrInt64;
		    case ValueType.ArrayUInt8:
			    var arrUInt8 = new byte[NativeMethods.GetVectorSizeUInt8(inValue)];
			    NativeMethods.GetVectorDataUInt8(inValue, arrUInt8);
			    return arrUInt8;
		    case ValueType.ArrayUInt16:
			    var arrUInt16 = new ushort[NativeMethods.GetVectorSizeUInt16(inValue)];
			    NativeMethods.GetVectorDataUInt16(inValue, arrUInt16);
			    return arrUInt16;
		    case ValueType.ArrayUInt32:
			    var arrUInt32 = new uint[NativeMethods.GetVectorSizeUInt32(inValue)];
			    NativeMethods.GetVectorDataUInt32(inValue, arrUInt32);
			    return arrUInt32;
		    case ValueType.ArrayUInt64:
			    var arrUInt64 = new ulong[NativeMethods.GetVectorSizeUInt64(inValue)];
			    NativeMethods.GetVectorDataUInt64(inValue, arrUInt64);
			    return arrUInt64;
		    case ValueType.ArrayPointer:
			    var arrIntPtr = new nint[NativeMethods.GetVectorSizeIntPtr(inValue)];
			    NativeMethods.GetVectorDataIntPtr(inValue, arrIntPtr);
			    return arrIntPtr;
		    case ValueType.ArrayFloat:
			    var arrFloat = new float[NativeMethods.GetVectorSizeFloat(inValue)];
			    NativeMethods.GetVectorDataFloat(inValue, arrFloat);
			    return arrFloat;
		    case ValueType.ArrayDouble:
			    var arrDouble = new double[NativeMethods.GetVectorSizeDouble(inValue)];
			    NativeMethods.GetVectorDataDouble(inValue, arrDouble);
			    return arrDouble;
		    case ValueType.ArrayString:
			    var arrString = new string[NativeMethods.GetVectorSizeString(inValue)];
			    NativeMethods.GetVectorDataString(inValue, arrString);
			    return arrString;
		    case ValueType.Vector2:
		    case ValueType.Vector3:
		    case ValueType.Vector4:
		    case ValueType.Matrix4x4:
		    {
			    return Marshal.PtrToStructure(inValue, paramType.IsByRef ? paramType.GetElementType() : paramType);
		    }
	    }
	    
	    if (paramType.IsValueType)
	    {
		    return Marshal.PtrToStructure(inValue, paramType.IsByRef ? paramType.GetElementType() : paramType);
	    }
	    
	    throw new NotImplementedException($"Parameter type {paramType.Name} not implemented");
    }

    public static void MarshalFieldAddress(object obj, FieldInfo fieldInfo, nint outValue)
    {
	    var ptr = Unsafe.As<object, nint>(ref obj) + IntPtr.Size + GetFieldOffset(fieldInfo);
	    Marshal.WriteIntPtr(outValue, ptr);
    }
    
    private static int GetFieldOffset(this FieldInfo fieldInfo) => GetFieldOffset(fieldInfo.FieldHandle);

    private static int GetFieldOffset(RuntimeFieldHandle handle) => Marshal.ReadInt32(handle.Value + (4 + IntPtr.Size)) & 0xFFFFFF;
    
    private static Delegate GetDelegateForFunctionPointer(nint funcAddress, Type delegateType)
    {
	    MethodInfo methodInfo = delegateType.GetMethod("Invoke");
	    if (IsNeedMarshal(methodInfo.ReturnType) || methodInfo.GetParameters().Any(p => IsNeedMarshal(p.ParameterType)))
	    {
		    return MethodUtils.CreateObjectArrayDelegate(delegateType, ExternalInvoke(funcAddress, methodInfo));
	    }

	    return Marshal.GetDelegateForFunctionPointer(funcAddress, delegateType);
    }

    #region External Call from C# to C++

    private static readonly DCCallVM vm = new(4096);

    private static Func<object[], object> ExternalInvoke(nint funcAddress, MethodInfo methodInfo)
    {
	    ManagedType returnType =  new ManagedType(methodInfo.ReturnType, methodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(false));
	    ManagedType[] parameterTypes = methodInfo.GetParameters().Select(p => new ManagedType(p.ParameterType, p.GetCustomAttributes(false))).ToArray();
	   
	    bool hasRet = returnType.ValueType is >= ValueType.String and <= ValueType.ArrayString;
	    bool hasRefs = parameterTypes.Any(t => t.IsByRef);
	    
        return parameters =>
        {
	        lock (vm)
	        {
		        DCaggr? ag = null;
		        vm.Reset();

		        List<(nint, ValueType)> handlers = [];

		        #region Allocate Memory for Return

		        ValueType retType = returnType.ValueType;
		        switch (retType)
		        {
			        case ValueType.String:
			        {
				        nint ptr = NativeMethods.AllocateString();
				        handlers.Add((ptr, retType));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayBool:
			        {
				        nint ptr = NativeMethods.AllocateVectorBool();
				        handlers.Add((ptr, retType));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayChar8:
			        {
				        nint ptr = NativeMethods.AllocateVectorChar8();
				        handlers.Add((ptr, retType));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayChar16:
			        {
				        nint ptr = NativeMethods.AllocateVectorChar16();
				        handlers.Add((ptr, retType));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayInt8:
			        {
				        nint ptr = NativeMethods.AllocateVectorInt8();
				        handlers.Add((ptr, retType));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayInt16:
			        {
				        nint ptr = NativeMethods.AllocateVectorInt16();
				        handlers.Add((ptr, retType));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayInt32:
			        {
				        nint ptr = NativeMethods.AllocateVectorInt32();
				        handlers.Add((ptr, retType));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayInt64:
			        {
				        nint ptr = NativeMethods.AllocateVectorInt64();
				        handlers.Add((ptr, retType));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayUInt8:
			        {
				        nint ptr = NativeMethods.AllocateVectorUInt8();
				        handlers.Add((ptr, retType));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayUInt16:
			        {
				        nint ptr = NativeMethods.AllocateVectorUInt16();
				        handlers.Add((ptr, retType));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayUInt32:
			        {
				        nint ptr = NativeMethods.AllocateVectorUInt32();
				        handlers.Add((ptr, retType));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayUInt64:
			        {
				        nint ptr = NativeMethods.AllocateVectorUInt64();
				        handlers.Add((ptr, retType));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayPointer:
			        {
				        nint ptr = NativeMethods.AllocateVectorIntPtr();
				        handlers.Add((ptr, retType));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayFloat:
			        {
				        nint ptr = NativeMethods.AllocateVectorFloat();
				        handlers.Add((ptr, retType));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayDouble:
			        {
				        nint ptr = NativeMethods.AllocateVectorDouble();
				        handlers.Add((ptr, retType));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayString:
			        {
				        nint ptr = NativeMethods.AllocateVectorString();
				        handlers.Add((ptr, retType));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.Vector2:
			        {
				        ag = new DCaggr(2, 8);
				        for (int i = 0; i < 2; ++i)
					        ag.AddField(SignatureChars.Float, 4 * i, 1);
				        ag.Reset();
				        vm.BeginCallAggr(ag);
				        break;
			        }
			        case ValueType.Vector3:
			        {
				        ag = new DCaggr(3, 12);
				        for (int i = 0; i < 3; ++i)
					        ag.AddField(SignatureChars.Float, 4 * i, 1);
				        ag.Reset();
				        vm.BeginCallAggr(ag);
				        break;
			        }
			        case ValueType.Vector4:
			        {
				        ag = new DCaggr(4, 16);
				        for (int i = 0; i < 4; ++i)
					        ag.AddField(SignatureChars.Float, 4 * i, 1);
				        ag.Reset();
				        vm.BeginCallAggr(ag);
				        break;
			        }
			        case ValueType.Matrix4x4:
			        {
				        ag = new DCaggr(16, 64);
				        for (int i = 0; i < 16; ++i)
					        ag.AddField(SignatureChars.Float, 4 * i, 1);
				        ag.Reset();
				        vm.BeginCallAggr(ag);
				        break;
			        }
		        }
		        
		        #endregion

		        List<GCHandle> pins = [];

		        #region Set Parameters 

		        for (int i = 0; i < parameters.Length; i++)
		        {
			        object paramValue = parameters[i];
			        ManagedType paramType = parameterTypes[i];
			        ValueType valueType = paramType.ValueType;

			        if (paramType.IsByRef)
			        {
				        switch (valueType)
				        {
					        case ValueType.Bool:
						        vm.ArgPointer(Pin<bool>(paramValue, pins));
						        break;
					        case ValueType.Char8:
						        vm.ArgPointer(Pin<char>(paramValue, pins));
						        break;
					        case ValueType.Char16:
						        vm.ArgPointer(Pin<char>(paramValue, pins));
						        break;
					        case ValueType.Int8:
						        vm.ArgPointer(Pin<sbyte>(paramValue, pins));
						        break;
					        case ValueType.Int16:
						        vm.ArgPointer(Pin<short>(paramValue, pins));
						        break;
					        case ValueType.Int32:
						        vm.ArgPointer(Pin<int>(paramValue, pins));
						        break;
					        case ValueType.Int64:
						        vm.ArgPointer(Pin<long>(paramValue, pins));
						        break;
					        case ValueType.UInt8:
						        vm.ArgPointer(Pin<byte>(paramValue, pins));
						        break;
					        case ValueType.UInt16:
						        vm.ArgPointer(Pin<ushort>(paramValue, pins));
						        break;
					        case ValueType.UInt32:
						        vm.ArgPointer(Pin<ulong>(paramValue, pins));
						        break;
					        case ValueType.UInt64:
						        vm.ArgPointer(Pin<ulong>(paramValue, pins));
						        break;
					        case ValueType.Pointer:
						        vm.ArgPointer(Pin<nint>(paramValue, pins));
						        break;
					        case ValueType.Float:
						        vm.ArgPointer(Pin<float>(paramValue, pins));
						        break;
					        case ValueType.Double:
						        vm.ArgPointer(Pin<double>(paramValue, pins));
						        break;
					        case ValueType.Vector2:
						        vm.ArgPointer(Pin<Vector2>(paramValue, pins));
						        break;
					        case ValueType.Vector3:
						        vm.ArgPointer(Pin<Vector3>(paramValue, pins));
						        break;
					        case ValueType.Vector4:
						        vm.ArgPointer(Pin<Vector4>(paramValue, pins));
						        break;
					        case ValueType.Matrix4x4:
						        vm.ArgPointer(Pin<Matrix4x4>(paramValue, pins));
						        break;
					        
					        case ValueType.Function:
					        {
						        Delegate d = GetDelegateForMarshalling((Delegate)paramValue);
						        Pin<Delegate>(d, pins);
						        vm.ArgPointer(Pin<nint>(Marshal.GetFunctionPointerForDelegate(d), pins));
						        break;
					        }
					        case ValueType.String:
					        {
						        nint ptr = NativeMethods.CreateString((string)paramValue);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayBool:
					        {
						        var arr = (bool[])paramValue;
						        nint ptr = NativeMethods.CreateVectorBool(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayChar8:
					        {
						        var arr = (char[])paramValue;
						        nint ptr = NativeMethods.CreateVectorChar8(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayChar16:
					        {
						        var arr = (char[])paramValue;
						        nint ptr = NativeMethods.CreateVectorChar16(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayInt8:
					        {
						        var arr = (sbyte[])paramValue;
						        nint ptr = NativeMethods.CreateVectorInt8(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayInt16:
					        {
						        var arr = (short[])paramValue;
						        nint ptr = NativeMethods.CreateVectorInt16(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayInt32:
					        {
						        var arr = (int[])paramValue;
						        nint ptr = NativeMethods.CreateVectorInt32(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayInt64:
					        {
						        var arr = (long[])paramValue;
						        nint ptr = NativeMethods.CreateVectorInt64(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayUInt8:
					        {
						        var arr = (byte[])paramValue;
						        nint ptr = NativeMethods.CreateVectorUInt8(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayUInt16:
					        {
						        var arr = (ushort[])paramValue;
						        nint ptr = NativeMethods.CreateVectorUInt16(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayUInt32:
					        {
						        var arr = (uint[])paramValue;
						        nint ptr = NativeMethods.CreateVectorUInt32(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayUInt64:
					        {
						        var arr = (ulong[])paramValue;
						        nint ptr = NativeMethods.CreateVectorUInt64(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayPointer:
					        {
						        var arr = (nint[])paramValue;
						        nint ptr = NativeMethods.CreateVectorIntPtr(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayFloat:
					        {
						        var arr = (float[])paramValue;
						        nint ptr = NativeMethods.CreateVectorFloat(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayDouble:
					        {
						        var arr = (double[])paramValue;
						        nint ptr = NativeMethods.CreateVectorDouble(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayString:
					        {
						        var arr = (string[])paramValue;
						        nint ptr = NativeMethods.CreateVectorString(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        default:
						        throw new TypeNotFoundException();
				        }
			        }
			        else
			        {
				        switch (valueType)
				        {
					        case ValueType.Bool:
						        vm.ArgBool((bool)paramValue);
						        break;
					        case ValueType.Char8:
						        vm.ArgChar8((char)paramValue);
						        break;
					        case ValueType.Char16:
						        vm.ArgChar16((char)paramValue);
						        break;
					        case ValueType.Int8:
						        vm.ArgInt8((sbyte)paramValue);
						        break;
					        case ValueType.Int16:
						        vm.ArgInt16((short)paramValue);
						        break;
					        case ValueType.Int32:
						        vm.ArgInt32((int)paramValue);
						        break;
					        case ValueType.Int64:
						        vm.ArgInt64((long)paramValue);
						        break;
					        case ValueType.UInt8:
						        vm.ArgUInt8((byte)paramValue);
						        break;
					        case ValueType.UInt16:
						        vm.ArgUInt16((ushort)paramValue);
						        break;
					        case ValueType.UInt32:
						        vm.ArgUInt32((uint)paramValue);
						        break;
					        case ValueType.UInt64:
						        vm.ArgUInt64((ulong)paramValue);
						        break;
					        case ValueType.Pointer:
						        vm.ArgPointer((nint)paramValue);
						        break;
					        case ValueType.Float:
						        vm.ArgFloat((float)paramValue);
						        break;
					        case ValueType.Double:
						        vm.ArgDouble((double)paramValue);
						        break;
					        case ValueType.Vector2:
						        vm.ArgPointer(Pin<Vector2>(paramValue, pins));
						        break;
					        case ValueType.Vector3:
						        vm.ArgPointer(Pin<Vector3>(paramValue, pins));
						        break;
					        case ValueType.Vector4:
						        vm.ArgPointer(Pin<Vector4>(paramValue, pins));
						        break;
					        case ValueType.Matrix4x4:
						        vm.ArgPointer(Pin<Matrix4x4>(paramValue, pins));
						        break;
					        
					        case ValueType.Function:
					        {
						        Delegate d = GetDelegateForMarshalling((Delegate)paramValue);
						        Pin<Delegate>(d, pins);
						        vm.ArgPointer(Pin<nint>(Marshal.GetFunctionPointerForDelegate(d), pins));
						        break;
					        }
					        case ValueType.String:
					        {
						        nint ptr = NativeMethods.CreateString((string)paramValue);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayBool:
					        {
						        var arr = (bool[])paramValue;
						        nint ptr = NativeMethods.CreateVectorBool(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayChar8:
					        {
						        var arr = (char[])paramValue;
						        nint ptr = NativeMethods.CreateVectorChar8(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayChar16:
					        {
						        var arr = (char[])paramValue;
						        nint ptr = NativeMethods.CreateVectorChar16(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayInt8:
					        {
						        var arr = (sbyte[])paramValue;
						        nint ptr = NativeMethods.CreateVectorInt8(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayInt16:
					        {
						        var arr = (short[])paramValue;
						        nint ptr = NativeMethods.CreateVectorInt16(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayInt32:
					        {
						        var arr = (int[])paramValue;
						        nint ptr = NativeMethods.CreateVectorInt32(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayInt64:
					        {
						        var arr = (long[])paramValue;
						        nint ptr = NativeMethods.CreateVectorInt64(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayUInt8:
					        {
						        var arr = (byte[])paramValue;
						        nint ptr = NativeMethods.CreateVectorUInt8(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayUInt16:
					        {
						        var arr = (ushort[])paramValue;
						        nint ptr = NativeMethods.CreateVectorUInt16(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayUInt32:
					        {
						        var arr = (uint[])paramValue;
						        nint ptr = NativeMethods.CreateVectorUInt32(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayUInt64:
					        {
						        var arr = (ulong[])paramValue;
						        nint ptr = NativeMethods.CreateVectorUInt64(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayPointer:
					        {
						        var arr = (nint[])paramValue;
						        nint ptr = NativeMethods.CreateVectorIntPtr(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayFloat:
					        {
						        var arr = (float[])paramValue;
						        nint ptr = NativeMethods.CreateVectorFloat(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayDouble:
					        {
						        var arr = (double[])paramValue;
						        nint ptr = NativeMethods.CreateVectorDouble(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayString:
					        {
						        var arr = (string[])paramValue;
						        nint ptr = NativeMethods.CreateVectorString(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        default:
						        throw new TypeNotFoundException();
				        }
			        }
		        }
		        
		        #endregion

		        #region Call Function

		        object? ret = null;

		        switch (retType)
		        {
			        case ValueType.Void:
				        vm.CallVoid(funcAddress);
				        break;
			        case ValueType.Bool:
				        ret = vm.CallBool(funcAddress);
				        break;
			        case ValueType.Char8:
				        ret = vm.CallChar8(funcAddress);
				        break;
			        case ValueType.Char16:
				        ret = vm.CallChar16(funcAddress);
				        break;
			        case ValueType.Int8:
				        ret = vm.CallInt8(funcAddress);
				        break;
			        case ValueType.Int16:
				        ret = vm.CallInt16(funcAddress);
				        break;
			        case ValueType.Int32:
				        ret = vm.CallInt32(funcAddress);
				        break;
			        case ValueType.Int64:
				        ret = vm.CallInt64(funcAddress);
				        break;
			        case ValueType.UInt8:
				        ret = vm.CallUInt8(funcAddress);
				        break;
			        case ValueType.UInt16:
				        ret = vm.CallUInt16(funcAddress);
				        break;
			        case ValueType.UInt32:
				        ret = vm.CallUInt32(funcAddress);
				        break;
			        case ValueType.UInt64:
				        ret = vm.CallUInt64(funcAddress);
				        break;
			        case ValueType.Pointer:
				        ret = vm.CallPointer(funcAddress);
				        break;
			        case ValueType.Float:
				        ret = vm.CallFloat(funcAddress);
				        break;
			        case ValueType.Double:
				        ret = vm.CallDouble(funcAddress);
				        break;
			        case ValueType.Function: {
				        ret = GetDelegateForFunctionPointer(vm.CallPointer(funcAddress), methodInfo.ReturnType);
				        break;
			        }
			        case ValueType.Vector2:
			        {
				        Vector2 source;
				        unsafe
				        {
					        vm.CallAggr(funcAddress, ag!, &source);
				        }
				        ret = source;
				        break;
			        }
			        case ValueType.Vector3:
			        {
				        Vector3 source;
				        unsafe
				        {
					        vm.CallAggr(funcAddress, ag!, &source);
				        }
				        ret = source;
				        break;
			        }
			        case ValueType.Vector4:
			        {
				        Vector4 source;
				        unsafe
				        {
					        vm.CallAggr(funcAddress, ag!, &source);
				        }
				        ret = source;
				        break;
			        }
			        case ValueType.Matrix4x4:
			        {
				        Matrix4x4 source;
				        unsafe
				        {
					        vm.CallAggr(funcAddress, ag!, &source);
				        }
				        ret = source;
				        break;
			        }
			        case ValueType.String:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        ret = NativeMethods.GetStringData(ptr);
				        break;
			        }
			        case ValueType.ArrayBool:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new bool[NativeMethods.GetVectorSizeBool(ptr)];
				        NativeMethods.GetVectorDataBool(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayChar8:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new char[NativeMethods.GetVectorSizeChar8(ptr)];
				        NativeMethods.GetVectorDataChar8(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayChar16:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new char[NativeMethods.GetVectorSizeChar16(ptr)];
				        NativeMethods.GetVectorDataChar16(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayInt8:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new sbyte[NativeMethods.GetVectorSizeInt8(ptr)];
				        NativeMethods.GetVectorDataInt8(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayInt16:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new short[NativeMethods.GetVectorSizeInt16(ptr)];
				        NativeMethods.GetVectorDataInt16(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayInt32:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new int[NativeMethods.GetVectorSizeInt32(ptr)];
				        NativeMethods.GetVectorDataInt32(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayInt64:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new long[NativeMethods.GetVectorSizeInt64(ptr)];
				        NativeMethods.GetVectorDataInt64(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayUInt8:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new byte[NativeMethods.GetVectorSizeUInt8(ptr)];
				        NativeMethods.GetVectorDataUInt8(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayUInt16:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new ushort[NativeMethods.GetVectorSizeUInt16(ptr)];
				        NativeMethods.GetVectorDataUInt16(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayUInt32:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new uint[NativeMethods.GetVectorSizeUInt32(ptr)];
				        NativeMethods.GetVectorDataUInt32(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayUInt64:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new ulong[NativeMethods.GetVectorSizeUInt64(ptr)];
				        NativeMethods.GetVectorDataUInt64(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayPointer:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new nint[NativeMethods.GetVectorSizeIntPtr(ptr)];
				        NativeMethods.GetVectorDataIntPtr(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayFloat:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new float[NativeMethods.GetVectorSizeFloat(ptr)];
				        NativeMethods.GetVectorDataFloat(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayDouble:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new double[NativeMethods.GetVectorSizeDouble(ptr)];
				        NativeMethods.GetVectorDataDouble(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayString:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new string[NativeMethods.GetVectorSizeString(ptr)];
				        NativeMethods.GetVectorDataString(ptr, arr);
				        ret = arr;
				        break;
			        }
			        default:
				        throw new TypeNotFoundException();
		        }
		        
		        #endregion

		        #region Pull Refererences Back

		        if (hasRefs)
		        {
			        int j = hasRet ? 1 : 0; // skip first param if has return

			        if (j < handlers.Count)
			        {
				        for (int i = 0; i < parameters.Length; i++)
				        {
					        //object paramValue = parameters[i];
					        ManagedType paramType = parameterTypes[i];
					        if (paramType.IsByRef)
					        {
						        switch (paramType.ValueType)
						        {
							        case ValueType.String:
							        {
								        nint ptr = handlers[j++].Item1;
								        parameters[i] = NativeMethods.GetStringData(ptr);
								        break;
							        }
							        case ValueType.ArrayBool:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new bool[NativeMethods.GetVectorSizeBool(ptr)];
								        NativeMethods.GetVectorDataBool(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayChar8:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new char[NativeMethods.GetVectorSizeChar8(ptr)];
								        NativeMethods.GetVectorDataChar8(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayChar16:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new char[NativeMethods.GetVectorSizeChar16(ptr)];
								        NativeMethods.GetVectorDataChar16(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayInt8:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new sbyte[NativeMethods.GetVectorSizeInt8(ptr)];
								        NativeMethods.GetVectorDataInt8(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayInt16:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new short[NativeMethods.GetVectorSizeInt16(ptr)];
								        NativeMethods.GetVectorDataInt16(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayInt32:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new int[NativeMethods.GetVectorSizeInt32(ptr)];
								        NativeMethods.GetVectorDataInt32(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayInt64:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new long[NativeMethods.GetVectorSizeInt64(ptr)];
								        NativeMethods.GetVectorDataInt64(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayUInt8:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new byte[NativeMethods.GetVectorSizeUInt8(ptr)];
								        NativeMethods.GetVectorDataUInt8(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayUInt16:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new ushort[NativeMethods.GetVectorSizeUInt16(ptr)];
								        NativeMethods.GetVectorDataUInt16(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayUInt32:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new uint[NativeMethods.GetVectorSizeUInt32(ptr)];
								        NativeMethods.GetVectorDataUInt32(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayUInt64:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new ulong[NativeMethods.GetVectorSizeUInt64(ptr)];
								        NativeMethods.GetVectorDataUInt64(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayPointer:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new nint[NativeMethods.GetVectorSizeIntPtr(ptr)];
								        NativeMethods.GetVectorDataIntPtr(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayFloat:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new float[NativeMethods.GetVectorSizeFloat(ptr)];
								        NativeMethods.GetVectorDataFloat(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayDouble:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new double[NativeMethods.GetVectorSizeDouble(ptr)];
								        NativeMethods.GetVectorDataDouble(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayString:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new string[NativeMethods.GetVectorSizeString(ptr)];
								        NativeMethods.GetVectorDataString(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
						        }
					        }

					        if (j >= handlers.Count)
						        break;
				        }
			        }
		        }
		        
		        #endregion

		        #region Cleanup Temp Storage

		        if (hasRet)
		        {
			        DeleteReturn(handlers);
		        }

		        DeleteParams(handlers, hasRet);

		        foreach (var pin in pins)
		        {
			        pin.Free();
		        }
		        
		        #endregion

		        return ret;
	        }
        };
    }
    
    private static nint Pin<T>(object paramValue, List<GCHandle> pins)
    {
        var handle = GCHandle.Alloc((T)paramValue, GCHandleType.Pinned);
        pins.Add(handle);
        return handle.AddrOfPinnedObject();
    }

    private static void DeleteReturn(List<(nint, ValueType)> handlers)
    {
        var (ptr, type) = handlers[0];
        switch (type)
        {
            case ValueType.String:
                NativeMethods.FreeString(ptr);
                break;
            case ValueType.ArrayBool:
                NativeMethods.FreeVectorBool(ptr);
                break;
            case ValueType.ArrayChar8:
                NativeMethods.FreeVectorChar8(ptr);
                break;
            case ValueType.ArrayChar16:
                NativeMethods.FreeVectorChar16(ptr);
                break;
            case ValueType.ArrayInt8:
                NativeMethods.FreeVectorInt8(ptr);
                break;
            case ValueType.ArrayInt16:
                NativeMethods.FreeVectorInt16(ptr);
                break;
            case ValueType.ArrayInt32:
                NativeMethods.FreeVectorInt32(ptr);
                break;
            case ValueType.ArrayInt64:
                NativeMethods.FreeVectorInt64(ptr);
                break;
            case ValueType.ArrayUInt8:
                NativeMethods.FreeVectorUInt8(ptr);
                break;
            case ValueType.ArrayUInt16:
                NativeMethods.FreeVectorUInt16(ptr);
                break;
            case ValueType.ArrayUInt32:
                NativeMethods.FreeVectorUInt32(ptr);
                break;
            case ValueType.ArrayUInt64:
                NativeMethods.FreeVectorUInt64(ptr);
                break;
            case ValueType.ArrayPointer:
                NativeMethods.FreeVectorIntPtr(ptr);
                break;
            case ValueType.ArrayFloat:
                NativeMethods.FreeVectorFloat(ptr);
                break;
            case ValueType.ArrayDouble:
                NativeMethods.FreeVectorDouble(ptr);
                break;
            case ValueType.ArrayString:
                NativeMethods.FreeVectorString(ptr);
                break;
            default:
                throw new TypeNotFoundException();
        }
    }
    
    private static void DeleteParams(List<(nint, ValueType)> handlers, bool hasRet)
    {
	    int j = hasRet ? 1 : 0;
        for (int i = j; i < handlers.Count; i++)
        {
            var (ptr, type) = handlers[i];
            switch (type)
            {
                case ValueType.String:
                    NativeMethods.DeleteString(ptr);
                    break;
                case ValueType.ArrayBool:
                    NativeMethods.DeleteVectorBool(ptr);
                    break;
                case ValueType.ArrayChar8:
                    NativeMethods.DeleteVectorChar8(ptr);
                    break;
                case ValueType.ArrayChar16:
                    NativeMethods.DeleteVectorChar16(ptr);
                    break;
                case ValueType.ArrayInt8:
                    NativeMethods.DeleteVectorInt8(ptr);
                    break;
                case ValueType.ArrayInt16:
                    NativeMethods.DeleteVectorInt16(ptr);
                    break;
                case ValueType.ArrayInt32:
                    NativeMethods.DeleteVectorInt32(ptr);
                    break;
                case ValueType.ArrayInt64:
                    NativeMethods.DeleteVectorInt64(ptr);
                    break;
                case ValueType.ArrayUInt8:
                    NativeMethods.DeleteVectorUInt8(ptr);
                    break;
                case ValueType.ArrayUInt16:
                    NativeMethods.DeleteVectorUInt16(ptr);
                    break;
                case ValueType.ArrayUInt32:
                    NativeMethods.DeleteVectorUInt32(ptr);
                    break;
                case ValueType.ArrayUInt64:
                    NativeMethods.DeleteVectorUInt64(ptr);
                    break;
                case ValueType.ArrayPointer:
                    NativeMethods.DeleteVectorIntPtr(ptr);
                    break;
                case ValueType.ArrayFloat:
                    NativeMethods.DeleteVectorFloat(ptr);
                    break;
                case ValueType.ArrayDouble:
                    NativeMethods.DeleteVectorDouble(ptr);
                    break;
                case ValueType.ArrayString:
                    NativeMethods.DeleteVectorString(ptr);
                    break;
                default:
                    throw new TypeNotFoundException();
            }
        }
    }
    
    #endregion

    private static Delegate GetDelegateForMarshalling(Delegate d)
    {
	    MethodInfo methodInfo = d.Method;
	    if (IsNeedMarshal(methodInfo.ReturnType) || methodInfo.GetParameters().Any(p => IsNeedMarshal(p.ParameterType)))
	    {
		    // Convert all string and array types into nint, if string and array return, append 1st hidden parameter
		    
		    List<Type> types = [];
		        
		    Type returnType = methodInfo.ReturnType;
		    bool hasHidden = TypeUtils.ConvertToValueType(returnType) is >= ValueType.String and <= ValueType.ArrayString;
		    if (hasHidden)
		    {
			    types.Add(typeof(nint));
		    }
		    
		    foreach (var parameterInfo in methodInfo.GetParameters())
		    {
			    Type paramType = parameterInfo.ParameterType;
			    types.Add(TypeUtils.ConvertToValueType(paramType) is >= ValueType.String and <= ValueType.ArrayString
				    ? typeof(nint)
				    : paramType);
		    }

		    types.Add(hasHidden ? typeof(nint) : returnType);

		    Type delegateType = Expression.GetDelegateType(types.ToArray());
		    return MethodUtils.CreateObjectArrayDelegate(delegateType, InternalInvoke(d));
	    }

	    return d;
    }

    #region External Call from C++ to C#
    
    private static Func<object[], object> InternalInvoke(Delegate d)
    {
	    MethodInfo methodInfo = d.Method;
	    
	    ManagedType returnType =  new ManagedType(methodInfo.ReturnType, methodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(false));
	    ManagedType[] parameterTypes = methodInfo.GetParameters().Select(p => new ManagedType(p.ParameterType, p.GetCustomAttributes(false))).ToArray();

	    bool hasRet = returnType.ValueType is >= ValueType.String and <= ValueType.ArrayString;
	    bool hasRefs = parameterTypes.Any(t => t.IsByRef);
	    int startIndex = hasRet ? 1 : 0;
	    
	    return parameters =>
	    {
		    object[] args = new object[parameterTypes.Length];
		    
		    for (int i = startIndex, j = 0; i < parameters.Length; i++, j++)
		    {
			    args[j] = SetParam(parameterTypes[j], parameters[i]);
		    }

		    object? ret = d.DynamicInvoke(args);

		    if (hasRefs)
		    {
			    for (int i = startIndex, j = 0; i < parameters.Length; i++, j++)
			    {
				    SetReference(parameterTypes[j], parameters[i], args[j]);
			    }
		    }
		    
		    return hasRet ? SetReturn(returnType, parameters[0], ret) : ret!;
	    };
    }

    private static object SetParam(ManagedType paramType, object paramValue)
    {
	    switch (paramType.ValueType)
	    {
		    case ValueType.String:
		    {
				return NativeMethods.GetStringData((nint)paramValue);
		    }
		    case ValueType.ArrayBool:
		    {
			    var ptr = (nint)paramValue;
			    var arr = new bool[NativeMethods.GetVectorSizeBool(ptr)];
			    NativeMethods.GetVectorDataBool(ptr, arr);
			    return arr;
		    }
		    case ValueType.ArrayChar8:
		    {
			    var ptr = (nint)paramValue;
			    var arr = new char[NativeMethods.GetVectorSizeChar8(ptr)];
			    NativeMethods.GetVectorDataChar8(ptr, arr);
			    return arr;
		    }
		    case ValueType.ArrayChar16:
		    {
			    var ptr = (nint)paramValue;
			    var arr = new char[NativeMethods.GetVectorSizeChar16(ptr)];
			    NativeMethods.GetVectorDataChar16(ptr, arr);
			    return arr;
		    }
		    case ValueType.ArrayInt8:
		    {
			    var ptr = (nint)paramValue;
			    var arr = new sbyte[NativeMethods.GetVectorSizeInt8(ptr)];
			    NativeMethods.GetVectorDataInt8(ptr, arr);
			    return arr;
		    }
		    case ValueType.ArrayInt16:
		    {
			    var ptr = (nint)paramValue;
			    var arr = new short[NativeMethods.GetVectorSizeInt16(ptr)];
			    NativeMethods.GetVectorDataInt16(ptr, arr);
			    return arr;
		    }
		    case ValueType.ArrayInt32:
		    {
			    var ptr = (nint)paramValue;
			    var arr = new int[NativeMethods.GetVectorSizeInt32(ptr)];
			    NativeMethods.GetVectorDataInt32(ptr, arr);
			    return arr;
		    }
		    case ValueType.ArrayInt64:
		    {
			    var ptr = (nint)paramValue;
			    var arr = new long[NativeMethods.GetVectorSizeInt64(ptr)];
			    NativeMethods.GetVectorDataInt64(ptr, arr);
			    return arr;
		    }
		    case ValueType.ArrayUInt8:
		    {
			    var ptr = (nint)paramValue;
			    var arr = new byte[NativeMethods.GetVectorSizeUInt8(ptr)];
			    NativeMethods.GetVectorDataUInt8(ptr, arr);
			    return arr;
		    }
		    case ValueType.ArrayUInt16:
		    {
			    var ptr = (nint)paramValue;
			    var arr = new ushort[NativeMethods.GetVectorSizeUInt16(ptr)];
			    NativeMethods.GetVectorDataUInt16(ptr, arr);
			    return arr;
		    }
		    case ValueType.ArrayUInt32:
		    {
			    var ptr = (nint)paramValue;
			    var arr = new uint[NativeMethods.GetVectorSizeUInt32(ptr)];
			    NativeMethods.GetVectorDataUInt32(ptr, arr);
			    return arr;
		    }
		    case ValueType.ArrayUInt64:
		    {
			    var ptr = (nint)paramValue;
			    var arr = new ulong[NativeMethods.GetVectorSizeUInt64(ptr)];
			    NativeMethods.GetVectorDataUInt64(ptr, arr);
			    return arr;
		    }
		    case ValueType.ArrayPointer:
		    {
			    var ptr = (nint)paramValue;
			    var arr = new nint[NativeMethods.GetVectorSizeIntPtr(ptr)];
			    NativeMethods.GetVectorDataIntPtr(ptr, arr);
			    return arr;
		    }
		    case ValueType.ArrayFloat:
		    {
			    var ptr = (nint)paramValue;
			    var arr = new float[NativeMethods.GetVectorSizeFloat(ptr)];
			    NativeMethods.GetVectorDataFloat(ptr, arr);
			    return arr;
		    }
		    case ValueType.ArrayDouble:
		    {
			    var ptr = (nint)paramValue;
			    var arr = new double[NativeMethods.GetVectorSizeDouble(ptr)];
			    NativeMethods.GetVectorDataDouble(ptr, arr);
			    return arr;
		    }
		    case ValueType.ArrayString:
		    {
			    var ptr = (nint)paramValue;
			    var arr = new string[NativeMethods.GetVectorSizeString(ptr)];
			    NativeMethods.GetVectorDataString(ptr, arr);
			    return arr;
		    }
		    default:
			    return paramValue;
	    }
    }

    private static void SetReference(ManagedType paramType, object paramValue, object arg)
    {
	    if (!paramType.IsByRef)
	    {
		    return;
	    }
	    
	    switch (paramType.ValueType)
	    {
		    case ValueType.String:
		    {
			    NativeMethods.AssignString((nint)paramValue, (string)arg);
			    break;
		    }
		    case ValueType.ArrayBool:
		    {
			    var arr = (bool[])arg;
			    NativeMethods.AssignVectorBool((nint)paramValue, arr, arr.Length);
			    break;
		    }
		    case ValueType.ArrayChar8:
		    {
			    var arr = (char[])arg;
			    NativeMethods.AssignVectorChar8((nint)paramValue, arr, arr.Length);
			    break;
		    }
		    case ValueType.ArrayChar16:
		    {
			    var arr = (char[])arg;
			    NativeMethods.AssignVectorChar16((nint)paramValue, arr, arr.Length);
			    break;
		    }
		    case ValueType.ArrayInt8:
		    {
			    var arr = (sbyte[])arg;
			    NativeMethods.AssignVectorInt8((nint)paramValue, arr, arr.Length);
			    break;
		    }
		    case ValueType.ArrayInt16:
		    {
			    var arr = (short[])arg;
			    NativeMethods.AssignVectorInt16((nint)paramValue, arr, arr.Length);
			    break;
		    }
		    case ValueType.ArrayInt32:
		    {
			    var arr = (int[])arg;
			    NativeMethods.AssignVectorInt32((nint)paramValue, arr, arr.Length);
			    break;
		    }
		    case ValueType.ArrayInt64:
		    {
			    var arr = (long[])arg;
			    NativeMethods.AssignVectorInt64((nint)paramValue, arr, arr.Length);
			    break;
		    }
		    case ValueType.ArrayUInt8:
		    {
			    var arr = (byte[])arg;
			    NativeMethods.AssignVectorUInt8((nint)paramValue, arr, arr.Length);
			    break;
		    }
		    case ValueType.ArrayUInt16:
		    {
			    var arr = (ushort[])arg;
			    NativeMethods.AssignVectorUInt16((nint)paramValue, arr, arr.Length);
			    break;
		    }
		    case ValueType.ArrayUInt32:
		    {
			    var arr = (uint[])arg;
			    NativeMethods.AssignVectorUInt32((nint)paramValue, arr, arr.Length);
			    break;
		    }
		    case ValueType.ArrayUInt64:
		    {
			    var arr = (ulong[])arg;
			    NativeMethods.AssignVectorUInt64((nint)paramValue, arr, arr.Length);
			    break;
		    }
		    case ValueType.ArrayPointer:
		    {
			    var arr = (nint[])arg;
			    NativeMethods.AssignVectorIntPtr((nint)paramValue, arr, arr.Length);
			    break;
		    }
		    case ValueType.ArrayFloat:
		    {
			    var arr = (float[])arg;
			    NativeMethods.AssignVectorFloat((nint)paramValue, arr, arr.Length);
			    break;
		    }
		    case ValueType.ArrayDouble:
		    {
			    var arr = (double[])arg;
			    NativeMethods.AssignVectorDouble((nint)paramValue, arr, arr.Length);
			    break;
		    }
		    case ValueType.ArrayString:
		    {
			    var arr = (string[])arg;
			    NativeMethods.AssignVectorString((nint)paramValue, arr, arr.Length);
			    break;
		    }
	    }
    }

    private static nint SetReturn(ManagedType returnType, object returnValue, object? ret)
    {
	    if (ret == null)
	    {
		    throw new Exception("Return cannot be null");
	    }

	    nint ptr = (nint)returnValue;
	    switch (returnType.ValueType)
	    {
		    case ValueType.String:
		    {
			    NativeMethods.ConstructString(ptr, (string)ret);
			    break;
		    }
		    case ValueType.ArrayBool:
		    {
			    var retArray = (bool[])ret;
			    NativeMethods.ConstructVectorBool(ptr, retArray, retArray.Length);
			    break;
		    }
		    case ValueType.ArrayChar8:
		    {
			    var retArray = (char[])ret;
			    NativeMethods.ConstructVectorChar8(ptr, retArray, retArray.Length);
			    break;
		    }
		    case ValueType.ArrayChar16:
		    {
			    var retArray = (char[])ret;
			    NativeMethods.ConstructVectorChar16(ptr, retArray, retArray.Length);
			    break;
		    }
		    case ValueType.ArrayInt8:
		    {
			    var retArray = (sbyte[])ret;
			    NativeMethods.ConstructVectorInt8(ptr, retArray, retArray.Length);
			    break;
		    }
		    case ValueType.ArrayInt16:
		    {
			    var retArray = (short[])ret;
			    NativeMethods.ConstructVectorInt16(ptr, retArray, retArray.Length);
			    break;
		    }
		    case ValueType.ArrayInt32:
		    {
			    var retArray = (int[])ret;
			    NativeMethods.ConstructVectorInt32(ptr, retArray, retArray.Length);
			    break;
		    }
		    case ValueType.ArrayInt64:
		    {
			    var retArray = (long[])ret;
			    NativeMethods.ConstructVectorInt64(ptr, retArray, retArray.Length);
			    break;
		    }
		    case ValueType.ArrayUInt8:
		    {
			    var retArray = (byte[])ret;
			    NativeMethods.ConstructVectorUInt8(ptr, retArray, retArray.Length);
			    break;
		    }
		    case ValueType.ArrayUInt16:
		    {
			    var retArray = (ushort[])ret;
			    NativeMethods.ConstructVectorUInt16(ptr, retArray, retArray.Length);
			    break;
		    }
		    case ValueType.ArrayUInt32:
		    {
			    var retArray = (uint[])ret;
			    NativeMethods.ConstructVectorUInt32(ptr, retArray, retArray.Length);
			    break;
		    }
		    case ValueType.ArrayUInt64:
		    {
			    var retArray = (ulong[])ret;
			    NativeMethods.ConstructVectorUInt64(ptr, retArray, retArray.Length);
			    break;
		    }
		    case ValueType.ArrayPointer:
		    {
			    var retArray = (nint[])ret;
			    NativeMethods.ConstructVectorIntPtr(ptr, retArray, retArray.Length);
			    break;
		    }
		    case ValueType.ArrayFloat:
		    {
			    var retArray = (float[])ret;
			    NativeMethods.ConstructVectorFloat(ptr, retArray, retArray.Length);
			    break;
		    }
		    case ValueType.ArrayDouble:
		    {
			    var retArray = (double[])ret;
			    NativeMethods.ConstructVectorDouble(ptr, retArray, retArray.Length);
			    break;
		    }
		    case ValueType.ArrayString:
		    {
			    var retArray = (string[])ret;
			    NativeMethods.ConstructVectorString(ptr, retArray, retArray.Length);
			    break;
		    }
	    }

	    return ptr;
    }

    #endregion
    
    private static bool IsNeedMarshal(Type paramType)
    {
	    ValueType valueType = TypeUtils.ConvertToValueType(paramType);
	    if (valueType == ValueType.Function)
	    {
		    var methodInfo = paramType.GetMethod("Invoke");
		    if (IsNeedMarshal(methodInfo.ReturnType) || methodInfo.GetParameters().Any(p => IsNeedMarshal(p.ParameterType)))
		    {
			    return true;
		    }
	    }
	    return valueType is >= ValueType.String and <= ValueType.ArrayString;
    }
}
