using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class PlayerView : MonoBehaviour
{
     public Transform m_ShapeParent;

    [SerializeField] public GameObject[] shapeTransforms;

    [Inject] private PlayerConfig playerConfig;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableShape(EShapeType shapeType)
    {
        int shapeIndex = (int)shapeType;

        playerConfig.SetCurrentShape(shapeType);

        shapeTransforms[shapeIndex].SetActive(true);

    }

    public void ChangeShape(EShapeType eShapeType)
    {
        DisableShape(playerConfig.m_CurrentShapeType);
        EnableShape(eShapeType);
    }


    public void DisableShape(EShapeType shapeIndex)
    {
         shapeTransforms[(int)shapeIndex].SetActive(false);
    }

    public void SpawnShapes(DiContainer diContainer)
    {
       m_ShapeParent = new GameObject("ShapeParent").transform;
       m_ShapeParent.SetParent(transform);

        shapeTransforms = null;
        shapeTransforms = new GameObject[playerConfig.m_ListOfShapes.Count];

        foreach (ShapeConfig item in playerConfig.m_ListOfShapes)
        {
            GameObject shapeInstance = diContainer.InstantiatePrefab(item.m_ShapeView);
            shapeInstance.transform.SetParent(m_ShapeParent);
            shapeInstance.SetActive(false);
            shapeTransforms[(int)item.m_ShapeType] = shapeInstance;
        }
    }

}
