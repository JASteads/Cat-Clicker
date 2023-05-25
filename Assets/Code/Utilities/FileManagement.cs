using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class FileManagement
{
    readonly BinaryFormatter formatter = new BinaryFormatter();
    readonly string saveDirectory = 
        $"{Directory.GetCurrentDirectory()}/Saves/";

    public FileManagement()
    {
        if (!Directory.Exists(saveDirectory))
        {
            UnityEngine.Debug.Log("Saves directory doesn't exist!" +
                " Creating new one ..");
            Directory.CreateDirectory(saveDirectory);
        }
    }

    public void NewFile(int slot)
    {
        SysManager.activeProfile = new Profile(slot);
        FileSave();
    }
    public void FileSave()
    {
        FileStream file = File.OpenWrite(GetActiveProfile());
        formatter.Serialize(file, SysManager.activeProfile);
        file.Close();
    }
    public void FileLoad(int slot)
    {
        FileStream file = File.OpenRead(GetProfile(slot));
        SysManager.activeProfile = (Profile)formatter
            .Deserialize(file);
        file.Close();
    }
    public void DeleteFile()
    {
        if (File.Exists(GetActiveProfile()))
            File.Delete(GetActiveProfile());
    }

    string GetActiveProfile()
    {
        return $"{saveDirectory}File " +
            $"{(char)('A' + SysManager.activeProfile.Slot)}.nimmy";
    }
    string GetProfile(int slot)
    {
        return $"{saveDirectory}File " +
            $"{(char)('A' + slot)}.nimmy";
    }
}