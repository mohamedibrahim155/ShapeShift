using Scripts.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class BlockView : MonoBehaviour
{
    [SerializeField] private EBlockType m_BlockType;
    [SerializeField] private Collider m_Collider;
    [SerializeField] private BlockWallColliderView m_ColliderListener;

   private IPlayerService m_playerService;
   private BlockConfig _config;

    [Inject]
    private void Construct(IPlayerService playerService,  BlockConfig config)
    {
        m_playerService =  playerService;
        _config = config; 
    }

    public EBlockType BlockType => m_BlockType;
    public BlockConfig Config => _config;

    private void Start()
    {
        if (m_playerService != null)
        {
            m_playerService.RegisterBlockWall(this);
        }
    }

    private void Reset()
    {
        m_Collider = GetComponentInChildren<Collider>();
        m_ColliderListener = GetComponentInChildren<BlockWallColliderView>();
        m_ColliderListener.SetBlock(this);
    }

    public EBlockType GetShapeType()
    {
        return m_BlockType;
    }

}
