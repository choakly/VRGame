using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class JSONFileHandler
{
    public static void SaveToJSON<T>(List<T> toSave, string fileName)
    {
        //Debug.Log(GetPath(fileName));
        string content = JSONHelper.ToJSON<T>(toSave.ToArray());
        WriteFile(GetPath(fileName), content);
        WriteFile(GetPathUser(fileName), content);
    }

    public static void SaveToJSON<T>(T toSave, string fileName)
    {
        string content = JsonUtility.ToJson(toSave);
        WriteFile(GetPath(fileName), content);
        WriteFile(GetPathUser(fileName), content);
    }

    public static List<T> ReadListFromJSON<T>(string fileName)
    {
        string content = ReadFile(GetPath(fileName));

        if(string.IsNullOrEmpty(content) || content == "{}") return new List<T>();

        List<T> res = JSONHelper.FromJSON<T>(content).ToList();

        return res;
    }

    public static T ReadFromJSON<T>(string fileName)
    {
        string content = ReadFile(GetPath(fileName));

        if(string.IsNullOrEmpty(content) || content == "{}")
            return default(T);

        T res = JsonUtility.FromJson<T>(content);

        return res;
    }

    private static string GetPath(string fileName)
    {
        //return Application.persistentDataPath + "/" + fileName;
        return Application.dataPath + "/" + fileName;
    }

    private static string GetPathUser(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName;
    }

    private static void WriteFile(string path, string content)
    {
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using(StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
        Debug.Log(path);
    }

    private static string ReadFile(string path)
    {
        if(File.Exists(path))
        {
            using(StreamReader reader = new StreamReader(path))
            {
                string content = reader.ReadToEnd();
                return content;
            }
        }
        return "";
    }

    public static class JSONHelper
    {
        public static T[] FromJSON<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJSON<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJSON<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}