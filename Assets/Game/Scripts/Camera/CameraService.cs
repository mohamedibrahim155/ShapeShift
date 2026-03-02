using UnityEngine;
using Zenject;
using Zenject.Asteroids;


public class CameraService : ICameraService
{


    public CameraView CameraView { get; private set; }

    private CameraConfig m_CameraConfig;
    private DiContainer m_Container;
    private CameraType currentCameraID;

    [Inject]
    private void Construct(CameraConfig config, DiContainer container)
    {
        m_CameraConfig = config;
        m_Container = container;
    }

    //sets the follow target for all cameras
    public void SetCameraFollow(Transform followTarget)
    {
        CameraView.GetCamera(ECameraType.START_CAMERA).CinemachineCamera.Follow      = followTarget;
        CameraView.GetCamera(ECameraType.FOLLOW_CAMERA).CinemachineCamera.Follow     = followTarget;
        CameraView.GetCamera(ECameraType.FINISHLINE_CAMERA).CinemachineCamera.Follow = followTarget;
    }

    //sets the look at target for all cameras
    public void SetCameraLookAt(Transform lookAtTransform)
    {
         CameraView.GetCamera(ECameraType.START_CAMERA).CinemachineCamera.LookAt      = lookAtTransform;
         CameraView.GetCamera(ECameraType.FOLLOW_CAMERA).CinemachineCamera.LookAt     = lookAtTransform;
         CameraView.GetCamera(ECameraType.FINISHLINE_CAMERA).CinemachineCamera.LookAt = lookAtTransform;
    }

    //spawns camera view at provided position
    public void SpawnCamera(Vector3 spawnPosition)
    {
        //intializes camera view from prefab
        CameraView = m_Container.InstantiatePrefabForComponent<CameraView>(m_CameraConfig.CameraView);
        CameraView.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, CameraView.transform.position.z);
        CameraView.Setup(m_CameraConfig);

        //sets to start camera by default
        EnableCamera(ECameraType.START_CAMERA);
    }

    //sets visual priority of cameras based on type
    public void EnableCamera(ECameraType cameraID)
    {
        switch (cameraID)
        {
            case ECameraType.START_CAMERA:
                CameraView.GetCamera(ECameraType.START_CAMERA).CinemachineCamera.Priority      = 10;
                CameraView.GetCamera(ECameraType.FOLLOW_CAMERA).CinemachineCamera.Priority     =  0;
                CameraView.GetCamera(ECameraType.FINISHLINE_CAMERA).CinemachineCamera.Priority =  0;
                break;
            case ECameraType.FOLLOW_CAMERA:
                CameraView.GetCamera(ECameraType.START_CAMERA).CinemachineCamera.Priority       =  0;
                CameraView.GetCamera(ECameraType.FOLLOW_CAMERA).CinemachineCamera.Priority      = 10;
                CameraView.GetCamera(ECameraType.FINISHLINE_CAMERA).CinemachineCamera.Priority  =  0;
                break;
            case ECameraType.FINISHLINE_CAMERA:
                 CameraView.GetCamera(ECameraType.START_CAMERA).CinemachineCamera.Priority       =  0;
                 CameraView.GetCamera(ECameraType.FOLLOW_CAMERA).CinemachineCamera.Priority      =  0;
                 CameraView.GetCamera(ECameraType.FINISHLINE_CAMERA).CinemachineCamera.Priority  = 10;
                break;
            default:
                break;
        }
    }

    //sets the follow target for all cameras
    public void  Cleanup()
    {
        if (CameraView!= null)
        {
            GameObject.Destroy(CameraView.gameObject);
        }
    }

}
