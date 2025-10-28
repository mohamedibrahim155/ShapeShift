using Scripts.Player;
using System;
using UnityEngine;
using Zenject;


public enum EShapeType
{
    SPHERE,
    CUBE,
    CAPSULE,
    CYLINDER
}
public class PlayerCollisionListener : MonoBehaviour
{

    public static event Action<EShapeType, GameObject> OnShapeCollision = delegate { };
    [SerializeField] private EShapeType shapeType;


    private void OnTriggerEnter(Collider other)
    {
        OnShapeCollision?.Invoke(shapeType, other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //OnShapeCollision?.Invoke(shapeType, collision.gameObject);
    }
}
