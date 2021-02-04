using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class GlobalData
{
    // Data for a new game ... default
    private static string level = "SceneOne";
    private static string titleName = "Title";
    private static float[] checkpoint = new float[3] { 0, 0, 0 };
    private static int checkpointID = 0;
    private static int saveFileIndex = 0;

    public static void SaveData(float[] pt, int id)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string fileName = "/file" + saveFileIndex + ".data";
        string path = Application.persistentDataPath + fileName;
        FileStream stream = new FileStream(path, FileMode.Create);
        SaveDataType data = new SaveDataType(pt, id);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveDataType LoadData(int fileIndex)
    {
        string fileName = "/file" + fileIndex + ".data";
        string path = Application.persistentDataPath + fileName;
        saveFileIndex = fileIndex;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            SaveDataType data = formatter.Deserialize(stream) as SaveDataType;
            stream.Close();
            checkpoint = data.checkpoint;
            checkpointID = data.checkPointId;
            return data;
        }
        else
        {
            return null;
        }
    }

    public static void DeleteCurrentFile()
    {
        string fileName = "/file" + saveFileIndex + ".data";
        string path = Application.persistentDataPath + fileName;
        checkpoint = new float[3] { 0, 0, 0 };
        File.Delete(path);
    }

    public static string GetTitleSceneName()
    {
        return titleName;
    }

    public static string GetLevelName()
    {
        return level;
    }

    public static float[] GetCheckpoint()
    {
        return checkpoint;
    }

    public static int GetCheckpointID()
    {
        return checkpointID;
    }
}
