using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Scripts.Level
{
    public class LevelService : ILevelService
    {
        private List<LevelConfig> _levelConfig;
        private BlockConfig _blockConfig;
        private DiContainer _container;
        private List<LevelView> _currentLevels = new List<LevelView>();
        private Transform _levelParent;

        public event Action OnLevelCreated = delegate { };
        public event Action OnLevelCompleted = delegate { };



        [Inject]
        public void Setup(List<LevelConfig> config, DiContainer container, BlockConfig blockConfig)
        {
            _levelConfig = config;
            _container = container;
            _blockConfig = blockConfig;
        }

        public void CreateLevel(int levelNo)
        {
            if (_levelParent == null)
            {
                _levelParent = new GameObject("LEVEL").transform;
            }

            _levelParent.name = $"LEVEL_{levelNo}";

            List<LevelView> currentLevelGorund = _levelConfig[levelNo].LevelPrefabs;

            Vector3 chunkPosition = Vector3.zero;
            for (int i = 0; i < currentLevelGorund.Count; i++)
            {

                LevelView levelToCreate = currentLevelGorund[i];

                // half extents of the current level chunk to position the next chunk correctly
                if (i > 0)
                {
                    chunkPosition.z += levelToCreate.GetZBounds() / 2;
                }

                LevelView levelView = _container.InstantiatePrefabForComponent<LevelView>(currentLevelGorund[i], _levelParent);


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

        public void InvokeLevelCompleted()
        {
            OnLevelCompleted.Invoke();
        }

        public void Cleanup()
        {
            foreach (LevelView levelView in _currentLevels)
            {
                GameObject.Destroy(levelView.gameObject);
            }

            _currentLevels.Clear();

        }
    }
}
