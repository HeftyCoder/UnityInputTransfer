using System;
using System.Collections.Generic;
using System.IO;

namespace Hefty.GenericExtensions
{
    public static class GenericExtensions
    {
        public static void Clear(this MemoryStream source)
        {
            byte[] buffer = source.GetBuffer();
            Array.Clear(buffer, 0, buffer.Length);
            source.Position = 0;
            source.SetLength(0);
        }
    }
}