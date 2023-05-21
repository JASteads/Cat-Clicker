using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class FileManagement
{
    readonly string saveDirectory = $"{ Directory.GetCurrentDirectory() }/Saves/";

    public void FileSave()
    {
        // File to save in
        FileStream file;

        // Encodes the file so it's hard to crack
        BinaryFormatter formatter = new BinaryFormatter();

        // Where to save the file. Profile slot # is used to make the file name correct
        string path = $"{ saveDirectory }File { (char)('A' + SysManager.profile.Slot)}.nimmy";
        
        // If file exists, overwrite it. Otherwise, create new file.
        if (File.Exists(path))
        {
            file = File.OpenWrite(path);
        }
        else
        {
            file = File.Create(path);
        }

        // Convert game data into a .nimmy file
        formatter.Serialize(file, SysManager.profile);

        file.Close();
    }

    public void FileLoad(int slot)
    {
        FileStream file;
        BinaryFormatter formatter = new BinaryFormatter();
        string path = $"{ saveDirectory }File { (char)('A' + SysManager.profile.Slot)}.nimmy";

        if (File.Exists(path))
        {
            file = File.OpenRead(path);
            SysManager.profile = (Profile)formatter.Deserialize(file);

            file.Close();
        }
        else
        {
            UnityEngine.Debug.Log("File could not be loaded ..");
        }
    }

    public void NewFile(int slot, Difficulty difficulty)
    {
        FileStream file;
        BinaryFormatter formatter = new BinaryFormatter();
        string path = $"{ saveDirectory }File { (char)('A' + SysManager.profile.Slot)}.nimmy";

        SysManager.profile = new Profile(slot, difficulty);
        
        if (File.Exists(path))
        {
            file = File.OpenWrite(path);
        }
        else
        {
            file = File.Create(path);
        }

        
        formatter.Serialize(file, SysManager.profile);

        file.Close();
    }

    public void DeleteFile()
    {
        string path = $"{ saveDirectory }File { (char)('A' + SysManager.profile.Slot)}.nimmy";

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
        
}