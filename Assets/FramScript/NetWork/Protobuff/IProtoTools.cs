using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProtoBuf;

using System;
using System.IO;

public class IProtoTools
{
    //序列化
    public static byte[] Serialize(IExtensible msg)
    {
        byte[] data;

        using (var stream = new MemoryStream())
        {
            Serializer.Serialize(stream, msg);

            data = stream.ToArray();
        }

        return data;
    }

    //反序列化
    public static IExtensible Deserialize<IExtensible>(byte[] msg)
    {
        IExtensible data;

        using (var stream = new MemoryStream(msg))
        {
            data = Serializer.Deserialize<IExtensible>(stream);
        }

        return data;
    }
}
