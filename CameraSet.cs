using UnityEngine;
using UnityEngine.Networking;

namespace UnityStandardAssets.Cameras {
    public class CameraSet : NetworkBehaviour {
        GameObject camera;
        // Use this for initialization
        void Start() {
            if (isLocalPlayer)
            {
                camera = GameObject.FindGameObjectWithTag("Camera");
                camera.GetComponent<FreeLookCam>().SetTarget(this.transform);
                GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().m_Cam = camera.transform;
            }
        }
        
    }
}