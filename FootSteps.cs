using UnityEngine;
using UnityEngine.Networking;
namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class FootSteps : NetworkBehaviour
    {
        [SyncVar]
        bool checkIsWalking;

        bool checkIsWalkingChanged;
        public bool m_IsGrounded;
        public bool isMoving;
        public AudioSource audio;
        // Use this for initialization
        void Start()
        {
            checkIsWalking = false;
            checkIsWalkingChanged = checkIsWalking;
            audio = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        void LeftFootSound()
        {
            if (isLocalPlayer)
            {
                audio.Play();
                CmdleftFoodSound();
            }
            else
            {
                if (checkIsWalkingChanged != checkIsWalking)
                {
                    audio.Play();
                    checkIsWalkingChanged = checkIsWalking;
                }
            }
        }
        void RightFootSound()
        {
            if (isLocalPlayer)
            {
                audio.Play();
                CmdleftFoodSound();
            }
            else
            {
                if (checkIsWalkingChanged != checkIsWalking)
                {
                    audio.Play();
                    checkIsWalkingChanged = checkIsWalking;
                }
            }
        }
        [Command]
        void CmdleftFoodSound()
        {
            checkIsWalking = !checkIsWalking;

        }
        [Command]
        void CmdrightFootSound()
        {
            checkIsWalking = !checkIsWalking;
        }
    }

}