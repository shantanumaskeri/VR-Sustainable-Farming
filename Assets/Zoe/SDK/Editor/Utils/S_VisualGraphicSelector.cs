using SpatialStories;
using System;
using System.Collections.Generic;
using UnityEngine;

// -------------------------------------------
/* 
 * Tool that allow the developer to select a hand
 */
public class S_VisualGraphicSelector
{
    private class NavigationTypeVisualData
    {
        public string resourcesEnabledIconName;
        public Texture2D resourcesEnabledIconTexture2D;
        public string resourcesDisabledIconName;
        public Texture2D resourcesDisabledIconTexture2D;
        public string title;
    }

    private Dictionary<S_NavigationTypes, NavigationTypeVisualData> m_VisualDataByNavigationType = new Dictionary<S_NavigationTypes, NavigationTypeVisualData> {
        { S_NavigationTypes.TELEPORT, new NavigationTypeVisualData() {
            resourcesEnabledIconName = "UI/Navigation/NormalTeleportation_Enabled",
            resourcesDisabledIconName = "UI/Navigation/NormalTeleportation_Disabled",
            title = "Teleportation"
            }
        },
        // we chose not to have hotspot for now, uncomment the following lines
        // to re-activate it
        /*{ S_NavigationTypes.HOTSPOT, new NavigationTypeVisualData() {
            resourcesEnabledIconName = "UI/Teleporter/HotspotTeleportation_Enabled",
            resourcesDisabledIconName = "UI/Teleporter/HotspotTeleportation_Disabled",
            title = "Hotspot Teleportion"
            }
        },*/
        { S_NavigationTypes.JOYSTICK, new NavigationTypeVisualData() {
            resourcesEnabledIconName = "UI/Navigation/Joystick_Enabled",
            resourcesDisabledIconName = "UI/Navigation/Joystick_Disabled",
            title = "Joystick Navigation"
            }
        }
    };

    public S_VisualGraphicSelector()
    {
        LoadHandAssets();
    }

    // -------------------------------------------
    /* 
    * LoadHandAssets
    */
    public void LoadHandAssets()
    {
        foreach (KeyValuePair<S_NavigationTypes, NavigationTypeVisualData> visualDataForNavigationType in m_VisualDataByNavigationType)
        {
            NavigationTypeVisualData visualData = visualDataForNavigationType.Value;
            visualData.resourcesDisabledIconTexture2D = Resources.Load<Texture2D>(visualData.resourcesDisabledIconName);
            visualData.resourcesEnabledIconTexture2D = Resources.Load<Texture2D>(visualData.resourcesEnabledIconName);
        }
    }

    // -------------------------------------------
    /* 
     * DisplayHandsSelectionButtons
     */
    public void DisplayHandsSelectionButtons(S_InputManager _inputManager)
    {
        int xIni = 10;
        int yIni = 100;
        int sizeButton = 50;

        GUIStyle centeredTextStyle = new GUIStyle("label");
        centeredTextStyle.alignment = TextAnchor.MiddleCenter;

        S_NavigationTypes currentNavigationType = _inputManager.TeleportType;
        foreach (S_NavigationTypes navigationType in m_VisualDataByNavigationType.Keys)
        {
            if (GUI.Button(new Rect(xIni, yIni, sizeButton, sizeButton),
            currentNavigationType == navigationType ? m_VisualDataByNavigationType[navigationType].resourcesEnabledIconTexture2D :
                                                        m_VisualDataByNavigationType[navigationType].resourcesDisabledIconTexture2D))
            {
                _inputManager.TeleportType = navigationType;
            }

            xIni += sizeButton;
        }

        GUIStyle leftTextStyle = new GUIStyle("label");
        leftTextStyle.alignment = TextAnchor.MiddleLeft;
        GUI.Label(new Rect(xIni + 5, yIni, 4 * sizeButton, sizeButton), m_VisualDataByNavigationType[_inputManager.TeleportType].title, leftTextStyle);
    }
}