using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void SavePlayer(SceneControll sceneController) {

            string path = Application.persistentDataPath + "/saveFile.zss";
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);
            PlayerSaveData data = new PlayerSaveData(sceneController);
            formatter.Serialize(stream, data);
            stream.Close();
    }

    public static PlayerSaveData LoadPLayer() { 
        string path = Application.persistentDataPath + "/saveFile.zss";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            PlayerSaveData data =  formatter.Deserialize(stream) as PlayerSaveData;
            stream.Close();
            return data;

        }
        else {
            return null;
        }
    }

    public static void FromStart(SceneControll sceneController) {
        string path = Application.persistentDataPath + "/saveFile.zss";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);
            PlayerSaveData data = new PlayerSaveData(sceneController);
            data.fromStart = true;
            formatter.Serialize(stream, data);
            stream.Close();
        }
    }
}
