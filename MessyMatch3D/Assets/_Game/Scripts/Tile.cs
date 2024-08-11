using _Game.Scripts.Items;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Item _item;
    public Item Item
    {
        get { return _item; }
        set { _item = value; }
    }

    private void Awake()
    {
        _item = null;
    }
}