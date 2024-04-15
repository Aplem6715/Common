using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Aplem.Common
{
    public static class ListExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> AsSpan<T>(this List<T> list)
        {
            return Unsafe.As<ListDummy<T>>(list).Items.AsSpan(0, list.Count);
        }

        class ListDummy<T>
        {
            internal T[] Items;
        }
    }
}
