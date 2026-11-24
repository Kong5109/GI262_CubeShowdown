using System;
using UnityEngine;

public enum BowShopItemState
{
    Purchasable,
    Owned,
    Equipped
}

public class BowShopItemUI : MonoBehaviour
{
    #region Fields & Properties
    [field: SerializeField] public string BowId { get; private set; }
    [field: SerializeField] public UnityEngine.UI.Image IconImage { get; private set; }
    [field: SerializeField] public UnityEngine.UI.Text NameText { get; private set; }
    [field: SerializeField] public UnityEngine.UI.Text DetailText { get; private set; }
    [field: SerializeField] public UnityEngine.UI.Button ActionButton { get; private set; }
    [field: SerializeField] public UnityEngine.UI.Image ActionButtonBackground { get; private set; }
    [field: SerializeField] public UnityEngine.UI.Text ActionButtonLabel { get; private set; }

    public event Action<string> OnActionClicked;

    private static readonly Color BuyAffordableColor = new Color(0.40f, 0.66f, 0.00f, 1f);
    private static readonly Color BuyUnaffordableColor = new Color(0.42f, 0.50f, 0.34f, 1f);
    private static readonly Color OwnedColor = new Color(0.18f, 0.49f, 0.76f, 1f);
    private static readonly Color EquippedColor = new Color(0.15f, 0.60f, 0.36f, 1f);
    private static readonly Color DefaultIconTint = new Color(0.96f, 0.96f, 0.97f, 1f);

    private const int FreeCost = 0;
    private const string NameFormat = "Name: {0}";
    private const string BuyFormat = "Buy ({0}G)";
    private const string FreeBuyLabel = "Buy (Free)";
    private const string EquipLabel = "Equip";
    private const string EquippedLabel = "Equipped";
    private const string CostDetailFormat = "Price: {0}G";
    private const string FreeDetailLabel = "Starter Bow";
    private const string OwnedDetailLabel = "Owned - Click Equip to use";
    private const string EquippedDetailLabel = "Currently Equipped";
    #endregion

    #region Unity Lifecycle
    private void OnEnable()
    {
        if (ActionButton != null)
        {
            ActionButton.onClick.AddListener(HandleActionClick);
        }
    }

    private void OnDisable()
    {
        if (ActionButton != null)
        {
            ActionButton.onClick.RemoveListener(HandleActionClick);
        }
    }
    #endregion

    #region Public Methods
    public void Initialize(
        string bowId,
        UnityEngine.UI.Image iconImage,
        UnityEngine.UI.Text nameText,
        UnityEngine.UI.Text detailText,
        UnityEngine.UI.Button actionButton,
        UnityEngine.UI.Image actionButtonBackground,
        UnityEngine.UI.Text actionButtonLabel)
    {
        if (ActionButton != null)
        {
            ActionButton.onClick.RemoveListener(HandleActionClick);
        }

        BowId = bowId;
        IconImage = iconImage;
        NameText = nameText;
        DetailText = detailText;
        ActionButton = actionButton;
        ActionButtonBackground = actionButtonBackground;
        ActionButtonLabel = actionButtonLabel;

        if (Application.isPlaying && isActiveAndEnabled && ActionButton != null)
        {
            ActionButton.onClick.AddListener(HandleActionClick);
        }
    }

    public void RefreshView(string displayName, int cost, Sprite icon, BowShopItemState state, bool canAfford)
    {
        if (IconImage != null)
        {
            IconImage.sprite = icon;
            IconImage.preserveAspect = icon != null;
            IconImage.color = icon != null ? Color.white : DefaultIconTint;
        }

        if (NameText != null)
        {
            NameText.text = string.Format(NameFormat, displayName);
        }

        if (DetailText != null)
        {
            DetailText.text = GetDetailText(cost, state);
        }

        if (ActionButtonBackground != null)
        {
            ActionButtonBackground.color = GetButtonColor(state, canAfford);
        }

        if (ActionButtonLabel != null)
        {
            ActionButtonLabel.text = GetButtonLabel(cost, state);
        }

        if (ActionButton != null)
        {
            ActionButton.interactable = state != BowShopItemState.Equipped;
        }
    }
    #endregion

    #region Private Methods
    private void HandleActionClick()
    {
        OnActionClicked?.Invoke(BowId);
    }

    private static Color GetButtonColor(BowShopItemState state, bool canAfford)
    {
        return state switch
        {
            BowShopItemState.Equipped => EquippedColor,
            BowShopItemState.Owned => OwnedColor,
            _ => canAfford ? BuyAffordableColor : BuyUnaffordableColor
        };
    }

    private static string GetButtonLabel(int cost, BowShopItemState state)
    {
        return state switch
        {
            BowShopItemState.Equipped => EquippedLabel,
            BowShopItemState.Owned => EquipLabel,
            _ => cost <= FreeCost ? FreeBuyLabel : string.Format(BuyFormat, cost)
        };
    }

    private static string GetDetailText(int cost, BowShopItemState state)
    {
        return state switch
        {
            BowShopItemState.Equipped => EquippedDetailLabel,
            BowShopItemState.Owned => OwnedDetailLabel,
            _ => cost <= FreeCost ? FreeDetailLabel : string.Format(CostDetailFormat, cost)
        };
    }
    #endregion
}
