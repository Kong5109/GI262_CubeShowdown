using System;
using UnityEngine;

[Serializable]
public class ArrowSkillNodeData
{
    #region Fields & Properties
    [field: SerializeField] public string NodeId { get; private set; }
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public string ParentNodeId { get; private set; }
    [field: SerializeField] public int Tier { get; private set; }
    [field: SerializeField] public float HorizontalPosition { get; private set; }
    [field: SerializeField] public int Cost { get; private set; }
    [field: SerializeField] public bool UnlockedByDefault { get; private set; }
    [field: SerializeField] public GameObject ArrowPrefab { get; private set; }

    public bool IsUnlocked { get; private set; }
    #endregion

    #region Public Methods
    public ArrowSkillNodeData()
    {
    }

    public ArrowSkillNodeData(
        string nodeId,
        string displayName,
        string parentNodeId,
        int tier,
        float horizontalPosition,
        int cost,
        bool unlockedByDefault = false)
    {
        NodeId = nodeId;
        DisplayName = displayName;
        ParentNodeId = parentNodeId;
        Tier = tier;
        HorizontalPosition = horizontalPosition;
        Cost = cost;
        UnlockedByDefault = unlockedByDefault;
    }

    public void ResetRuntimeState()
    {
        IsUnlocked = UnlockedByDefault;
    }

    public void Unlock()
    {
        IsUnlocked = true;
    }
    #endregion
}

