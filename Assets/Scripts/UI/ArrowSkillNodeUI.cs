using System;
using UnityEngine;

public enum ArrowSkillNodeState
{
    Locked,
    Purchasable,
    Unlocked,
    Selected
}

public class ArrowSkillNodeUI : MonoBehaviour
{
    #region Fields & Properties
    [field: SerializeField] public string NodeId { get; private set; }
    [field: SerializeField] public UnityEngine.UI.Button NodeButton { get; private set; }
    [field: SerializeField] public UnityEngine.UI.Image BackgroundImage { get; private set; }
    [field: SerializeField] public UnityEngine.UI.Text LabelText { get; private set; }

    public RectTransform RectTransform => transform as RectTransform;

    public event Action<string> OnNodeClicked;

    private static readonly Color LockedColor = new Color(0.32f, 0.34f, 0.38f, 1f);
    private static readonly Color PurchasableColor = new Color(0.85f, 0.68f, 0.18f, 1f);
    private static readonly Color UnlockedColor = new Color(0.22f, 0.56f, 0.85f, 1f);
    private static readonly Color SelectedColor = new Color(0.18f, 0.72f, 0.40f, 1f);

    private const string EquippedLabel = "[SELECTED]";
    private const string OwnedLabel = "[OWNED]\nClick to Select";
    private const string BuyFormat = "Buy: {0}G";
    private const string LockedFormat = "Locked\nNeed: {0}";
    #endregion

    #region Unity Lifecycle
    private void OnEnable()
    {
        if (NodeButton != null)
        {
            NodeButton.onClick.AddListener(HandleClick);
        }
    }

    private void OnDisable()
    {
        if (NodeButton != null)
        {
            NodeButton.onClick.RemoveListener(HandleClick);
        }
    }
    #endregion

    #region Public Methods
    public void Initialize(
        string nodeId,
        UnityEngine.UI.Button button,
        UnityEngine.UI.Image background,
        UnityEngine.UI.Text label)
    {
        if (NodeButton != null)
        {
            NodeButton.onClick.RemoveListener(HandleClick);
        }

        NodeId = nodeId;
        NodeButton = button;
        BackgroundImage = background;
        LabelText = label;

        if (Application.isPlaying && isActiveAndEnabled && NodeButton != null)
        {
            NodeButton.onClick.AddListener(HandleClick);
        }
    }

    public void RefreshView(string displayName, int cost, string parentDisplayName, ArrowSkillNodeState state)
    {
        if (BackgroundImage != null)
        {
            BackgroundImage.color = GetStateColor(state);
        }

        if (NodeButton != null)
        {
            NodeButton.interactable = state != ArrowSkillNodeState.Locked;
        }

        if (LabelText != null)
        {
            string statusText = GetStatusText(cost, parentDisplayName, state);
            LabelText.text = $"{displayName}\n{statusText}";
        }
    }
    #endregion

    #region Private Methods
    private void HandleClick()
    {
        OnNodeClicked?.Invoke(NodeId);
    }

    private static Color GetStateColor(ArrowSkillNodeState state)
    {
        return state switch
        {
            ArrowSkillNodeState.Selected => SelectedColor,
            ArrowSkillNodeState.Unlocked => UnlockedColor,
            ArrowSkillNodeState.Purchasable => PurchasableColor,
            _ => LockedColor
        };
    }

    private static string GetStatusText(int cost, string parentDisplayName, ArrowSkillNodeState state)
    {
        return state switch
        {
            ArrowSkillNodeState.Selected => EquippedLabel,
            ArrowSkillNodeState.Unlocked => OwnedLabel,
            ArrowSkillNodeState.Purchasable => string.Format(BuyFormat, cost),
            _ => string.Format(LockedFormat, parentDisplayName)
        };
    }
    #endregion
}
