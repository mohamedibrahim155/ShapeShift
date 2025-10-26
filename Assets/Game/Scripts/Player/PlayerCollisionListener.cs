using Scripts.Player;
using System;
using UnityEngine;
using Zenject;


public enum ShapeType
{
    Sphere,
    Cube,
    Capsule,
    Cylinder
}
public class PlayerCollisionListener : MonoBehaviour
{

    public static event Action<ShapeType> OnShapeCollision = delegate { };
    [SerializeField] private ShapeType shapeType;


    private void OnTriggerEnter(Collider other)
    {
        OnShapeCollision?.Invoke(shapeType);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnShapeCollision?.Invoke(shapeType);
    }
}
