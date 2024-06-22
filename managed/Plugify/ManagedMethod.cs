using System;
using System.Runtime.InteropServices;

namespace Plugify
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ManagedMethod
    {
        public Guid guid;
    }
}