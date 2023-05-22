using UnityEngine;
using UnityEngine.UI;

public class InterfaceTool
{
    public static Font DEFAULT_FONT = SysManager.DEFAULT_FONT;

    public static void ToggleCanvasPriority(GameObject parent,
        Canvas priority)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GraphicRaycaster ray = parent.transform.GetChild(i)
                .GetComponent<Canvas>()
                .GetComponent<GraphicRaycaster>();
            ray.enabled = !ray.IsActive();
        }
        priority.GetComponent<GraphicRaycaster>().enabled = true;
    }

    public static GameObject CanvasSetup(string name,
        Transform parentTF, out Canvas newCanvas)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parentTF, false);

        newCanvas = obj.AddComponent<Canvas>();
        CanvasScaler scaler = obj.AddComponent<CanvasScaler>();
        GraphicRaycaster ray = obj.AddComponent<GraphicRaycaster>();
        RectMask2D mask = obj.AddComponent<RectMask2D>();

        newCanvas.worldCamera = Camera.main;
        newCanvas.renderMode = RenderMode.ScreenSpaceCamera;

        scaler.uiScaleMode = CanvasScaler
            .ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler
            .ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 1;
        scaler.referencePixelsPerUnit = 100;

        return obj;
    }
    public static GameObject ImgSetup(string objName,
        Transform parentTF, out Image img, bool raycasted)
    {
        GameObject imgObj = new GameObject(objName);
        imgObj.transform.SetParent(parentTF, false);
        img = imgObj.AddComponent<Image>();

        img.raycastTarget = raycasted;

        return imgObj;
    }
    public static GameObject ImgSetup(string objName,
        Transform parentTF, out Image img, Sprite sprite,
        bool raycasted)
    {
        GameObject imgObj = new GameObject(objName);
        imgObj.transform.SetParent(parentTF, false);
        img = imgObj.AddComponent<Image>();
        img.sprite = sprite;
        img.type = Image.Type.Sliced;
        img.pixelsPerUnitMultiplier = 15;

        img.raycastTarget = raycasted;

        return imgObj;
    }
    public static GameObject TextSetup(string objName,
        Transform parentTF, out Text txt, bool raycasted)
    {
        GameObject txtObj = new GameObject(objName);
        txtObj.transform.SetParent(parentTF, false);
        txt = txtObj.AddComponent<Text>();

        txt.raycastTarget = raycasted;

        return txtObj;
    }
    public static GameObject ButtonSetup(string objName,
        Transform parentTF, out Image img, out Button button,
        Sprite sprite, UnityEngine.Events.UnityAction call)
    {
        GameObject buttonObj = new GameObject(objName);
        buttonObj.transform.SetParent(parentTF, false);
        img = buttonObj.AddComponent<Image>();
        button = buttonObj.AddComponent<Button>();

        img.sprite = sprite;
        img.type = Image.Type.Sliced;
        img.pixelsPerUnitMultiplier = 15;
        if (call != null)
            button.onClick.AddListener(call);

        return buttonObj;
    }
    public static GameObject ScrollbarSetup(Transform parentTF,
        GameObject scrollObj, RectTransform contentTF, int width)
    {
        GameObject scrollbar = ImgSetup("Scrollbar", parentTF,
            out Image scrollImg, null, true);
        FormatRectNPos(scrollImg.rectTransform,
            new Vector2(width, 0), new Vector2(1, 0),
            Vector2.one, new Vector2(0, 0.5f));
        scrollImg.pixelsPerUnitMultiplier = 15;
        scrollImg.type = Image.Type.Sliced;
        Scrollbar scroll = scrollbar.AddComponent<Scrollbar>();
        scroll.direction = Scrollbar.Direction.BottomToTop;
        scrollImg.color = new Color(0.6f, 0.6f, 0.6f);

        GameObject scrollArea = new GameObject("Sliding Area");
        scrollArea.transform.SetParent(scrollbar.transform, false);
        FormatRectNPos(scrollArea.AddComponent<RectTransform>(),
            Vector2.zero, Vector2.zero, Vector2.one,
            new Vector2(0.5f, 0.5f));

        GameObject scrollHandle = ImgSetup("Handle",
            scrollArea.transform, out Image scrollHandleImg,
            null, true);
        FormatRectNPos(scrollHandleImg.rectTransform,
            Vector2.zero, new Vector2(0, 0.5f), new Vector2(1, 0.5f),
            new Vector2(0.5f, 0.5f));
        scroll.handleRect = scrollHandleImg.rectTransform;
        scrollHandleImg.pixelsPerUnitMultiplier = 15;
        scrollHandleImg.type = Image.Type.Sliced;

        ScrollRect scrollRect = scrollObj
            .AddComponent<ScrollRect>();
        scrollRect.content = contentTF;
        scrollRect.horizontal = false;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = 80;
        scrollRect.verticalScrollbar = scroll;

        return scrollbar;
    }
    public static GameObject ScrollbarSetup(Transform parentTF,
        GameObject scrollObj, RectTransform contentTF,
        Vector2 size, Vector2 aMin, Vector2 aMax,
        Vector2 pivot, Vector2 aPos)
    {
        GameObject scrollbar = ImgSetup("Scrollbar", parentTF,
            out Image scrollImg, null, true);
        FormatRect(scrollImg.rectTransform, size, aMin,
            aMax, pivot, aPos);
        scrollImg.pixelsPerUnitMultiplier = 15;
        scrollImg.type = Image.Type.Sliced;
        Scrollbar scroll = scrollbar.AddComponent<Scrollbar>();
        scroll.direction = Scrollbar.Direction.BottomToTop;
        scrollImg.color = new Color(0.6f, 0.6f, 0.6f);

        GameObject scrollArea = new GameObject("Sliding Area");
        scrollArea.transform.SetParent(scrollbar.transform, false);
        FormatRectNPos(scrollArea.AddComponent<RectTransform>(),
            Vector2.zero, Vector2.zero, Vector2.one,
            new Vector2(0.5f, 0.5f));

        GameObject scrollHandle = ImgSetup("Handle",
            scrollArea.transform, out Image scrollHandleImg,
            null, true);
        FormatRectNPos(scrollHandleImg.rectTransform,
            Vector2.zero, new Vector2(0, 0.5f),
            new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f));
        scroll.handleRect = scrollHandleImg.rectTransform;
        scrollHandleImg.pixelsPerUnitMultiplier = 15;
        scrollHandleImg.type = Image.Type.Sliced;

        ScrollRect scrollRect = scrollObj.AddComponent<ScrollRect>();
        scrollRect.content = contentTF;
        scrollRect.horizontal = false;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = 80;
        scrollRect.verticalScrollbar = scroll;

        return scrollbar;
    }

    public static Text CreateHeader(string text, Transform parent,
        Vector2 size, Vector2 offset, int fontSize)
    {
        GameObject headerObj = TextSetup(
            "Header", parent,
            out Text header, false);
        FormatRect(header.rectTransform,
            size, new Vector2(0, 1),
            Vector2.one, new Vector2(),
            offset);
        FormatText(header, DEFAULT_FONT, fontSize,
            Color.white, TextAnchor.UpperLeft, FontStyle.Normal);
        header.text = text;

        return header;
    }
    public static Text CreateBody(string text, Transform parent,
        int fontSize)
    {
        GameObject bodyObj = TextSetup("Body", parent,
            out Text body, false);
        FormatRect(body.rectTransform);
        FormatText(body, DEFAULT_FONT,
            fontSize, Color.black, TextAnchor.MiddleCenter,
            FontStyle.Normal);
        body.text = text;

        return body;
    }
    public static void FormatText(Text txt, Font font, int fontSize,
        Color color, TextAnchor alignment, FontStyle style)
    {
        txt.font = font;
        txt.color = color;
        txt.fontSize = fontSize;
        txt.alignment = alignment;
        txt.fontStyle = style;
    }

    public static void FormatRectNPos(RectTransform tf, Vector2 size,
        Vector2 aMin, Vector2 aMax, Vector2 pivot)
    {
        tf.sizeDelta = size;
        tf.anchorMin = aMin;
        tf.anchorMax = aMax;
        tf.pivot = pivot;
    }
    public static void FormatRectNPos(RectTransform tf, Vector2 size)
    { tf.sizeDelta = size; }

    // For simple text objects : Stretch to fill parent area
    public static void FormatRect(RectTransform tf)
    {
        tf.sizeDelta = new Vector2();
        tf.anchorMin = new Vector2();
        tf.anchorMax = Vector2.one;
    }
    public static void FormatRect(RectTransform tf, Vector2 size,
        Vector2 aPos)
    {
        tf.sizeDelta = size;
        tf.anchoredPosition = aPos;
    }
    public static void FormatRect(RectTransform tf, Vector2 size,
        Vector2 aMin, Vector2 aMax, Vector2 aPos)
    {
        tf.sizeDelta = size;
        tf.anchorMin = aMin;
        tf.anchorMax = aMax;
        tf.anchoredPosition = aPos;
    }
    public static void FormatRect(RectTransform tf, Vector2 size,
        Vector2 aMin, Vector2 aMax, Vector2 pivot, Vector2 aPos)
    {
        tf.sizeDelta = size;
        tf.anchorMin = aMin;
        tf.anchorMax = aMax;
        tf.pivot = pivot;
        tf.anchoredPosition = aPos;
    }

    // NOT WORKING PROPERLY
    public static void FormatFillRect(RectTransform tf,
        float top, float bottom, float left, float right)
    {
        RectTransform parentRectTF = tf.parent
            .GetComponent<RectTransform>();

        if (parentRectTF == null) return;

        tf.pivot = new Vector2(0.5f, 0);
        tf.anchorMin = new Vector2();
        tf.anchorMax = Vector2.one;
        tf.anchoredPosition = new Vector2(
            (left - right) * 0.5f, bottom);
        tf.sizeDelta = new Vector2(-(left + right),
            -(parentRectTF.sizeDelta.y - (bottom - top)));
    }

    public static void FormatStretchW(RectTransform tf, float left,
        float right, float yPos, float height, Vector2 pivot)
    {
        tf.sizeDelta = new Vector2(-(left + right), -height);
        tf.anchorMin = new Vector2();
        tf.anchorMax = Vector2.one;
        tf.pivot = new Vector2(0.5f, 0);
        tf.anchoredPosition = new Vector2(
            (left - right) * 0.5f, yPos);
    }
}