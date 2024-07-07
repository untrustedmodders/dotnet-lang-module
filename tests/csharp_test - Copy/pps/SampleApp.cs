using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Plugify;

//generated with https://github.com/untrustedmodders/dotnet-lang-module/blob/main/generator/generator.py from SampleApp 

namespace SampleApp
{

	internal static unsafe class SampleApp
	{
		private static Dictionary<Delegate, Delegate> s_DelegateHolder = new();
		private static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value) where TKey : notnull
		{
			if (dict.TryGetValue(key, out TValue? val))
			{
				return val;
			}
			dict.Add(key, value);
			return value;
		}

		internal static delegate* <int, string, void> MyExportFunction = &___MyExportFunction;
		internal static delegate* unmanaged[Cdecl]<int, nint, void> __MyExportFunction;
		private static void ___MyExportFunction(int a, string b)
		{
			var __b = NativeMethods.CreateString(b);

			__MyExportFunction(a, __b);

			NativeMethods.DeleteString(__b);

		}
	}
}
