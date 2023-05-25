using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static SysManager;

public class CLSCUpgrades : MonoBehaviour
{
    CLSCSystem system;

    GameObject upgradesShop, upgradesCanvas, shopToggle, upgradeList;
    public UpgradeType activeTabType;

    public CLSCUpgradeTab[] upgradeTabs = new CLSCUpgradeTab[4];

    List<UpgradeData> upgradesData = activeProfile.cl.upgradesData;
    List<UpgradeData> activeUpgrades = new List<UpgradeData>(); // Index for active_upgrades array
    List<CLSCUpgradeButton> upgradeButtons = new List<CLSCUpgradeButton>();
    CLSCStatusMessagesList upgradeMessages;

    

    public void UpdateSystem()
    {
        bool newUpgrades = false;

        // Check for new upgrades that are available
        activeProfile.cl.upgradesData.ForEach(upgrade =>
        {
            if (upgrade.Status == Status.LOCKED)
            {
                // If preconditions are met, unlock upgrade
                if (upgrade.CheckRequirement())
                {
                    Debug.Log($"{upgrade.Name} is unlocked.");

                    upgrade.Unlock();
                    upgradeMessages.Broadcast($"{upgrade.Name} is available in the Upgrade Shop!", StatusType.BONUS);
                    newUpgrades = true;
                }
                else
                {
                    // Debug.Log($"{upgrade.Name} could not be unlocked.");
                }
            }
        });

        if (newUpgrades)
            RefreshUpgrades();

        // Enable all upgrade buttons for upgrades that can be purchased
        activeUpgrades.ForEach(upgrade =>
        {
            // This button will be enabled or disabled if the upgrade can be purchased
            Button targetButton = upgradeButtons.Find(
                    button => button.data.Name == upgrade.Name).button;

            if (activeProfile.cl.GetCurrencyCurrent() >= upgrade.Price)
            {
                if (!targetButton.IsInteractable())
                {
                    targetButton.interactable = true;
                }
            }
            else if (targetButton.IsInteractable())
            {
                targetButton.interactable = false;
            }
        });
    }

    public void CreateUpgradesPanel(Transform parentTf, CLSCTooltip tooltip, CLSCSystem sys)
    {
        GameObject upgradeButtonTxtObj, scrollbar, title_box, 
            title_text_container, upgrades_container, tabs_container;
        RectTransform list_tf;
        const int BUTTON_HEIGHT = 150;

        system = sys;

        upgradesCanvas = InterfaceTool.CanvasSetup("Upgrades Canvas", transform, out Canvas canvas);
        InterfaceTool.FormatRect(upgradesCanvas.GetComponent<RectTransform>());

        upgradesShop = new GameObject("Upgrades");
        upgradesShop.AddComponent<RectTransform>();
        upgradesShop.transform.SetParent(upgradesCanvas.transform, false);
        InterfaceTool.FormatRect(upgradesShop.GetComponent<RectTransform>(), new Vector2(460, 560),
            new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 30));

