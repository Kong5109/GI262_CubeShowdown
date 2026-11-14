using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    public virtual void Update()
    {
        Debug.Log(this.gameObject.name + " Effect Apply");
    }
}
