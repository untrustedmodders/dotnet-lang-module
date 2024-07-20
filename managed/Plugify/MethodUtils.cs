using System.Reflection;
using System.Reflection.Emit;

namespace Plugify;

internal static class MethodUtils
{
    private static readonly MethodInfo FuncInvoke = typeof(Func<object[], object>).GetMethod("Invoke");
    private static readonly MethodInfo ArrayEmpty = typeof(Array).GetMethod(nameof(Array.Empty))!.MakeGenericMethod(typeof(object));

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
        MethodInfo delegateInvokeMethod = delegateType.GetMethod("Invoke");

        Type returnType = delegateInvokeMethod.ReturnType;
        bool hasReturnValue = returnType != typeof(void);

        ParameterInfo[] parameters = delegateInvokeMethod.GetParameters();
        Type[] paramTypes = new Type[parameters.Length + 1];
        paramTypes[0] = typeof(Func<object[], object>);
        for (int i = 0; i < parameters.Length; i++)
        {
            paramTypes[i + 1] = parameters[i].ParameterType;
        }

        DynamicMethod thunkMethod = new DynamicMethod("Thunk", returnType, paramTypes);
        ILGenerator ilgen = thunkMethod.GetILGenerator();

        LocalBuilder argArray = ilgen.DeclareLocal(typeof(object[]));
        LocalBuilder retValue = ilgen.DeclareLocal(typeof(object));

        // create the argument array
        if (parameters.Length == 0)
        {
            ilgen.Emit(OpCodes.Call, ArrayEmpty);
        }
        else
        {
            ilgen.Emit(OpCodes.Ldc_I4, parameters.Length);
            ilgen.Emit(OpCodes.Newarr, typeof(object));
        }
        ilgen.Emit(OpCodes.Stloc, argArray);

        // populate object array
        bool hasRefArgs = false;
        for (int i = 0; i < parameters.Length; i++)
        {
            Type paramType = parameters[i].ParameterType;
            bool paramIsByReference = paramType.IsByRef;
            if (paramIsByReference)
                paramType = paramType.GetElementType();

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
        ilgen.Emit(OpCodes.Callvirt, FuncInvoke);
        ilgen.Emit(OpCodes.Stloc, retValue);

        if (hasRefArgs)
        {
            // copy back ref/out args
            ilgen.BeginFinallyBlock();
            for (int i = 0; i < parameters.Length; i++)
            {
                var paramType = parameters[i].ParameterType;
                if (paramType.IsByRef)
                {
                    Type byrefToType = paramType.GetElementType();

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
