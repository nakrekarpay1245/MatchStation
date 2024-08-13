using UnityEngine;

public interface ISelector
{
    void Select(ISelectable selectable);
    void DeSelect(ISelectable selectable);
}
