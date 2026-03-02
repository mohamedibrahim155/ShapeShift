using Scripts.Level;
using Scripts.Player;
using System;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class FinishLine : MonoBehaviour
{
    public Collider Collider;


    [Inject] private PlayerConfig _config;
    [Inject] private ILevelService _levelService;  

    private void Reset()
    {
        Collider = GetComponentInChildren<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((_config.m_PlayerLayer & (1 << other.gameObject.layer)) != 0)
        {
            OnPlayerReachedFinishLine();
        }

    }

    private void OnPlayerReachedFinishLine()
    {
        _levelService.InvokeLevelCompleted();
    }
}
