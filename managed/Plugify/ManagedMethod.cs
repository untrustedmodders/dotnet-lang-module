﻿using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Plugify
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ManagedMethod
    {
        public Guid guid;
    }
    
    public static class MethodUtils
    {
        private static readonly MethodInfo s_FuncInvoke = typeof(Func<object[], object>).GetMethod("Invoke")!;
        private static readonly MethodInfo s_ArrayEmpty = typeof(Array).GetMethod(nameof(Array.Empty))!.MakeGenericMethod(typeof(object));

        // https://github.com/mono/corefx/blob/main/src/System.Linq.Expressions/src/System/Dynamic/Utils/DelegateHelpers.cs
        // We will generate the following code:
        //
        // object ret;
        // object[] args = new object[parameterCount];
        // args[0] = param0;
        // args[1] = param1;
        //  ...
        // try {
        //      ret = handler.Invoke(args);
        // } finally {
        //      param0 = (T0)args[0]; // only generated for each byref argument
        // }
        // return (TRet)ret;
        public static Delegate CreateObjectArrayDelegate(Type delegateType, Func<object[], object> handler)
        {
            MethodInfo delegateInvokeMethod = delegateType.GetMethod("Invoke") ?? throw new InvalidOperationException();

            Type returnType = delegateInvokeMethod.ReturnType;
            bool hasReturnValue = returnType != typeof(void);

            ParameterInfo[] parameterInfos = delegateInvokeMethod.GetParameters();
            Type[] paramTypes = new Type[parameterInfos.Length + 1];
            paramTypes[0] = typeof(Func<object[], object>);
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                paramTypes[i + 1] = parameterInfos[i].ParameterType;
            }

            DynamicMethod thunkMethod = new DynamicMethod("Thunk", returnType, paramTypes);
            ILGenerator ilgen = thunkMethod.GetILGenerator();

            LocalBuilder argArray = ilgen.DeclareLocal(typeof(object[]));
            LocalBuilder retValue = ilgen.DeclareLocal(typeof(object));

            // create the argument array
            if (parameterInfos.Length == 0)
            {
                ilgen.Emit(OpCodes.Call, s_ArrayEmpty);
            }
            else
            {
                ilgen.Emit(OpCodes.Ldc_I4, parameterInfos.Length);
                ilgen.Emit(OpCodes.Newarr, typeof(object));
            }
            ilgen.Emit(OpCodes.Stloc, argArray);

            // populate object array
            bool hasRefArgs = false;
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                Type paramType = parameterInfos[i].ParameterType;
                bool paramIsByReference = paramType.IsByRef;
                if (paramIsByReference)
                    paramType = paramType.GetElementType() ?? throw new InvalidOperationException();

                hasRefArgs = hasRefArgs || paramIsByReference;

                ilgen.Emit(OpCodes.Ldloc, argArray);
                ilgen.Emit(OpCodes.Ldc_I4, i);
                ilgen.Emit(OpCodes.Ldarg, i + 1);

                if (paramIsByReference)
                {
                    ilgen.Emit(OpCodes.Ldobj, paramType);
                }
                Type boxType = ConvertToBoxableType(paramType);
                ilgen.Emit(OpCodes.Box, boxType);
                ilgen.Emit(OpCodes.Stelem_Ref);
            }

            if (hasRefArgs)
            {
                ilgen.BeginExceptionBlock();
            }

            // load delegate
            ilgen.Emit(OpCodes.Ldarg_0);

            // load array
            ilgen.Emit(OpCodes.Ldloc, argArray);

            // invoke Invoke
            ilgen.Emit(OpCodes.Callvirt, s_FuncInvoke);
            ilgen.Emit(OpCodes.Stloc, retValue);

            if (hasRefArgs)
            {
                // copy back ref/out args
                ilgen.BeginFinallyBlock();
                for (int i = 0; i < parameterInfos.Length; i++)
                {
                    if (parameterInfos[i].ParameterType.IsByRef)
                    {
                        Type byrefToType = parameterInfos[i].ParameterType.GetElementType() ?? throw new InvalidOperationException();

                        // update parameter
                        ilgen.Emit(OpCodes.Ldarg, i + 1);
                        ilgen.Emit(OpCodes.Ldloc, argArray);
                        ilgen.Emit(OpCodes.Ldc_I4, i);
                        ilgen.Emit(OpCodes.Ldelem_Ref);
                        ilgen.Emit(OpCodes.Unbox_Any, byrefToType);
                        ilgen.Emit(OpCodes.Stobj, byrefToType);
                    }
                }
                ilgen.EndExceptionBlock();
            }

            if (hasReturnValue)
            {
                ilgen.Emit(OpCodes.Ldloc, retValue);
                ilgen.Emit(OpCodes.Unbox_Any, ConvertToBoxableType(returnType));
            }

            ilgen.Emit(OpCodes.Ret);

            // TODO: we need to cache these.
            return thunkMethod.CreateDelegate(delegateType, handler);
        }

        private static Type ConvertToBoxableType(Type t)
        {
            return t.IsPointer ? typeof(nint) : t;
        }
    }
}