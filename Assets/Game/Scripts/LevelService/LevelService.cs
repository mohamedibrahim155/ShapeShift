using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Zenject;

namespace Scripts.Level
{
    public class LevelService : ILevelService
    {
        private  LevelConfig _levelConfig;
        private  BlockConfig _blockConfig;
        private DiContainer _container;

        private List<LevelView> LevelList = new List<LevelView>();

        public event Action<LevelView> OnLevelCreated = delegate { };

        [Inject]
        public void Setup(LevelConfig config, DiContainer container, BlockConfig blockConfig )
        {
            _levelConfig = config;
            _container = container;
            _blockConfig = blockConfig;
        }

        public void CreateLevel(int levelNo)
        {
            LevelView levelView = _container.InstantiatePrefabForComponent<LevelView>(_levelConfig.LevelViewPrefab);

            levelView.transform.position = Vector3.zero;

            LevelList.Add(levelView);

            SpawnBlocksForLevel(levelView);

            OnLevelCreated.Invoke(levelView);
        }


        public LevelView GetLevel(int levelNo)
        {
            return LevelList[levelNo];
        }

        public void SpawnBlocksForLevel(LevelView view)
        {
            if (view == null) return;

            if (view.m_LevelParts != null)
            {

                foreach (GameObject spawnPoint in view.m_LevelParts)
                {

                    float ZSpacing = _levelConfig.BlockZSpacing;
                    Vector3 intialPosition =  new Vector3(spawnPoint.transform.position.x, 0 , spawnPoint.transform.position.z );

                    Collider collider = view.Collider;
                    float MaxZ = collider.bounds.max.z;
                    for (int i = 0; i < _levelConfig.MaxBlockIterationPerSpawnPoint; i++)
                    {
                        float nextZ = intialPosition.z + ZSpacing;

                        if (nextZ > MaxZ)
                        {
                            break;
                        }
                        int randomBlock = UnityEngine.Random.Range(0, _blockConfig.Blocks.Count);
                        BlockView blockInstance = _container.InstantiatePrefabForComponent<BlockView>(_blockConfig.Blocks[randomBlock]);
                        blockInstance.transform.parent = spawnPoint.transform;

                        blockInstance.transform.position = intialPosition;
                        intialPosition = blockInstance.transform.position + new Vector3(0, 0, ZSpacing);

                        view.AddBlock(blockInstance);

                    }

                }


            }

        }
    }
}
