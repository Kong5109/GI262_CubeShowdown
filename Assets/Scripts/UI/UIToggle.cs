using System;
using UnityEngine;

public class UIToggle : MonoBehaviour
{
    #region Fields & Properties
    [field: SerializeField] public GameObject TargetObject { get; private set; }

    public bool IsOpen => ResolveTarget().activeSelf;

    public event Action<bool> OnToggled;
    #endregion

    #region Unity Lifecycle
    #endregion

    #region Public Methods
    public void Open()
    {
        SetOpen(true);
    }

    public void Close()
    {
        SetOpen(false);
    }

    public void Toggle()
    {
        SetOpen(!IsOpen);
    }

    public void SetOpen(bool isOpen)
    {
        GameObject target = ResolveTarget();
        if (target.activeSelf == isOpen)
        {
            return;
        }

        target.SetActive(isOpen);
        OnToggled?.Invoke(isOpen);
    }
    #endregion

    #region Private Methods
    private GameObject ResolveTarget()
    {
        return TargetObject != null ? TargetObject : gameObject;
    }
    #endregion
}
