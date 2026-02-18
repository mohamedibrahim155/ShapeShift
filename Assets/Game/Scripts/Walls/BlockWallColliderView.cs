using System;
using UnityEngine;
using Zenject;

public class BlockWallColliderView : MonoBehaviour
{
    public static event  Action<BlockView> OnBlockCollision;

    [SerializeField] private BlockView m_Block;
    [SerializeField] private BlockConfig _config;

    [Inject]
    public void Construct(BlockConfig config)
    {
        _config = config;
    }
    private void OnTriggerEnter(Collider collision)
    {
        if ((_config.m_CollisionLayer & (1 << collision.gameObject.layer)) != 0)
        {
            InvokeCollisionWithPlayer();
        }  
    }

    private void InvokeCollisionWithPlayer()
    {
        OnBlockCollision?.Invoke(m_Block);
    }

    public void SetBlock(BlockView view)
    {
        m_Block = view;
    }
}
