using UnityEngine;
using Zenject;
using Zenject.Asteroids;


public class CameraService : ICameraService
{


    public CameraView CameraView { get; private set; }

    private CameraConfig m_CameraConfig;
    private DiContainer m_Container;

    [Inject]
    private void Construct(CameraConfig config, DiContainer container)
    {
        m_CameraConfig = config;
        m_Container = container;
    }

    public void SetCameraFollow(Transform followTarget)
    {
        CameraView.GetCamera(ECameraType.START_CAMERA).m_CinemachineCamera.Follow      = followTarget;
        CameraView.GetCamera(ECameraType.FOLLOW_CAMERA).m_CinemachineCamera.Follow     = followTarget;
        CameraView.GetCamera(ECameraType.FINISHLINE_CAMERA).m_CinemachineCamera.Follow = followTarget;
    }

    public void SetCameraLookAt(Transform lookAtTransform)
    {
         CameraView.GetCamera(ECameraType.START_CAMERA).m_CinemachineCamera.LookAt      = lookAtTransform;
         CameraView.GetCamera(ECameraType.FOLLOW_CAMERA).m_CinemachineCamera.LookAt     = lookAtTransform;
         CameraView.GetCamera(ECameraType.FINISHLINE_CAMERA).m_CinemachineCamera.LookAt = lookAtTransform;
    }

    public void SpawnCamera(Vector3 spawnPosition)
    {
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
                CameraView.GetCamera(ECameraType.START_CAMERA).m_CinemachineCamera.Priority      = 10;
                CameraView.GetCamera(ECameraType.FOLLOW_CAMERA).m_CinemachineCamera.Priority     =  0;
                CameraView.GetCamera(ECameraType.FINISHLINE_CAMERA).m_CinemachineCamera.Priority =  0;
                break;
            case ECameraType.FOLLOW_CAMERA:
                CameraView.GetCamera(ECameraType.START_CAMERA).m_CinemachineCamera.Priority       =  0;
                CameraView.GetCamera(ECameraType.FOLLOW_CAMERA).m_CinemachineCamera.Priority      = 10;
                CameraView.GetCamera(ECameraType.FINISHLINE_CAMERA).m_CinemachineCamera.Priority  =  0;
                break;
            case ECameraType.FINISHLINE_CAMERA:
                 CameraView.GetCamera(ECameraType.START_CAMERA).m_CinemachineCamera.Priority       =  0;
                 CameraView.GetCamera(ECameraType.FOLLOW_CAMERA).m_CinemachineCamera.Priority      =  0;
                 CameraView.GetCamera(ECameraType.FINISHLINE_CAMERA).m_CinemachineCamera.Priority  = 10;
                break;
            default:
                break;
        }
    }
  


}
