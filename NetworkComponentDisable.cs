using UnityEngine;
using UnityEngine.Networking;

namespace UnityStandardAssets.Characters.ThirdPerson
{


    public class NetworkComponentDisable : NetworkBehaviour
    {
        // Use this for initialization
        void Start()
        {
            if (!isLocalPlayer)
            {
                transform.GetComponent<ThirdPersonUserControl>().enabled = false;
                transform.GetComponent<ThirdPersonCharacter>().enabled = false;                
            }
        }


    }
}