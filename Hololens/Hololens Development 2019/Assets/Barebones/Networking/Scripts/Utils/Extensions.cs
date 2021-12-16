using System;
using System.Collections.Generic;
using System.IO;
namespace Barebones.Networking
{
    public static class Extensions
    {
        public static byte[] ToBytesExtension(this ISerializablePacket packet)
        {
            byte[] b;
            using (var ms = new MemoryStream())
            {
                using (var writer = new EndianBinaryWriter(EndianBitConverter.Big, ms))
                {
                    packet.ToBinaryWriter(writer);
                }

                b = ms.ToArray();
            }
            return b;
        }
    }
}
