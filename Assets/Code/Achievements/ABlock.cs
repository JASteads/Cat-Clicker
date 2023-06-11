using UnityEngine;
using UnityEngine.UI;

public class ABlock
{
    public RectTransform transform;

    Image block, icon, pBarImg, cover, bar;
    Text value, title, desc, date, progressTxt;

    readonly AchievementInfo target;


    public ABlock(Transform parent, AchievementInfo target,
        int blockHeight)
    {
        this.target = target;

        GenerateBlock(parent, blockHeight);
    }

    public void UpdateBlock()
    {
        string newProgress =
            BitNotation.Format(target.Progress) + "/" +
            BitNotation.Format(target.Max);

        bar.rectTransform.localScale =
            new Vector3((float)(target.Progress / target.Max), 1, 1);
        progressTxt.text = newProgress;

        if (target.Status == AchievementStatus.UNLOCKED)
        {
            cover.gameObject.SetActive(false);
            date.text = SysManager.activeProfile.achievements.data
                .Find(a => a.name == target.Title)
                .dateEarned.ToString();
        }
        else
        {
            cover.gameObject.SetActive(true);
            date.text = "";
        }
    }

    void GenerateBlock(Transform parent, int blockHeight)
    {
        Vector2 blockSize = new Vector2(0, blockHeight),
                iconSize = new Vector2(100, 100),
                titleSize = new Vector2(800, 40),
                descSize = new Vector2(800, 80),
                valueSize = new Vector2(190, 40),
                dateSize = new Vector2(190, 80),
                pBarSize = new Vector2(1000, 40);

        Color cBlock = new Color(0.75f, 0.72f, 0.8f),
                cBlockBar = new Color(0.9f, 0.75f, 0),
                cPBar = new Color(0.4f, 0.4f, 0.4f),
                cStory = Color.white,
                cCP = new Color(1, 0, 0.3334f),
                cBPS = new Color(0, 0.5f, 1),
                cResearch = new Color(0, 0.75f, 0.3f),
                cSecret = new Color(0.45f, 0.35f, 1),
                cCover = new Color(0, 0, 0, 0.3f);

        Transform iconObj, val, progressBar, titleObj;


        transform = InterfaceTool.ImgSetup(
            $"{target.Title} Block", parent, out block,
            SysManager.defaultBox, false)
            .GetComponent<RectTransform>();
        InterfaceTool.FormatRectNPos(block, blockSize,
            Vector2.up, Vector2.one, new Vector2(0.5f, 1));
        block.color = cBlock;

        iconObj = InterfaceTool.ImgSetup("Icon", transform,
            out Image icon, false);
        InterfaceTool.FormatRect(icon, iconSize, Vector2.up,
            Vector2.up, Vector2.up, new Vector2(40, -40));

        switch (target.Type)
        {
            case AchievementType.STORY:
                icon.color = cStory; break;
            case AchievementType.CP:
                icon.color = cCP; break;
            case AchievementType.BPS:
                icon.color = cBPS; break;
            case AchievementType.RESEARCH:
                icon.color = cResearch; break;
            case AchievementType.SECRET:
                icon.color = cSecret; break;
            default:
                Debug.Log("Invalid bit color assignment"); break;
        }

        titleObj = InterfaceTool.TextSetup("Title",
            iconObj, out title, false);
        InterfaceTool.FormatRect(title, titleSize, Vector2.one,
            Vector2.one, Vector2.up, new Vector2(30, 0));
        InterfaceTool.FormatText(title, SysManager.DEFAULT_FONT, 32,
            Color.white, TextAnchor.UpperLeft, FontStyle.Bold);
        title.text = target.Title;
        Text desc = InterfaceTool.CreateFooter(target.Desc, titleObj,
            55, Vector2.zero, 24);
        desc.alignment = TextAnchor.UpperLeft;

        progressBar = InterfaceTool.ImgSetup("Progress Bar",
            desc.transform, out pBarImg, false);
        InterfaceTool.FormatRectNPos(pBarImg, pBarSize, Vector2.zero,
            Vector2.zero, Vector2.up);
        pBarImg.color = cPBar;

        InterfaceTool.ImgSetup("Bar", progressBar, out bar, false);
        InterfaceTool.FormatRect(bar);
        bar.color = cBlockBar;
        progressTxt = InterfaceTool.CreateBody("", progressBar, 32);

        val = InterfaceTool.TextSetup("Value", transform,
            out value, false);
        InterfaceTool.FormatRect(value, valueSize, Vector2.one,
            Vector2.one, Vector2.one, new Vector2(-30, -20));
        InterfaceTool.FormatText(value, SysManager.DEFAULT_FONT, 32,
            Color.white, TextAnchor.UpperRight, FontStyle.Italic);
        value.text = $"{target.BitAmount}";
        date = InterfaceTool.CreateFooter("", val, 75,
            Vector2.zero, 28);
        date.alignment = TextAnchor.UpperRight;

        InterfaceTool.ImgSetup("Cover", transform, out cover, true);
        InterfaceTool.FormatRect(cover);
        cover.color = cCover;
        cover.enabled = target.Status != 0;
    }
}
