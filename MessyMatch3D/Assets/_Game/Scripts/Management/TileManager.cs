using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Management
{
    /// <summary>
    /// Manages the tiles in the game, including tracking active tiles and providing access to empty tiles.
    /// </summary>
    public class TileManager : MonoBehaviour
    {
        [Header("Tile Manager Parameters")]
        [Tooltip("The list of all tiles managed by this manager.")]
        [SerializeField]
        private List<Tile> _activeTileList;

        /// <summary>
        /// Finds the first empty tile in the list.
        /// </summary>
        /// <returns>The first empty tile, or null if no empty tiles are available.</returns>
        public Tile FindEmptyTile()
        {
            foreach (var tile in _activeTileList)
            {
                if (!tile.Item)
                {
                    return tile;
                }
            }
            return null;
        }
    }
}