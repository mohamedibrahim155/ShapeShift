using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class PlayerView : MonoBehaviour
{

    [SerializeField] GameObject[] shapeTransforms;

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

  
}
