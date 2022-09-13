using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace IndicoToolkit
{
    public static class ExtensionMethods
    {
        public static List<T> CloneList<T>(this List<T> oldList)  
        {  
            BinaryFormatter formatter = new BinaryFormatter();  
            MemoryStream stream = new MemoryStream();  
            formatter.Serialize(stream, oldList);  
            stream.Position = 0;  
            return (List<T>)formatter.Deserialize(stream);      
        } 
    }
}