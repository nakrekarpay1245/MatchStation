using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Data
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Data/GameData")]

    public class GameData : ScriptableObject
    {
        [Header("GAME DATA")]
        [Tooltip("")]
        [SerializeField]
        private List<LevelConfig> _levelList;

        [SerializeField]
        private int _currentLevelIndex;
        public int CurrentLevelIndex
        {
            get => _currentLevelIndex % _levelList.Count;
            set => _currentLevelIndex = value;
        }

        public LevelConfig CurrentLevel => _levelList[CurrentLevelIndex];

    }
}