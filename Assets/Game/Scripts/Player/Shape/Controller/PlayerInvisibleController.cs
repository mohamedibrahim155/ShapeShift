using Scripts.Level;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

namespace Scripts.Player
{
    public class PlayerInvisibleController 
    {
        
        private readonly PlayerView _playerView;
        private readonly PlayerConfig _playerConfig;
        private readonly ILevelService _levelService;

        private int _currentBlockIndex;
        private List<BlockView> _blocks;

        public PlayerInvisibleController(PlayerView playerView, PlayerConfig playerConfig, ILevelService levelService)
        {
            _playerView = playerView;
            _playerConfig = playerConfig;
            _levelService = levelService;
            _currentBlockIndex = 0;

            InitializeBlocks();
        }

        private void InitializeBlocks()
        {
            var currentLevelBlocks = _levelService.GetCurrentLevelBlocks();
            _blocks = new List<BlockView>(currentLevelBlocks);
        }

        public void Update()
        {
            BlockView currentBlock = GetCurrentBlock();

            if (currentBlock == null)
            {
                Hide();
                return;
            }
           
            float distance =  GetDistanceSq(currentBlock); ;

            if (distance > _playerConfig.m_HighlightRadius * _playerConfig.m_HighlightRadius)
            {
                Hide();
                return;
            }

            Color color = ((int)_playerConfig.m_CurrentShapeType == (int)currentBlock.BlockType) ? Color.green : Color.red;
            color.a = 0.5f;

            _playerView.UpdateHighligherPosition(currentBlock.transform.position, color, _playerConfig.m_CurrentShapeType);

        }

        public void NextBlock()
        {
            _currentBlockIndex++;
            Hide();
        }

        public void Hide()
        {
            _playerView.HideTransparentShapes();
        }

        public BlockView GetCurrentBlock()
        {
            if (_blocks == null || _currentBlockIndex >= _blocks.Count)
                return null;

            return _blocks[_currentBlockIndex];
        }

        //Taking z and x axis alone
        private float GetDistanceSq(BlockView block)
        {
            Vector3 d = _playerView.transform.position - block.transform.position;
            return d.x * d.x + d.z * d.z;
        }
    }
    
}