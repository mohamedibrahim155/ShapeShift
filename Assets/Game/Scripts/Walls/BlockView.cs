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
    [SerializeField] private TransparentBlockView m_TransparentBlockView;

    private IPlayerService m_playerService;
    private BlockConfig m_Config;
    public EBlockType BlockType => m_BlockType;
    public int BlockID { get; private set; }


    [Inject]
    private void Construct(IPlayerService playerService,  BlockConfig config)
    {
        m_playerService =  playerService;
        m_Config = config;
    }

    private void Reset()
    {
        m_Collider = GetComponentInChildren<Collider>();
        m_ColliderListener = GetComponentInChildren<BlockWallColliderView>();
        m_TransparentBlockView = GetComponentInChildren<TransparentBlockView>(true);
    }

    private void Initialize()
    {
        if (m_ColliderListener == null)
            return;

        m_ColliderListener.Setup(this, m_Config, m_playerService);
       
    }

    private void OnEnable()
    {
        Initialize();
        m_playerService.OnPlayerCrossedWall += Animate;
        m_ColliderListener.OnBlockCollision += HandleCollision;
    }

    private void OnDisable()
    {
        m_playerService.OnPlayerCrossedWall -= Animate;
        m_ColliderListener.OnBlockCollision -= HandleCollision;
    }
    private void Animate(BlockView view)
    {
        if (m_TransparentBlockView == null)
            return;

        if (view ==  this)
        {
            m_TransparentBlockView.Show();
            m_TransparentBlockView.SetColor(m_playerService.PlayerConfig.m_PlayerColor);
            m_TransparentBlockView.AnimateScaling(m_Config.m_AnimationScaleFactor, m_Config.m_AnimationDuration, () => m_TransparentBlockView.Hide());
        }
    }

    public void SetID(int ID)
    {
        BlockID = ID;
    }

    private void HandleCollision()
    {
        m_playerService.CheckCollision(this);
    }

}
