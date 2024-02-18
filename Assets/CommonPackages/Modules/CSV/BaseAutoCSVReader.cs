namespace Modules.CSV
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Modules.Converters;

    public class BaseAutoCSVReader<T>
    {
        private CSVTable Table;
        protected Dictionary<string, int> StopIndices = new Dictionary<string, int>();
        public bool SkipCorrupted;
        public bool StopAtFirstCorrupted;
        public T RealLoadNative(string typeName, string path, string nativeKey, string nativeVal)
        {
            Type type = Type.GetType(typeName);
            Object instanceObj = Activator.CreateInstance(type);
            CSVLoader csvLoader = new CSVLoader();
            Table = csvLoader.LoadCSVEncrypted(path);
            foreach (CSVRecord record in Table.Records)
            {
                FieldInfo nativeKeyInfo = type.GetField(record.GetField(nativeKey));
                if (nativeKeyInfo != null)
                {
                    nativeKeyInfo.SetValue(instanceObj, TypeConverter.DOConvert(record.GetField(nativeVal), nativeKeyInfo.FieldType));
                }
            }
            return (T)instanceObj;
        }

        public List<T> RealLoadConfig(string typeName, string path)
        {
            List<T> resultT = new List<T>();
            Type type = Type.GetType(typeName);
            CSVLoader csvLoader = new CSVLoader();
            Table = csvLoader.LoadCSVEncrypted(path);
            bool shouldStop = false;

            for (var i = 0; i < Table.Records.Count; i++)
            {
                CSVRecord record = Table.Records[i];
                Object instanceObj = Activator.CreateInstance(type);

                foreach (string header in Table.Headers)
                {
                    FieldInfo fieldInfo = type.GetField(header);
                    if (fieldInfo != null)
                    {
                        if (string.IsNullOrEmpty(record.GetField(header)))
                        {
                            if (!StopIndices.ContainsKey(header)) StopIndices[header] = i;
                            if (!shouldStop)
                                shouldStop = StopAtFirstCorrupted;
                            if (SkipCorrupted)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (i == Table.Records.Count - 1)
                            {
                                StopIndices[header] = i + 1;
                            }
                        }
                        fieldInfo.SetValue(instanceObj, TypeConverter.DOConvert(record.GetField(header), fieldInfo.FieldType));
                    }
                    else
                        continue;
                }
                if (shouldStop) break;
                resultT.Add((T)instanceObj);
            }
            return resultT;
        }
    }
}