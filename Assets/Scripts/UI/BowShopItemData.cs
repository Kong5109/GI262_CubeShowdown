using System;
using UnityEngine;

[Serializable]
public class BowShopItemData
{
    #region Fields & Properties
    [field: SerializeField] public string BowId { get; private set; }
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public int Cost { get; private set; }
    [field: SerializeField] public bool OwnedByDefault { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public BowSO BowData { get; private set; }

    public bool IsOwned { get; private set; }
    #endregion

    #region Public Methods
    public BowShopItemData()
    {
    }

    public BowShopItemData(
        string bowId,
        string displayName,
        int cost,
        bool ownedByDefault = false,
        BowSO bowData = null,
        Sprite icon = null)
    {
        BowId = bowId;
        DisplayName = displayName;
        Cost = cost;
        OwnedByDefault = ownedByDefault;
        BowData = bowData;
        Icon = icon;
    }

    public void ResetRuntimeState()
    {
        IsOwned = OwnedByDefault;
    }

    public void Unlock()
    {
        IsOwned = true;
    }
    #endregion
}
