using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Scripts.Level
{
    public class LevelService : ILevelService
    {
        private LevelListData m_LevelConfigPresets;
        private BlockConfig m_BlockConfig;
        private DiContainer m_Container;
        private List<LevelView> _currentLevels = new List<LevelView>();
        private Transform _levelParent;
        private int _currentLevelIndex = 0;

        public event Action OnLevelCreated = delegate { };
        public event Action OnLevelCompleted = delegate { };
        public event Action OnLevelFailed = delegate { };

        public FinishLine FinishLineView { get; private set; }


        [Inject]
        public void Setup(LevelListData config, DiContainer container, BlockConfig blockConfig)
        {
            m_LevelConfigPresets = config;
            m_Container = container;
            m_BlockConfig = blockConfig;


            SpawnLevel(GetCurrentLevel());
        }
        public void CreateLevel(int levelNumber)
        {
            if (_levelParent == null)
            {
                _levelParent = new GameObject("LEVEL").transform;
            }

            _levelParent.name = $"LEVEL_{levelNumber}";

            int idx = levelNumber < 1   ? 1 : levelNumber-1;

            Debug.Log($"Creating level {idx}" +  _currentLevels.Count);

            List<LevelView> currentLevelGorund = GetLevel(GetWrappedLevelIndex(idx)).ListOfChunks;

            Vector3 chunkPosition = Vector3.zero;
            for (int i = 0; i < currentLevelGorund.Count; i++)
            {

                LevelView levelToCreate = currentLevelGorund[i];

                // half extents of the current level chunk to position the next chunk correctly
                if (i > 0)
                {
                    chunkPosition.z += levelToCreate.GetZBounds() / 2;
                }

                LevelView levelView = m_Container.InstantiatePrefabForComponent<LevelView>(currentLevelGorund[i], _levelParent);


                levelView.transform.position = chunkPosition;

                chunkPosition.z += levelView.GetZBounds() / 2;

                _currentLevels.Add(levelView);
            }


            OnLevelCreated.Invoke();
        }

        public void InitializeFinishLine(FinishLine view)
        {
            FinishLineView = view;
        }



        public LevelConfig GetLevel(int index)
        {
            return m_LevelConfigPresets.m_Levels[index];
        }

        public void SpawnBlocksForLevel(LevelView view)
        {
            if (view == null) return;

        }

        public void InvokeLevelCompleted()
        {
            OnLevelCompleted.Invoke();
        }
        public void InvokeLevelFailed()
        {
            OnLevelFailed.Invoke();
        }



        public int GetWrappedLevelIndex(int levelNumber)
        {
            return levelNumber % m_LevelConfigPresets.m_Levels.Count;
        }

        public void SpawnLevel(int levelNumber)
        {
            int currentLevel = GetWrappedLevelIndex(levelNumber);
            CreateLevel(levelNumber);
        }

        public void SpawnNextLevel()
        {
            int nextLevelID = GetCurrentLevel() + 1;
            UpdateLevel(nextLevelID);
            SpawnLevel(nextLevelID);
        }

        public int GetCurrentLevel()
        {
            return m_LevelConfigPresets.m_CurrentLevelIndex;
        }

        public void UpdateLevel(int levelNo)
        {
            m_LevelConfigPresets.m_CurrentLevelIndex = levelNo;
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
