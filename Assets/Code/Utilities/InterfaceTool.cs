using UnityEngine;
using UnityEngine.UI;

public class InterfaceTool
{
    public static Font DEFAULT_FONT = SysManager.DEFAULT_FONT;
    

    public static void ToggleCanvasPriority(Transform parent,
        Canvas priority)
    {
        GraphicRaycaster[] rays = 
            parent.GetComponentsInChildren<GraphicRaycaster>();

        for (int i = 0; i < rays.Length; i++)
            rays[i].enabled = !rays[i].IsActive();

        priority.GetComponent<GraphicRaycaster>().enabled = true;
    }

    public static Transform CanvasSetup(string name,
        Transform parent, out Canvas newCanvas)
    {
        Vector2 REF_RESOLUTION = new Vector2(1920, 1080);

        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        newCanvas = obj.AddComponent<Canvas>();
        newCanvas.worldCamera = SysManager.mainCam;
        newCanvas.renderMode = RenderMode.ScreenSpaceCamera;

        if (parent == null || parent.GetComponent<Canvas>() == null)
        {
            CanvasScaler scaler = obj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler
            .ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = REF_RESOLUTION;
            scaler.screenMatchMode = CanvasScaler
                .ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 1;
            scaler.referencePixelsPerUnit = 100;
        }
        
        obj.AddComponent<GraphicRaycaster>();
        obj.AddComponent<RectMask2D>();

        return obj.transform;
    }
    public static Transform ImgSetup(string objName,
        Transform parent, out Image img, bool raycasted)
    {
        GameObject obj = new GameObject(objName);
        obj.transform.SetParent(parent, false);
        img = obj.AddComponent<Image>();

        img.raycastTarget = raycasted;

        return obj.transform;
    }
    public static Transform ImgSetup(string objName,
        Transform parent, out Image img, Sprite sprite,
        bool raycasted)
    {
        GameObject obj = new GameObject(objName);
        obj.transform.SetParent(parent, false);
        img = obj.AddComponent<Image>();
        img.sprite = sprite;
        img.type = Image.Type.Sliced;
        img.pixelsPerUnitMultiplier = 15;

        img.raycastTarget = raycasted;

        return obj.transform;
    }
    public static Transform TextSetup(string objName,
        Transform parent, out Text txt, bool raycasted)
    {
        GameObject obj = new GameObject(objName);
        obj.transform.SetParent(parent, false);
        txt = obj.AddComponent<Text>();

        txt.raycastTarget = raycasted;

        return obj.transform;
    }
    public static Transform ButtonSetup(string objName,
        Transform parent, out Image img, out Button button,
        Sprite sprite, UnityEngine.Events.UnityAction call)
    {   
        GameObject obj = new GameObject(objName);
        obj.transform.SetParent(parent, false);
        img = obj.AddComponent<Image>();
        button = obj.AddComponent<Button>();

        img.sprite = sprite;
        img.type = Image.Type.Sliced;
        img.pixelsPerUnitMultiplier = 15;
        if (call != null)
            button.onClick.AddListener(call);

        return obj.transform;
    }
    public static Transform ScrollbarSetup(Transform parent,
        GameObject scrollObj, RectTransform contentTF, int width)
    {
        GameObject obj = ImgSetup("Scrollbar", parent,
            out Image scrollImg, null, true).gameObject;
        FormatRectNPos(scrollImg, new Vector2(width, 0),
            new Vector2(1, 0), Vector2.one, new Vector2(0, 0.5f));
        scrollImg.pixelsPerUnitMultiplier = 15;
        scrollImg.type = Image.Type.Sliced;
        Scrollbar scroll = obj.AddComponent<Scrollbar>();
        scroll.direction = Scrollbar.Direction.BottomToTop;
        scrollImg.color = new Color(0.6f, 0.6f, 0.6f);

        GameObject scrollArea = new GameObject("Sliding Area");
        scrollArea.transform.SetParent(obj.transform, false);
        RectTransform areaTF = scrollArea
            .AddComponent<RectTransform>();
        areaTF.sizeDelta = Vector2.zero;
        areaTF.anchorMin = Vector2.zero;
        areaTF.anchorMax = Vector2.one;
        areaTF.pivot = new Vector2(0.5f, 0.5f);

        ImgSetup("Handle", scrollArea.transform,
            out Image scrollHandleImg, null, true);
        FormatRectNPos(scrollHandleImg, Vector2.zero,
            new Vector2(0, 0.5f), new Vector2(1, 0.5f),
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

        return obj.transform;
    }
    public static Transform ScrollbarSetup(Transform parent,
        GameObject scrollObj, RectTransform contentTF,
        Vector2 size, Vector2 aMin, Vector2 aMax,
        Vector2 pivot, Vector2 aPos)
    {
        GameObject obj = ImgSetup("Scrollbar", parent,
            out Image scrollImg, null, true).gameObject;
        FormatRect(scrollImg, size, aMin,
            aMax, pivot, aPos);
        scrollImg.pixelsPerUnitMultiplier = 15;
        scrollImg.type = Image.Type.Sliced;
        Scrollbar scroll = obj.AddComponent<Scrollbar>();
        scroll.direction = Scrollbar.Direction.BottomToTop;
        scrollImg.color = new Color(0.6f, 0.6f, 0.6f);

        GameObject scrollArea = new GameObject("Sliding Area");
        scrollArea.transform.SetParent(obj.transform, false);
        RectTransform areaTF = scrollArea
            .AddComponent<RectTransform>();
        areaTF.sizeDelta = Vector2.zero;
        areaTF.anchorMin = Vector2.zero;
        areaTF.anchorMax = Vector2.one;
        areaTF.pivot = new Vector2(0.5f, 0.5f);

        ImgSetup("Handle", scrollArea.transform,
            out Image scrollHandleImg, null, true);
        FormatRectNPos(scrollHandleImg, Vector2.zero, 
            new Vector2(0, 0.5f), new Vector2(1, 0.5f),
            new Vector2(0.5f, 0.5f));
        scroll.handleRect = scrollHandleImg.rectTransform;
        scrollHandleImg.pixelsPerUnitMultiplier = 15;
        scrollHandleImg.type = Image.Type.Sliced;

        ScrollRect scrollRect = scrollObj.AddComponent<ScrollRect>();
        scrollRect.content = contentTF;
        scrollRect.horizontal = false;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = 80;
        scrollRect.verticalScrollbar = scroll;

        return obj.transform;
    }

    public static Text CreateHeader(string text, Transform parent,
         int height, Vector2 offset, int fontSize)
    {
        TextSetup("Header", parent, out Text header, false);
        FormatRect(header, new Vector2(-offset.x, height),
            Vector2.up, Vector2.one, Vector2.zero, offset);
        FormatText(header, DEFAULT_FONT, fontSize, Color.white,
            TextAnchor.MiddleLeft, FontStyle.Normal);
        header.text = text;

        return header;
    }
    public static Text CreateBody(string text, Transform parent,
        int fontSize)
    {
        TextSetup("Body", parent, out Text body, false);
        FormatRect(body);
        FormatText(body, DEFAULT_FONT, fontSize,
            Color.black, TextAnchor.MiddleCenter, FontStyle.Normal);
        body.text = text;

        return body;
    }
    public static Text CreateFooter(string text, Transform parent,
         int height, Vector2 offset, int fontSize)
    {
        TextSetup("Footer", parent, out Text header, false);
        FormatRect(header, new Vector2(0, height),
            Vector2.zero, Vector2.right, Vector2.up, offset);
        FormatText(header, DEFAULT_FONT, fontSize, Color.white,
            TextAnchor.MiddleLeft, FontStyle.Normal);
        header.text = text;

        return header;
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
    public static void FormatRectNPos(Graphic graphic, Vector2 size,
        Vector2 aMin, Vector2 aMax, Vector2 pivot)
    {
        RectTransform tf = graphic.rectTransform;

        tf.sizeDelta = size;
        tf.anchorMin = aMin;
        tf.anchorMax = aMax;
        tf.pivot = pivot;
    }
    public static void FormatRectNPos(RectTransform tf, Vector2 size)
    { tf.sizeDelta = size; }
    public static void FormatRectNPos(Graphic graphic, Vector2 size)
    { graphic.rectTransform.sizeDelta = size; }

    // For simple text objects : Stretch to fill parent area
    public static void FormatRect(RectTransform tf)
    {
        tf.sizeDelta = new Vector2();
        tf.anchorMin = new Vector2();
        tf.anchorMax = Vector2.one;
    }
    public static void FormatRect(Graphic graphic)
    {
        RectTransform tf = graphic.rectTransform;

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
    public static void FormatRect(Graphic graphic, Vector2 size,
        Vector2 aPos)
    {
        RectTransform tf = graphic.rectTransform;

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
    public static void FormatRect(Graphic graphic, Vector2 size,
        Vector2 aMin, Vector2 aMax, Vector2 aPos)
    {
        RectTransform tf = graphic.rectTransform;

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
    public static void FormatRect(Graphic graphic, Vector2 size,
        Vector2 aMin, Vector2 aMax, Vector2 pivot, Vector2 aPos)
    {
        RectTransform tf = graphic.rectTransform;

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