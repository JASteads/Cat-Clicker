using System;
using System.Collections.Generic;

[Serializable]
public class AchieveData
{
    public List<AchieveDataBlock> data =
        new List<AchieveDataBlock>();

    public AchieveData(List<AchievementInfo> infoList)
    {
        infoList.ForEach((a) =>
        { data.Add(new AchieveDataBlock(a.Title)); });
    }

    public AchieveDataBlock Find(string name)
    {
        return data.Find(a => a.name == name);
    }
}