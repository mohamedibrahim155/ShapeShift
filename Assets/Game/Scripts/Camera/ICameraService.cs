using UnityEditor.Compilation;
using UnityEngine;
using Zenject;
public interface ICameraService  {
   
    public abstract void SpawnCamera(Vector3 spawnPosition);
    public abstract void SetCameraLookAt(Transform lookAtTransform);
    public abstract void SetCameraFollow(Transform followTarget);

    public abstract void EnableCamera(ECameraType type);
   
    public CameraView CameraView { get; }
}
