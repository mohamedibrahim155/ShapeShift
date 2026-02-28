using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Scripts.Level
{
    public class LevelService : ILevelService
    {
        private  List<LevelConfig> _levelConfig;
        private  BlockConfig _blockConfig;
        private DiContainer _container;
        private List<LevelView> _currentLevels = new List<LevelView>();

        public event Action OnLevelCreated = delegate { };



        [Inject]
        public void Setup(List<LevelConfig> config, DiContainer container, BlockConfig blockConfig )
        {
            _levelConfig = config;
            _container = container;
            _blockConfig = blockConfig;
        }

        public void CreateLevel(int levelNo)
        {
            Transform LevelParent =  new GameObject("LEVEL").transform;
            List<LevelView> currentLevelGorund = _levelConfig[levelNo].LevelPrefabs;

            Vector3 chunkPosition = Vector3.zero;
            for (int i = 0; i < currentLevelGorund.Count; i++)
            {

                LevelView levelToCreate = currentLevelGorund[i];

                if (i > 0)
                {
                    chunkPosition.z += levelToCreate.GetZBounds() / 2;
                }

                LevelView levelView = _container.InstantiatePrefabForComponent<LevelView>(currentLevelGorund[i], LevelParent);

              
                levelView.transform.position = chunkPosition;

                chunkPosition.z += levelView.GetZBounds() / 2;

                _currentLevels.Add(levelView);
            }


            OnLevelCreated.Invoke();
        }


        public LevelConfig GetLevel(int levelNo)
        {
            return _levelConfig[levelNo];
        }

        public void SpawnBlocksForLevel(LevelView view)
        {
            if (view == null) return;

        }
    }
}
