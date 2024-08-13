using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts._helpers
{
    [System.Serializable]
    public class ParticleData
    {
        public string ParticleName;
        public int ParticleCount;
        [SerializeField]
        private List<ParticleSystem> _particleSystemList;
        public ParticleSystem ParticleSystem
        {
            get => _particleSystemList[Random.Range(0, _particleSystemList.Count)];
            private set { }
        }

        public ParticleData(ParticleSystem ps, int count, string name)
        {
            ParticleSystem = ps;
            ParticleCount = count;
            ParticleName = name;
        }
    }
}