        title_box = InterfaceTool.ImgSetup("Upgrades Title", upgradesShop.transform, out Image title_img, defaultBox, false);
        InterfaceTool.FormatRect(title_img.rectTransform, new Vector2(0, 120),
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f), new Vector2(0, 30));
        title_img.color = new Color(.8f, .8f, 1);

        title_text_container = InterfaceTool.TextSetup("Upgrade Title Text", title_box.transform, out Text title_text, false);
        InterfaceTool.FormatRect(title_text.rectTransform, new Vector2(-30, 90),
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(-15, 0));
        InterfaceTool.FormatText(title_text, DEFAULT_FONT, 42, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);
        title_text.text = "UPGRADES";

        upgrades_container = InterfaceTool.ImgSetup("Upgrades Container", upgradesShop.transform, out Image upgrades_container_img, defaultBox, true);
        upgrades_container.AddComponent<RectMask2D>();
        InterfaceTool.FormatRect(upgrades_container_img.rectTransform);
        upgrades_container_img.color = new Color(0.6f, 0.6f, 1);

        upgradeList = new GameObject("Upgrades List");
        upgradeList.transform.SetParent(upgrades_container.transform, false);
        list_tf = upgradeList.AddComponent<RectTransform>();
        InterfaceTool.FormatRect(list_tf, new Vector2(-30, 0),
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -15));

        shopToggle = InterfaceTool.ButtonSetup("Upgrades Button", upgradesCanvas.transform, out Image up_toggle_img, out Button upgradeShopButton, uiSprites[3],
            () => ToggleUpgradesShop());
        InterfaceTool.FormatRect(up_toggle_img.rectTransform, new Vector2(140, BUTTON_HEIGHT),
            new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-35, 25));

        upgradeButtonTxtObj = InterfaceTool.TextSetup("Upgrades Button Text", shopToggle.transform, out Text up_toggle_txt, false);
        InterfaceTool.FormatRect(up_toggle_txt.rectTransform);
        InterfaceTool.FormatText(up_toggle_txt, DEFAULT_FONT, 24, Color.black, TextAnchor.MiddleCenter, FontStyle.Bold);
        up_toggle_txt.text = "UPGRADES";

        tabs_container = new GameObject("Tabs Containter");
        tabs_container.transform.SetParent(upgradesShop.transform, false);
        tabs_container.AddComponent<RectTransform>();
        InterfaceTool.FormatRectNPos(tabs_container.GetComponent<RectTransform>(), new Vector2(50, 0),
            new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0.5f));

        scrollbar = InterfaceTool.ScrollbarSetup(upgradesShop.transform, upgrades_container, list_tf, 30);


        for (int i = 0; i < 4; i++)
        {
            upgradeTabs[i] = GenerateUpgradeTab((UpgradeType)i, new Vector2(0, 180 - (i * 120)));
        }
            
        activeTabType = UpgradeType.NONE;

        for (int i = 0; i < upgradesData.Count; i++)
        {
            Vector2 buttonPosition = new Vector2(0, -80 * i);

            upgradeButtons.Add(GenerateUpgradeButton(upgradesData[i], buttonPosition, tooltip));
        }

        upgradeMessages = gameObject.AddComponent<CLSCStatusMessagesList>();
        upgradeMessages.SetupList("Upgrade Messages", 32, upgradesCanvas.transform,
            new Vector2(540, 60), new Vector2(660, -290));

        ToggleUpgradesShop();
    }

    CLSCUpgradeButton GenerateUpgradeButton(UpgradeData targetUpgrade, Vector2 pos, CLSCTooltip tooltip)
    {
        GameObject upgrade, icon, title, price;
        CLSCUpgradeButton upgradeButton = new CLSCUpgradeButton
        {
            data = targetUpgrade
        };

        upgrade = InterfaceTool.ButtonSetup($"Upgrade {targetUpgrade.Name}", upgradeList.transform,
            out upgradeButton.buttonImage, out upgradeButton.button, uiSprites[3], () => system.PurchaseUpgrade(upgradeButton));

        InterfaceTool.FormatRect(upgradeButton.buttonImage.rectTransform, new Vector2(-30, 80),
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -15));
        upgradeButton.buttonImage.color = new Color(1, 1, .8f);
        upgradeButton.rectTf = upgradeButton.buttonImage.rectTransform;

        ColorBlock buttonColors = upgradeButton.button.colors;
        buttonColors.disabledColor = new Color(0.5f, 0.5f, 0.4f);
        buttonColors.highlightedColor = new Color(.7f, .7f, .7f);
        buttonColors.pressedColor = new Color(.4f, .4f, .4f);
        upgradeButton.button.colors = buttonColors;

        icon = InterfaceTool.ImgSetup("Upgrade Icon", upgradeButton.rectTf, out upgradeButton.icon, false);
        InterfaceTool.FormatRect(upgradeButton.icon.rectTransform, new Vector2(60, 60),
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(17, -10));
        upgradeButton.icon.color = new Color(0, .1f, 1);

        title = InterfaceTool.TextSetup("Upgrade Title", upgradeButton.rectTf, out upgradeButton.title, false);
        InterfaceTool.FormatRect(upgradeButton.title.rectTransform, new Vector2(340, 30),
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(90, -25));
        InterfaceTool.FormatText(upgradeButton.title, DEFAULT_FONT, 16, Color.black, TextAnchor.MiddleLeft, FontStyle.Bold);
        upgradeButton.title.text = targetUpgrade.Name;
        upgradeButton.title.alignByGeometry = true;
        upgradeButton.title.resizeTextForBestFit = true;
        upgradeButton.title.resizeTextMaxSize = 30;

        price = InterfaceTool.TextSetup("Upgrade Price", upgradeButton.rectTf, out upgradeButton.price, false);
        InterfaceTool.FormatRect(upgradeButton.price.rectTransform, new Vector2(250, 20),
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-20, -7.5f));
        InterfaceTool.FormatText(upgradeButton.price, DEFAULT_FONT, 16, Color.black, TextAnchor.MiddleRight, FontStyle.Italic);
        upgradeButton.price.text = $"$ {targetUpgrade.Price.ToString("0.##")}";
        upgradeButton.price.alignByGeometry = true;

        tooltip.AssignTooltip(upgrade, targetUpgrade.Name, DisplayFormat.UPGRADE);

        return upgradeButton;
    }

    CLSCUpgradeTab GenerateUpgradeTab(UpgradeType tabType, Vector2 pos)
    {
        GameObject tab, text;
        CLSCUpgradeTab upgradeTab = new CLSCUpgradeTab
        {
            type = tabType
        };

        tab = InterfaceTool.ButtonSetup($"{tabType} Tab", upgradesShop.transform.GetChild(2), out upgradeTab.image, out upgradeTab.button, uiSprites[3], () => RefreshUpgrades(tabType));
        upgradeTab.image.rectTransform.sizeDelta = new Vector2(50, 100);
        upgradeTab.image.rectTransform.anchoredPosition = pos;
        upgradeTab.image.color = Color.white;

        text = InterfaceTool.TextSetup("Tab Text", upgradeTab.button.transform, out upgradeTab.text, false);
        upgradeTab.text.rectTransform.sizeDelta = new Vector2(50, 150);
        InterfaceTool.FormatText(upgradeTab.text, DEFAULT_FONT, 16, Color.black, TextAnchor.MiddleCenter, FontStyle.Normal);
        upgradeTab.text.text = $"{(int)tabType + 1}";

        return upgradeTab;
    }

    CLSCUpgradeTab GetTab(UpgradeType tabToFind)
    {
        foreach (CLSCUpgradeTab tab in upgradeTabs)
        {
            if (tab.type == tabToFind)
                return tab;
        }

        return null;
    }

    public void RefreshUpgrades(UpgradeType nextTabType)
    {
        float buttonPos = 0;

        // Sets which tab to display
        
        // Find which tab is active
        CLSCUpgradeTab activeTab = GetTab(activeTabType);
        CLSCUpgradeTab nextTab = GetTab(nextTabType);

        if (activeTab != null)
        {
            activeTab.button.transition = Selectable.Transition.ColorTint;
            activeTab.image.color = Color.white;
        }
            
        if (activeTabType == nextTabType)
        {
            activeTabType = UpgradeType.NONE;
        }
        else
        {
            // If not, toggle on the new tab

            nextTab.button.transition = Selectable.Transition.None;
            nextTab.image.color = new Color(0.5f, 0.5f, 0.5f);

            activeTabType = nextTabType;
        }

        // Turn off all inactive upgrade buttons
        upgradeButtons.ForEach(button =>
        button.rectTf.gameObject.SetActive(false));

        activeUpgrades.Clear();
        
        // Display available upgrades
        for (int i = 0; i < upgradesData.Count; i++)
        {
            if ((activeTabType == UpgradeType.NONE || upgradesData[i].Type == activeTabType) 
                && upgradesData[i].Status == Status.UNLOCKED)
            {
                upgradeButtons[i].rectTf.gameObject.SetActive(true);
                upgradeButtons[i].rectTf.anchoredPosition = new Vector2(0, buttonPos - 15);
                buttonPos -= upgradeButtons[i].rectTf.rect.height;
                activeUpgrades.Add(upgradeButtons[i].data);
            }
        }

        // Resize list so it can be scrolled through
        upgradeList.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 80 * activeUpgrades.Count);
    }

    public void RefreshUpgrades()
    {
        float buttonPos = 0;
        
        // Turn off all inactive upgrade buttons
        upgradeButtons.ForEach(button =>
        button.rectTf.gameObject.SetActive(false));

        activeUpgrades.Clear();

        // Display available upgrades
        for (int i = 0; i < upgradesData.Count; i++)
        {
            if ((activeTabType == UpgradeType.NONE || upgradesData[i].Type == activeTabType)
                && upgradesData[i].Status == Status.UNLOCKED)
            {
                upgradeButtons[i].rectTf.gameObject.SetActive(true);
                upgradeButtons[i].rectTf.anchoredPosition = new Vector2(0, buttonPos - 15);
                buttonPos -= upgradeButtons[i].rectTf.rect.height;
                activeUpgrades.Add(upgradeButtons[i].data);
            }
        }

        // Resize list so it can be scrolled through
        upgradeList.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 80 * activeUpgrades.Count);
    }

    public void ToggleUpgradesShop()
    {
        // Use the tab section as an indicator to whether the upgrade shop is open or not.
        // Could be any of the tabs, but we can use them to access the parent object and turn off.
        GameObject tabs_section = upgradeTabs[0].button.transform.parent.gameObject;
        RectTransform shop_tf = upgradesShop.GetComponent<RectTransform>();
        Image toggleImg = shopToggle.GetComponent<Image>();
        Button toggleButton = shopToggle.GetComponent<Button>();

        if (!tabs_section.activeSelf)
        {
            shop_tf.pivot = new Vector2(1, 0.5f);
            shop_tf.anchoredPosition = new Vector2(-30, 30);
            tabs_section.SetActive(true);
            RefreshUpgrades();
            toggleImg.color = new Color(.5f, .5f, .5f);
            toggleButton.transition = Selectable.Transition.None;
        }
        else
        {
            shop_tf.pivot = new Vector2(0, 0.5f);
            shop_tf.anchoredPosition = new Vector2(0, 30);
            tabs_section.SetActive(false);
            toggleImg.color = Color.white;
            toggleButton.transition = Selectable.Transition.ColorTint;
        }
    }
}