using UnityEngine;
using UnityEngine.UI;

public class ABlock
{
    public RectTransform transform;

    Image block, icon, progressBar, cover, bar;
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
        bar.rectTransform.localScale =
            new Vector3((float)(target.Progress / target.Max), 1, 1);
        progressTxt.text = $"{target.Progress:#,0.#} / {target.Max}";

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
                iconSize  = new Vector2(100, 100),
                titleSize = new Vector2(800, 40),
                descSize  = new Vector2(800, 80),
                valueSize = new Vector2(190, 40),
                dateSize  = new Vector2(190, 80),
                pBarSize  = new Vector2(1020, 40);

        Color   cBlock    = new Color(0.75f, 0.72f, 0.8f),
                cBlockBar = new Color(0.9f, 0.75f, 0),
                cPBar     = new Color(0.4f, 0.4f, 0.4f),
                cStory    = Color.white,
                cCP       = new Color(1, 0, 0.3334f),
                cBPS      = new Color(0, 0.5f, 1),
                cResearch = new Color(0, 0.75f, 0.3f),
                cSecret   = new Color(0.45f, 0.35f, 1),
                cCover    = new Color(0, 0, 0, 0.3f);

        GameObject iconObj, valueObj, progressBarObj, titleObj;


        transform = InterfaceTool.ImgSetup(
            $"{target.Title} Block", parent, out block,
            SysManager.defaultBox, false)
            .GetComponent<RectTransform>();
        block.color = cBlock;
        InterfaceTool.FormatRectNPos(block.rectTransform,
            blockSize, Vector2.up, Vector2.one,
            new Vector2(0.5f, 1));

        iconObj = InterfaceTool.ImgSetup("Icon", transform,
            out Image icon, false);
        InterfaceTool.FormatRect(icon.rectTransform,
            iconSize, Vector2.up, Vector2.up, Vector2.up,
            new Vector2(40, -40));

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
            iconObj.transform, out title, false);
        InterfaceTool.FormatRect(title.rectTransform,
            titleSize, Vector2.one, Vector2.one, Vector2.up,
            new Vector2(30, 0));
        InterfaceTool.FormatText(title,
            SysManager.DEFAULT_FONT, 32, Color.white,
            TextAnchor.UpperLeft, FontStyle.Bold);
        title.text = target.Title;

        InterfaceTool.TextSetup("Desc", titleObj.transform,
            out Text desc, false);
        InterfaceTool.FormatRectNPos(desc.rectTransform,
            descSize, Vector2.zero, Vector2.zero, Vector2.up);
        InterfaceTool.FormatText(desc, SysManager.DEFAULT_FONT,
            24, Color.white, TextAnchor.UpperLeft,
            FontStyle.Normal);
        desc.text = target.Desc;

        valueObj = InterfaceTool.TextSetup("Value",
            transform, out value, false);
        InterfaceTool.FormatRect(value.rectTransform,
            valueSize, Vector2.one, Vector2.one, Vector2.one,
            new Vector2(-15, -15));
        InterfaceTool.FormatText(value,
            SysManager.DEFAULT_FONT, 32, Color.white,
            TextAnchor.UpperRight, FontStyle.Italic);
        value.text = $"{target.BitAmount}";

        InterfaceTool.TextSetup("Date", valueObj.transform,
            out date, false);
        InterfaceTool.FormatRectNPos(
            date.rectTransform, dateSize,
            Vector2.right, Vector2.right, Vector2.one);
        InterfaceTool.FormatText(date,
            SysManager.DEFAULT_FONT, 28, Color.white,
            TextAnchor.UpperRight, FontStyle.Normal);
        date.text = "";

        progressBarObj = InterfaceTool.ImgSetup("Progress Bar",
            transform, out progressBar, false);
        InterfaceTool.FormatRect(
            progressBar.rectTransform,
            pBarSize, Vector2.right, Vector2.right,
            Vector2.right, new Vector2(-15, 10));
        progressBar.color = cPBar;

        InterfaceTool.ImgSetup("Bar", progressBarObj.transform,
            out bar, false);
        InterfaceTool.FormatRectNPos(
            bar.rectTransform,
            new Vector2(progressBar.rectTransform
                .sizeDelta.x, 0), Vector2.zero, Vector2.up,
            new Vector2(0, 0.5f));
        bar.color = cBlockBar;
        bar.rectTransform.localScale =
            new Vector2(target.Status == 0 ?
            1 : (float)(target.Progress / target.Max), 1);

        InterfaceTool.TextSetup("Value",
            progressBarObj.transform,
            out progressTxt, false);
        InterfaceTool.FormatRect(progressTxt
            .rectTransform);
        InterfaceTool.FormatText(progressTxt,
            SysManager.DEFAULT_FONT, 32, Color.black,
            TextAnchor.MiddleCenter, FontStyle.Normal);
        progressTxt.text =
            BitNotation.ToBitNotation(target.Progress) +
            $"/ {target.Max}";

        InterfaceTool.ImgSetup("Cover", transform, out cover, true);
        InterfaceTool.FormatRect(cover.rectTransform);
        cover.color = cCover;
        cover.enabled = target.Status != 0;
    }
}
