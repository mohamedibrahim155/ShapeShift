using Scripts.Player;
using System;
using UnityEngine;
using Zenject;

public class BlockWallColliderView : MonoBehaviour
{
    public event  Action OnBlockCollision = delegate { };

    private IPlayerService m_PlayerService;
    private BlockView m_Block;
    private BlockConfig m_Config;

    private void OnTriggerEnter(Collider collision)
    {
        if ((m_Config.m_CollisionLayer & (1 << collision.gameObject.layer)) != 0)
        {
            InvokeCollisionWithPlayer();
        }  
    }

    private void InvokeCollisionWithPlayer()
    {
        OnBlockCollision?.Invoke();
    }

    public void Setup(BlockView view, BlockConfig config, IPlayerService playerService)
    {
        m_Block = view;
        m_Config = config;
        m_PlayerService = playerService;
    }
}
