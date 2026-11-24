using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class BowShopUI : MonoBehaviour
{
    #region Fields & Properties
    [field: Header("References")]
    [field: SerializeField] public Player Player { get; private set; }

    [field: Header("Shop Settings")]
    [field: SerializeField] public bool OpenOnStart { get; private set; } = true;
    [field: SerializeField] public List<BowShopItemData> BowItems { get; private set; } = new List<BowShopItemData>();

    [Header("UI References")]
    [SerializeField] private GameObject _panelRoot;
    [SerializeField] private UnityEngine.UI.Text _goldText;
    [SerializeField] private UnityEngine.UI.Text _equippedText;
    [SerializeField] private UnityEngine.UI.Text _statusText;
    [SerializeField] private UnityEngine.UI.Button _closeButton;
    [SerializeField] private UnityEngine.UI.Button _openShopButton;
    [SerializeField] private List<BowShopItemUI> _sceneItemUIs = new List<BowShopItemUI>();

    public string EquippedBowId { get; private set; }
    public bool IsOpen => _panelRoot != null && _panelRoot.activeSelf;

    public event Action<BowShopItemData> OnBowPurchased;
    public event Action<BowShopItemData> OnBowEquipped;

    private const string EventSystemObjectName = "EventSystem";
    private const string GoldFormat = "Gold: {0}G";
    private const string EquippedFormat = "Equipped: {0}";
    private const string EmptySelectionPlaceholder = "-";
    private const string DefaultStatusHint = "Buy a bow with Gold, or click Equip to switch.";
    private const string NotEnoughGoldMessage = "Not enough Gold!";
    private const string PurchasedFormat = "Purchased & Equipped: {0}";
    private const string EquippedMessageFormat = "Equipped: {0}";

    private const string SingleShotId = "single_shot";
    private const string DoubleShotId = "double_shot";
    private const string SpreadShotId = "spread_shot";

    private const string SingleShotDisplayName = "SingleShot Bow";
    private const string DoubleShotDisplayName = "DoubleShot Bow";
    private const string SpreadShotDisplayName = "SpreadShot Bow";

    private const int SingleShotCost = 0;
    private const int DoubleShotCost = 100;
    private const int SpreadShotCost = 200;
    private const int DefaultGoldAmount = 0;

    private readonly Dictionary<string, BowShopItemData> _itemLookup = new Dictionary<string, BowShopItemData>();
    private readonly Dictionary<string, BowShopItemUI> _itemUIs = new Dictionary<string, BowShopItemUI>();
    private bool _isInitialized;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (Player == null)
        {
            Player = FindAnyObjectByType<Player>();
        }

        if (BowItems == null || BowItems.Count == 0)
        {
            PopulateDefaultBows();
        }

        InitializeItemStates();
        SyncInitialEquippedBow();
        BindSceneItemUIs();
        EnsureEventSystem();

        _isInitialized = true;
        RefreshAllItems();
        SetStatusMessage(DefaultStatusHint);
        SetShopVisible(OpenOnStart);
    }

    private void OnEnable()
    {
        SubscribeEvents();

        if (_isInitialized)
        {
            if (OpenOnStart)
            {
                SetShopVisible(true);
            }

            SyncEquippedFromWeaponHolder();
            RefreshAllItems();
        }
    }

    private void Reset()
    {
        PopulateDefaultBows();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }
    #endregion

    #region Public Methods
    [ContextMenu("Open Shop")]
    public void OpenShop()
    {
        SetShopVisible(true);
        SyncEquippedFromWeaponHolder();
        SetStatusMessage(DefaultStatusHint);
        RefreshAllItems();
    }

    [ContextMenu("Close Shop")]
    public void CloseShop()
    {
        SetShopVisible(false);
    }

    public void ToggleShop()
    {
        if (IsOpen)
        {
            CloseShop();
            return;
        }

        OpenShop();
    }

    public bool IsBowOwned(string bowId)
    {
        return !string.IsNullOrEmpty(bowId)
            && _itemLookup.TryGetValue(bowId, out BowShopItemData item)
            && item.IsOwned;
    }

    public bool TryBuyBow(string bowId)
    {
        if (string.IsNullOrEmpty(bowId) || !_itemLookup.TryGetValue(bowId, out BowShopItemData item) || item.IsOwned)
        {
            return false;
        }

        if (Player == null || !Player.TrySpendGold(item.Cost))
        {
            SetStatusMessage(NotEnoughGoldMessage);
            return false;
        }

        item.Unlock();
        OnBowPurchased?.Invoke(item);
        TryEquipBow(item.BowId);
        SetStatusMessage(string.Format(PurchasedFormat, item.DisplayName));
        return true;
    }

    public bool TryEquipBow(string bowId)
    {
        if (string.IsNullOrEmpty(bowId) || !_itemLookup.TryGetValue(bowId, out BowShopItemData item) || !item.IsOwned)
        {
            return false;
        }

        EquippedBowId = item.BowId;

        if (Player != null && Player.WeaponHolder != null && item.BowData != null)
        {
            Player.WeaponHolder.SetBowData(item.BowData);
        }

        OnBowEquipped?.Invoke(item);
        RefreshAllItems();
        return true;
    }
    #endregion

    #region Private Methods
    private void PopulateDefaultBows()
    {
        BowItems = new List<BowShopItemData>
        {
            new BowShopItemData(SingleShotId, SingleShotDisplayName, SingleShotCost, true),
            new BowShopItemData(DoubleShotId, DoubleShotDisplayName, DoubleShotCost, false),
            new BowShopItemData(SpreadShotId, SpreadShotDisplayName, SpreadShotCost, false)
        };
    }

    private void InitializeItemStates()
    {
        _itemLookup.Clear();
        EquippedBowId = string.Empty;

        for (int i = 0; i < BowItems.Count; i++)
        {
            BowShopItemData item = BowItems[i];
            if (item == null || string.IsNullOrEmpty(item.BowId))
            {
                continue;
            }

            item.ResetRuntimeState();
            _itemLookup[item.BowId] = item;
        }
    }

    private void SyncInitialEquippedBow()
    {
        BowSO currentBowData = Player != null && Player.WeaponHolder != null
            ? Player.WeaponHolder.BowData
            : null;

        if (currentBowData != null)
        {
            BowShopItemData matchedItem = FindItemByBowData(currentBowData);
            if (matchedItem != null)
            {
                matchedItem.Unlock();
                EquippedBowId = matchedItem.BowId;
            }

            return;
        }

        for (int i = 0; i < BowItems.Count; i++)
        {
            BowShopItemData item = BowItems[i];
            if (item != null && item.IsOwned)
            {
                TryEquipBow(item.BowId);
                return;
            }
        }
    }

    private void SyncEquippedFromWeaponHolder()
    {
        if (Player == null || Player.WeaponHolder == null || Player.WeaponHolder.BowData == null)
        {
            return;
        }

        BowShopItemData matchedItem = FindItemByBowData(Player.WeaponHolder.BowData);
        if (matchedItem != null)
        {
            matchedItem.Unlock();
            EquippedBowId = matchedItem.BowId;
        }
    }

    private BowShopItemData FindItemByBowData(BowSO bowData)
    {
        if (bowData == null)
        {
            return null;
        }

        for (int i = 0; i < BowItems.Count; i++)
        {
            BowShopItemData item = BowItems[i];
            if (item != null && item.BowData == bowData)
            {
                return item;
            }
        }

        return null;
    }

    private void BindSceneItemUIs()
    {
        _itemUIs.Clear();

        if (_sceneItemUIs == null || _sceneItemUIs.Count == 0)
        {
            _sceneItemUIs = new List<BowShopItemUI>(GetComponentsInChildren<BowShopItemUI>(true));
        }

        for (int i = 0; i < _sceneItemUIs.Count; i++)
        {
            BowShopItemUI itemUI = _sceneItemUIs[i];
            if (itemUI == null || string.IsNullOrEmpty(itemUI.BowId))
            {
                continue;
            }

            _itemUIs[itemUI.BowId] = itemUI;
        }
    }

    private void SubscribeEvents()
    {
        if (Player != null)
        {
            Player.OnGoldChanged += HandleGoldChanged;

            if (Player.WeaponHolder != null)
            {
                Player.WeaponHolder.OnBowChanged += HandleBowChanged;
            }
        }

        if (_closeButton != null)
        {
            _closeButton.onClick.AddListener(CloseShop);
        }

        if (_openShopButton != null)
        {
            _openShopButton.onClick.AddListener(ToggleShop);
        }

        foreach (BowShopItemUI itemUI in _itemUIs.Values)
        {
            if (itemUI != null)
            {
                itemUI.OnActionClicked += HandleItemActionClicked;
            }
        }
    }

    private void UnsubscribeEvents()
    {
        if (Player != null)
        {
            Player.OnGoldChanged -= HandleGoldChanged;

            if (Player.WeaponHolder != null)
            {
                Player.WeaponHolder.OnBowChanged -= HandleBowChanged;
            }
        }

        if (_closeButton != null)
        {
            _closeButton.onClick.RemoveListener(CloseShop);
        }

        if (_openShopButton != null)
        {
            _openShopButton.onClick.RemoveListener(ToggleShop);
        }

        foreach (BowShopItemUI itemUI in _itemUIs.Values)
        {
            if (itemUI != null)
            {
                itemUI.OnActionClicked -= HandleItemActionClicked;
            }
        }
    }

    private void HandleGoldChanged(int _)
    {
        RefreshAllItems();
    }

    private void HandleBowChanged(BowSO bowData)
    {
        BowShopItemData matchedItem = FindItemByBowData(bowData);
        if (matchedItem != null)
        {
            matchedItem.Unlock();
            EquippedBowId = matchedItem.BowId;
        }
        else if (bowData != null)
        {
            EquippedBowId = string.Empty;
        }

        RefreshAllItems();
    }

    private void HandleItemActionClicked(string bowId)
    {
        if (!_itemLookup.TryGetValue(bowId, out BowShopItemData item))
        {
            return;
        }

        if (item.IsOwned)
        {
            if (TryEquipBow(item.BowId))
            {
                SetStatusMessage(string.Format(EquippedMessageFormat, item.DisplayName));
            }

            return;
        }

        TryBuyBow(item.BowId);
    }

    private void RefreshAllItems()
    {
        RefreshHeader();
        int currentGold = Player != null ? Player.Gold : DefaultGoldAmount;

        foreach (KeyValuePair<string, BowShopItemUI> pair in _itemUIs)
        {
            if (!_itemLookup.TryGetValue(pair.Key, out BowShopItemData item) || pair.Value == null)
            {
                continue;
            }

            BowShopItemState state = EvaluateItemState(item);
            bool canAfford = currentGold >= item.Cost;
            pair.Value.RefreshView(item.DisplayName, item.Cost, item.Icon, state, canAfford);
        }
    }

    private void RefreshHeader()
    {
        int currentGold = Player != null ? Player.Gold : DefaultGoldAmount;
        if (_goldText != null)
        {
            _goldText.text = string.Format(GoldFormat, currentGold);
        }

        if (_equippedText != null)
        {
            string equippedName;
            if (!string.IsNullOrEmpty(EquippedBowId) && _itemLookup.TryGetValue(EquippedBowId, out BowShopItemData equippedItem))
            {
                equippedName = equippedItem.DisplayName;
            }
            else if (Player != null && Player.WeaponHolder != null && Player.WeaponHolder.BowData != null)
            {
                equippedName = Player.WeaponHolder.BowData.name;
            }
            else
            {
                equippedName = EmptySelectionPlaceholder;
            }

            _equippedText.text = string.Format(EquippedFormat, equippedName);
        }
    }

    private BowShopItemState EvaluateItemState(BowShopItemData item)
    {
        if (!item.IsOwned)
        {
            return BowShopItemState.Purchasable;
        }

        return item.BowId == EquippedBowId
            ? BowShopItemState.Equipped
            : BowShopItemState.Owned;
    }

    private void SetShopVisible(bool visible)
    {
        if (_panelRoot != null)
        {
            _panelRoot.SetActive(visible);
        }
    }

    private void SetStatusMessage(string message)
    {
        if (_statusText != null)
        {
            _statusText.text = message;
        }
    }

    private void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystemObject = new GameObject(EventSystemObjectName);
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<InputSystemUIInputModule>();
    }
    #endregion
}
