using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class Serializer {
    public static byte[] Serialize(object o) {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream()) {
            bf.Serialize(ms, o);
            return ms.ToArray();
        }
    }   
    public static T Deserialize<T>(byte[] bytes) {
        using (var memStream = new MemoryStream()) {
            var binForm = new BinaryFormatter();
            memStream.Write(bytes, 0, bytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream);
            return (T)obj;
        }
    }
    public static T Copy<T>(T o) {
        return Deserialize<T>(Serialize(o));
    }
}