using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncRotation : NetworkBehaviour {

    [SyncVar]
    Vector3 charLookAt;

    Camera cam;
    public Transform spine;
    int lm;
	// Use this for initialization
	void Start () {
        lm = 1 << LayerMask.NameToLayer("LocalPlayer");
        lm = ~lm;
        cam = GameObject.FindGameObjectWithTag("Camera").GetComponentInChildren<Camera>();
        
    }
    
    void LateUpdate () {
        RaycastHit hit;        
        Physics.SphereCast(cam.transform.position, 0.1f, cam.transform.forward, out hit, 100.0f, lm);
        if (isLocalPlayer)
        {
            CmdSyncRot(hit.point);
        }
        if (!isLocalPlayer)
        {
            spine.LookAt(charLookAt);
        }
        else
        {
            spine.LookAt(hit.point);
        }


    }

    [Command]
    void CmdSyncRot(Vector3 hitPoint)
    {
        charLookAt = hitPoint;
    }
}
