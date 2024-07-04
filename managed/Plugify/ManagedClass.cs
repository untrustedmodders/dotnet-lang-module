﻿using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Reflection;

namespace Plugify;

public delegate ManagedObject NewObjectDelegate();
public delegate void FreeObjectDelegate(ManagedObject obj);

internal class DelegateCache
{
    public static DelegateCache Instance { get; } = new();

    private readonly Dictionary<Guid, GCHandle> _pinnedDelegates = new();

    public void AddToCache(Guid delegateGuid, GCHandle handle)
    {
        if (!_pinnedDelegates.TryAdd(delegateGuid, handle))
        {
            throw new Exception("Delegate already exists in cache");
        }
    }

    public void RemoveFromCache(Guid delegateGuid)
    {
        if (!_pinnedDelegates.TryGetValue(delegateGuid, out var @delegate))
        {
            throw new Exception("Delegate does not exist in cache");
        }
            
        @delegate.Free();
        _pinnedDelegates.Remove(delegateGuid);
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct ManagedClass
{
    private int typeHash;
    private nint classObjectPtr;
    private Guid assemblyGuid;
    private Guid newObjectGuid;
    private Guid freeObjectGuid;

    public void AddMethod(string methodName, Guid guid, MethodInfo methodInfo)
    {
        ManagedType returnType = new ManagedType(methodInfo.ReturnType);

        ParameterInfo[] parameterInfos = methodInfo.GetParameters();
        ManagedType[] parameterTypesPtrs = new ManagedType[parameterInfos.Length];
        
        for (int i = 0; i < parameterInfos.Length; i++)
        {
            ParameterInfo parameterInfo = parameterInfos[i];
            parameterTypesPtrs[i] = new ManagedType(parameterInfo.ParameterType);
        }

        nint methodNamePtr = Marshal.StringToHGlobalAnsi(methodName);

        ManagedClass_AddMethod(this, methodNamePtr, guid, returnType, (uint)parameterTypesPtrs.Length, parameterTypesPtrs);

        Marshal.FreeHGlobal(methodNamePtr);
    }

    public NewObjectDelegate NewObjectFunction
    {
        set
        {
            if (newObjectGuid != Guid.Empty)
            {
                DelegateCache.Instance.RemoveFromCache(newObjectGuid);
            }

            newObjectGuid = Guid.NewGuid();

            GCHandle handle = GCHandle.Alloc(value);
            DelegateCache.Instance.AddToCache(newObjectGuid, handle);

            ManagedClass_SetNewObjectFunction(this, Marshal.GetFunctionPointerForDelegate(value));
        }
    }

    public FreeObjectDelegate FreeObjectFunction
    {
        set
        {
            if (freeObjectGuid != Guid.Empty)
            {
                DelegateCache.Instance.RemoveFromCache(freeObjectGuid);
            }

            freeObjectGuid = Guid.NewGuid();

            GCHandle handle = GCHandle.Alloc(value);
            DelegateCache.Instance.AddToCache(freeObjectGuid, handle);

            ManagedClass_SetFreeObjectFunction(this, Marshal.GetFunctionPointerForDelegate(value));
        }
    }

    // Add a function pointer to the managed class
    [DllImport(NativeMethods.DllName)]
    private static extern void ManagedClass_AddMethod(ManagedClass managedClass, nint methodNamePtr, Guid guid, ManagedType returnType, uint numParameters, ManagedType[] parameterTypes);

    [DllImport(NativeMethods.DllName)]
    private static extern void ManagedClass_SetNewObjectFunction(ManagedClass managedClass, nint newObjectFunctionPtr);

    [DllImport(NativeMethods.DllName)]
    private static extern void ManagedClass_SetFreeObjectFunction(ManagedClass managedClass, nint freeObjectFunctionPtr);
}
