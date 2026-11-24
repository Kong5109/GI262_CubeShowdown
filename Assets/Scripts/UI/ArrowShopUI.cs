using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class ArrowShopUI : MonoBehaviour
{
    #region Fields & Properties
    [field: Header("References")]
    [field: SerializeField] public Player Player { get; private set; }

    [field: Header("Shop Settings")]
    [field: SerializeField] public bool OpenOnStart { get; private set; } = true;
    [field: SerializeField] public List<ArrowSkillNodeData> SkillNodes { get; private set; } = new List<ArrowSkillNodeData>();

    [Header("UI References")]
    [SerializeField] private GameObject _panelRoot;
    [SerializeField] private UnityEngine.UI.Text _goldText;
    [SerializeField] private UnityEngine.UI.Text _selectedText;
    [SerializeField] private UnityEngine.UI.Text _statusText;
    [SerializeField] private UnityEngine.UI.Button _closeButton;
    [SerializeField] private UnityEngine.UI.Button _openShopButton;
    [SerializeField] private List<ArrowSkillNodeUI> _sceneNodeUIs = new List<ArrowSkillNodeUI>();

    public string SelectedNodeId { get; private set; }
    public bool IsOpen => _panelRoot != null && _panelRoot.activeSelf;

    public event Action<ArrowSkillNodeData> OnArrowPurchased;
    public event Action<ArrowSkillNodeData> OnArrowSelected;

    private const string EventSystemObjectName = "EventSystem";
    private const string GoldFormat = "Gold: {0}G";
    private const string SelectedFormat = "Selected: {0}";
    private const string EmptySelectionPlaceholder = "-";
    private const string DefaultStatusHint = "Click a yellow node to Buy, or a blue node to Select.";
    private const string NotEnoughGoldMessage = "Not enough Gold!";
    private const string PurchasedFormat = "Purchased & Selected: {0}";
    private const string SelectedMessageFormat = "Selected: {0}";
    private const string LockedMessageFormat = "Unlock {0} first!";

    private const string NormalId = "normal";
    private const string FireId = "fire";
    private const string IceId = "ice";
    private const string PierceId = "pierce";
    private const string FireBombId = "fire_bomb";
    private const string FireAreaId = "fire_area";
    private const string FreezeId = "freeze";
    private const string IceAreaId = "ice_area";
    private const string HomingId = "homing";
    private const string BounceId = "bounce";

    private const string NormalDisplayName = "Normal";
    private const string FireDisplayName = "Fire Arrow";
    private const string IceDisplayName = "Ice Arrow";
    private const string PierceDisplayName = "Pierce Arrow";
    private const string FireBombDisplayName = "Fire Bomb Arrow";
    private const string FireAreaDisplayName = "Fire Area Arrow";
    private const string FreezeDisplayName = "Freeze Arrow";
    private const string IceAreaDisplayName = "Ice Area Arrow";
    private const string HomingDisplayName = "Homing Arrow";
    private const string BounceDisplayName = "Bounce Arrow";

    private const int Tier0 = 0;
    private const int Tier1 = 1;
    private const int Tier2 = 2;
    private const int FreeCost = 0;
    private const int Tier1Cost = 50;
    private const int Tier2Cost = 100;
    private const int DefaultGoldAmount = 0;

    private const float CenterX = 0f;
    private const float Tier1BranchX = 380f;
    private const float Tier2OuterX = 468f;
    private const float Tier2MidX = 292f;
    private const float Tier2InnerX = 88f;

    private readonly Dictionary<string, ArrowSkillNodeData> _nodeLookup = new Dictionary<string, ArrowSkillNodeData>();
    private readonly Dictionary<string, ArrowSkillNodeUI> _nodeUIs = new Dictionary<string, ArrowSkillNodeUI>();
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (Player == null)
        {
            Player = FindAnyObjectByType<Player>();
        }

        if (SkillNodes == null || SkillNodes.Count == 0)
        {
            PopulateDefaultSkillTree();
        }

        InitializeNodeStates();
        BindSceneNodeUIs();
        EnsureEventSystem();
        RefreshAllNodes();
        SetStatusMessage(DefaultStatusHint);
        SetShopVisible(OpenOnStart);
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void Reset()
    {
        PopulateDefaultSkillTree();
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
        SetStatusMessage(DefaultStatusHint);
        RefreshAllNodes();
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

    public bool TrySelectNode(string nodeId)
    {
        if (!_nodeLookup.TryGetValue(nodeId, out ArrowSkillNodeData node) || !node.IsUnlocked)
        {
            return false;
        }

        SelectedNodeId = node.NodeId;

        if (Player != null && Player.WeaponHolder != null && node.ArrowPrefab != null)
        {
            Player.WeaponHolder.SetArrowPrefab(node.ArrowPrefab);
        }

        OnArrowSelected?.Invoke(node);
        RefreshAllNodes();
        return true;
    }
    #endregion

    #region Private Methods
    private void PopulateDefaultSkillTree()
    {
        SkillNodes = new List<ArrowSkillNodeData>
        {
            new ArrowSkillNodeData(NormalId, NormalDisplayName, string.Empty, Tier0, CenterX, FreeCost, true),

            new ArrowSkillNodeData(FireId, FireDisplayName, NormalId, Tier1, -Tier1BranchX, Tier1Cost),
            new ArrowSkillNodeData(IceId, IceDisplayName, NormalId, Tier1, CenterX, Tier1Cost),
            new ArrowSkillNodeData(PierceId, PierceDisplayName, NormalId, Tier1, Tier1BranchX, Tier1Cost),

            new ArrowSkillNodeData(FireBombId, FireBombDisplayName, FireId, Tier2, -Tier2OuterX, Tier2Cost),
            new ArrowSkillNodeData(FireAreaId, FireAreaDisplayName, FireId, Tier2, -Tier2MidX, Tier2Cost),

            new ArrowSkillNodeData(FreezeId, FreezeDisplayName, IceId, Tier2, -Tier2InnerX, Tier2Cost),
            new ArrowSkillNodeData(IceAreaId, IceAreaDisplayName, IceId, Tier2, Tier2InnerX, Tier2Cost),

            new ArrowSkillNodeData(HomingId, HomingDisplayName, PierceId, Tier2, Tier2MidX, Tier2Cost),
            new ArrowSkillNodeData(BounceId, BounceDisplayName, PierceId, Tier2, Tier2OuterX, Tier2Cost)
        };
    }

    private void InitializeNodeStates()
    {
        _nodeLookup.Clear();
        SelectedNodeId = string.Empty;

        for (int i = 0; i < SkillNodes.Count; i++)
        {
            ArrowSkillNodeData node = SkillNodes[i];
            if (node == null || string.IsNullOrEmpty(node.NodeId))
            {
                continue;
            }

            node.ResetRuntimeState();
            _nodeLookup[node.NodeId] = node;

            if (string.IsNullOrEmpty(SelectedNodeId) && node.IsUnlocked)
            {
                SelectedNodeId = node.NodeId;
            }
        }
    }

    private void BindSceneNodeUIs()
    {
        _nodeUIs.Clear();

        if (_sceneNodeUIs == null || _sceneNodeUIs.Count == 0)
        {
            _sceneNodeUIs = new List<ArrowSkillNodeUI>(GetComponentsInChildren<ArrowSkillNodeUI>(true));
        }

        for (int i = 0; i < _sceneNodeUIs.Count; i++)
        {
            ArrowSkillNodeUI nodeUI = _sceneNodeUIs[i];
            if (nodeUI == null || string.IsNullOrEmpty(nodeUI.NodeId))
            {
                continue;
            }

            _nodeUIs[nodeUI.NodeId] = nodeUI;
        }
    }

    private void SubscribeEvents()
    {
        if (Player != null)
        {
            Player.OnGoldChanged += HandleGoldChanged;
        }

        if (_closeButton != null)
        {
            _closeButton.onClick.AddListener(CloseShop);
        }

        if (_openShopButton != null)
        {
            _openShopButton.onClick.AddListener(ToggleShop);
        }

        foreach (ArrowSkillNodeUI nodeUI in _nodeUIs.Values)
        {
            if (nodeUI != null)
            {
                nodeUI.OnNodeClicked += HandleNodeClicked;
            }
        }
    }

    private void UnsubscribeEvents()
    {
        if (Player != null)
        {
            Player.OnGoldChanged -= HandleGoldChanged;
        }

        if (_closeButton != null)
        {
            _closeButton.onClick.RemoveListener(CloseShop);
        }

        if (_openShopButton != null)
        {
            _openShopButton.onClick.RemoveListener(ToggleShop);
        }

        foreach (ArrowSkillNodeUI nodeUI in _nodeUIs.Values)
        {
            if (nodeUI != null)
            {
                nodeUI.OnNodeClicked -= HandleNodeClicked;
            }
        }
    }

    private void HandleGoldChanged(int _)
    {
        RefreshAllNodes();
    }

    private void HandleNodeClicked(string nodeId)
    {
        if (!_nodeLookup.TryGetValue(nodeId, out ArrowSkillNodeData node))
        {
            return;
        }

        if (node.IsUnlocked)
        {
            TrySelectNode(node.NodeId);
            SetStatusMessage(string.Format(SelectedMessageFormat, node.DisplayName));
            return;
        }

        if (!CanUnlockNode(node))
        {
            string parentName = GetParentDisplayName(node);
            SetStatusMessage(string.Format(LockedMessageFormat, parentName));
            return;
        }

        if (Player == null || !Player.TrySpendGold(node.Cost))
        {
            SetStatusMessage(NotEnoughGoldMessage);
            return;
        }

        node.Unlock();
        OnArrowPurchased?.Invoke(node);
        TrySelectNode(node.NodeId);
        SetStatusMessage(string.Format(PurchasedFormat, node.DisplayName));
    }

    private bool CanUnlockNode(ArrowSkillNodeData node)
    {
        if (node == null || node.IsUnlocked)
        {
            return false;
        }

        if (string.IsNullOrEmpty(node.ParentNodeId))
        {
            return true;
        }

        return _nodeLookup.TryGetValue(node.ParentNodeId, out ArrowSkillNodeData parentNode) && parentNode.IsUnlocked;
    }

    private string GetParentDisplayName(ArrowSkillNodeData node)
    {
        if (node == null || string.IsNullOrEmpty(node.ParentNodeId))
        {
            return string.Empty;
        }

        return _nodeLookup.TryGetValue(node.ParentNodeId, out ArrowSkillNodeData parentNode)
            ? parentNode.DisplayName
            : node.ParentNodeId;
    }

    private void RefreshAllNodes()
    {
        RefreshHeader();

        foreach (KeyValuePair<string, ArrowSkillNodeUI> pair in _nodeUIs)
        {
            if (!_nodeLookup.TryGetValue(pair.Key, out ArrowSkillNodeData node) || pair.Value == null)
            {
                continue;
            }

            ArrowSkillNodeState state = EvaluateNodeState(node);
            pair.Value.RefreshView(node.DisplayName, node.Cost, GetParentDisplayName(node), state);
        }
    }

    private void RefreshHeader()
    {
        int currentGold = Player != null ? Player.Gold : DefaultGoldAmount;
        if (_goldText != null)
        {
            _goldText.text = string.Format(GoldFormat, currentGold);
        }

        if (_selectedText != null)
        {
            string selectedName = _nodeLookup.TryGetValue(SelectedNodeId, out ArrowSkillNodeData selectedNode)
                ? selectedNode.DisplayName
                : EmptySelectionPlaceholder;
            _selectedText.text = string.Format(SelectedFormat, selectedName);
        }
    }

    private ArrowSkillNodeState EvaluateNodeState(ArrowSkillNodeData node)
    {
        if (node.IsUnlocked)
        {
            return node.NodeId == SelectedNodeId
                ? ArrowSkillNodeState.Selected
                : ArrowSkillNodeState.Unlocked;
        }

        return CanUnlockNode(node)
            ? ArrowSkillNodeState.Purchasable
            : ArrowSkillNodeState.Locked;
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
