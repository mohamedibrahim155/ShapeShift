using UnityEngine;


[System.Serializable]
public class CinimachineCameraSettings
{
    public Vector3 FollowOffset;
    public Vector2 ScreenPosition;
    public Vector3 TargetOffset;

}

[CreateAssetMenu(fileName = "CameraConfig", menuName = "Scriptable Objects/Configs/CameraConfig")]
public class CameraConfig : ScriptableObject
{
    public CameraView CameraView;

    //Start of the game camera positions
    public CinimachineCameraSettings StartCameraSettings;
    //Following of the game camera positions
    public CinimachineCameraSettings FollowCameraSettings;
    //Finish of endline  camera positions
    public CinimachineCameraSettings FinishCameraSettings;

    public float m_RotationSpeed = 100f;
}
