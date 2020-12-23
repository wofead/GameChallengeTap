using UnityEngine;

public class BaseComponent : MonoBehaviour
{
    public void AddTo(GameObject node)
    {
        transform.SetParent(node.transform, false);
    }

    public void AddChild(GameObject node)
    {
        node.transform.SetParent(transform, false);
    }
}
