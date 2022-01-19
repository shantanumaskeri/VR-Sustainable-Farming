using UnityEngine;

// -------------------------------------------
/* 
 * Tool that allow the developer to select a hand
 */
public class S_VisualHandSelector
{
    protected Texture2D m_logoEitherHandIdle;
    protected Texture2D m_logoEitherHandPressed;

    protected Texture2D m_logoLeftHandIdle;
    protected Texture2D m_logoLeftHandPressed;

    protected Texture2D m_logoRightHandIdle;
    protected Texture2D m_logoRightHandPressed;

    protected Texture2D m_logoBothHandsIdle;
    protected Texture2D m_logoBothHandsPressed;

    protected bool m_enableEitherHandsOption = true;
    protected bool m_enableBothHandsOption = true;

    enum Hand
    {
        None,
        Both,
        Either,
        Left,
        Right
    }

    //This works like a radio button - there can be only one choice
    private Hand m_EnableWhichHand = Hand.Either;

    public bool LogoEitherHandValue
    {
        get { return m_EnableWhichHand == Hand.Either; }
    }

    public bool LogoLeftHandValue
    {
        get { return m_EnableWhichHand == Hand.Left; }
    }

    public bool LogoRigthHandValue
    {
        get { return m_EnableWhichHand == Hand.Right; }
    }

    public bool LogoBothHandsValue
    {
        get { return m_EnableWhichHand == Hand.Both; }
    }

    public bool EnableEitherHandsOption
    {
        get { return m_enableEitherHandsOption; }
        set { m_enableEitherHandsOption = value; }
    }

    public bool EnableBothHandsOption
    {
        get { return m_enableBothHandsOption; }
        set { m_enableBothHandsOption = value; }
    }

    // -------------------------------------------
    /* 
     * Initialize
     */
    public void Initialize(bool _LogoEitherHandValue, bool _LogoLeftHandValue, bool _LogoRigthHandValue, bool _LogoBothHandsValue, bool _enableEitherHandsOption, bool _enableBothHandsOption)
    {
        if (_LogoEitherHandValue)
        {
            m_EnableWhichHand = Hand.Either;
        }
        else 
        if (_LogoLeftHandValue)
        {
            m_EnableWhichHand = Hand.Left;
        }
        else
        if (_LogoRigthHandValue)
        {
            m_EnableWhichHand = Hand.Right;
        }
        else
        if (_LogoBothHandsValue)
        {
            m_EnableWhichHand = Hand.Both;
        }

        m_enableEitherHandsOption = _enableEitherHandsOption;
        m_enableBothHandsOption = _enableBothHandsOption;
    }

    // -------------------------------------------
    /* 
    * LoadHandAssets
    */
    public void LoadHandAssets()
    {
        m_logoEitherHandIdle = Resources.Load<Texture2D>("UI/Touch/EitherHands_Idle");
        m_logoEitherHandPressed = Resources.Load<Texture2D>("UI/Touch/EitherHands_Pressed");

        m_logoLeftHandIdle = Resources.Load<Texture2D>("UI/Touch/LeftHand_Idle");
        m_logoLeftHandPressed = Resources.Load<Texture2D>("UI/Touch/LeftHand_Pressed");

        m_logoRightHandIdle = Resources.Load<Texture2D>("UI/Touch/RightHand_Idle");
        m_logoRightHandPressed = Resources.Load<Texture2D>("UI/Touch/RightHand_Pressed");

        m_logoBothHandsIdle = Resources.Load<Texture2D>("UI/Touch/BothHands_Idle");
        m_logoBothHandsPressed = Resources.Load<Texture2D>("UI/Touch/BothHands_Pressed");
    }

    // -------------------------------------------
    /* 
     * NoButtonSelected
     */
    protected bool NoButtonSelected()
    {
        return m_EnableWhichHand == Hand.None;
    }

    // -------------------------------------------
    /* 
     * DisplayHandsSelectionButtons
     */
    public void DisplayHandsSelectionButtons(int _HUDButtonHeight)
    {
        int xIni = 10;
        int yIni = _HUDButtonHeight;
        int sizeButton = 50;

        if (NoButtonSelected())
        {
            m_EnableWhichHand = Hand.Either;
        }

        GUIStyle centeredTextStyle = new GUIStyle("label");
        centeredTextStyle.alignment = TextAnchor.MiddleCenter;

        // EITHER HAND
        if (m_enableEitherHandsOption)
        {
            if (GUI.Button(new Rect(xIni, yIni, sizeButton, sizeButton), (m_EnableWhichHand != Hand.Either ? m_logoEitherHandIdle : m_logoEitherHandPressed)))
            {
                m_EnableWhichHand = Hand.Either;
            }
            GUI.Label(new Rect(xIni + 5, yIni + sizeButton, sizeButton - 10, sizeButton / 3), "Either", centeredTextStyle);
            xIni += sizeButton;
        }

        // LEFT HAND
        if (GUI.Button(new Rect(xIni, yIni, sizeButton, sizeButton), (m_EnableWhichHand != Hand.Left ? m_logoLeftHandIdle : m_logoLeftHandPressed)))
        {
            m_EnableWhichHand = Hand.Left;
        }

        GUI.Label(new Rect(xIni + 5, yIni + sizeButton, sizeButton - 10, sizeButton / 3), "Left", centeredTextStyle);
        xIni += sizeButton;

        // RIGHT HAND
        if (GUI.Button(new Rect(xIni, yIni, sizeButton, sizeButton), (m_EnableWhichHand != Hand.Right ? m_logoRightHandIdle : m_logoRightHandPressed)))
        {
            m_EnableWhichHand = Hand.Right;
        }

        GUI.Label(new Rect(xIni + 5, yIni + sizeButton, sizeButton - 10, sizeButton / 3), "Right", centeredTextStyle);
        xIni += sizeButton;

        // BOTH HANDS
        if (m_enableBothHandsOption)
        {
            if (GUI.Button(new Rect(xIni, yIni, sizeButton, sizeButton), (m_EnableWhichHand != Hand.Both ? m_logoBothHandsIdle : m_logoBothHandsPressed)))
            {
                m_EnableWhichHand = Hand.Both;
            }

            GUI.Label(new Rect(xIni + 5, yIni + sizeButton, sizeButton - 10, sizeButton / 3), "Both", centeredTextStyle);
            xIni += sizeButton;
        }
    }
}