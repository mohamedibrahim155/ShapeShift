using Scripts.Player;
using System;
using UnityEngine;
using Zenject;


public enum EShapeType
{
    CUBE = 0,
    SPHERE = 1,
    CYLINDER = 2,
    CAPSULE = 3,
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
