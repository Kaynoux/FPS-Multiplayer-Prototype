using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;
using System.Security.AccessControl;

public class PlayerCameraController : NetworkBehaviour
{
    public Vector2 maxFollowOsset = new Vector2(-1f, 6f);
    public Vector2 cameraVelocity = new Vector4(4f, .25f);
    public Transform playerTransform = null;
    public CinemachineVirtualCamera virtualCamera = null;
    private CinemachineTransposer transposer;

    public override void OnStartAuthority()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        virtualCamera.gameObject.SetActive(true);
        enabled = true;
        
    }
}
