using Scripts.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class BlockView : MonoBehaviour
{
    [SerializeField] private EBlockType m_BlockType;
    [SerializeField] private List<Collider> m_Collider;

   private IPlayerService m_playerService;

    [Inject]
    private void Construct(IPlayerService playerService)
    {
        m_playerService =  playerService;
    }

    public EBlockType BlockType => m_BlockType;

    private void Start()
    {
        if (m_playerService != null)
        {
            m_playerService.RegisterBlockWall(this);
        }
    }

    private void Reset()
    {
        m_Collider = GetComponentsInChildren<Collider>().ToList();
    }

}
