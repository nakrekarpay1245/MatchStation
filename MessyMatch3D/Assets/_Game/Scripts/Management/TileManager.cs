using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Game.Scripts.Tiles;
using DG.Tweening;
using _Game.Scripts.Items;

namespace _Game.Scripts.Management
{
    /// <summary>
    /// Manages the tiles in the game, including tracking active tiles and organizing items within them.
    /// </summary>
    public class TileManager : MonoBehaviour
    {
        [Header("Tile Manager Parameters")]
        [Tooltip("The list of all tiles managed by this manager.")]
        [SerializeField]
        private List<Tile> _activeTileList;

        [Header("Item Move Settings")]
        [Tooltip("Duration for items to move to their new positions.")]
        [SerializeField]
        private float _itemMoveDuration = 0.5f;

        /// <summary>
        /// Aligns tiles by collecting all items, sorting them by type, and reassigning them to the tiles.
        /// </summary>
        public void AlignMatchingItems()
        {
            // Collect all items from the tiles
            List<Item> items = _activeTileList.Where(tile => tile.Item != null)
                                              .Select(tile => tile.Item)
                                              .ToList();

            // Sort items by their type (e.g., ItemId)
            items = items.OrderBy(item => item.ItemId).ToList();

            // Clear all tiles
            foreach (var tile in _activeTileList)
            {
                tile.Item = null;
            }

            // Reassign sorted items back to tiles
            for (int i = 0; i < items.Count; i++)
            {
                _activeTileList[i].Item = items[i];
                // Animate the item moving to its new position
                items[i].transform.DOMove(_activeTileList[i].transform.position + Vector3.up,
                    _itemMoveDuration).SetEase(Ease.InOutQuad);
            }
        }

        /// <summary>
        /// Finds the first empty tile in the list.
        /// </summary>
        /// <returns>The first empty tile, or null if no empty tiles are available.</returns>
        public Tile FindEmptyTile()
        {
            return _activeTileList.FirstOrDefault(tile => tile != null && tile.Item == null);
        }
    }
}