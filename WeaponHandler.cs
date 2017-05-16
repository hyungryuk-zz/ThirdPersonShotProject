using UnityEngine;
using UnityEngine.Networking;

public class WeaponHandler : NetworkBehaviour
{

    Camera cam;
    RaycastHit hit;
    public float availableDistanceTogetWeapon;
    public Transform spine;
    public Transform rightHand;

    public Vector3 spineRot;

    public GameObject currWeapon;

    [SyncVar]
    public NetworkInstanceId rifle1;
    [SyncVar]
    public NetworkInstanceId rifle2;
    [SyncVar]
    public NetworkInstanceId pistol;
    [SyncVar]
    public NetworkInstanceId tool;

    [SyncVar]
    public bool rifle1Check;
    [SyncVar]
    public bool rifle2Check;
    [SyncVar]
    public bool pistolCheck;
    [SyncVar]
    public bool toolCheck;

    Animator ani;

    public int currWeaponNum;

    public Transform m4;
    int lm;
    // Use this for initialization
    void Start()
    {
        currWeaponNum = 0;
        if (!isLocalPlayer) enabled = false; ;
        spineRot.x = -4.3f;
        spineRot.y = 57.14f;
        spineRot.z = 18.4f;
        rifle1Check = false;
        rifle2Check = false;
        pistolCheck = false;
        toolCheck = false;

        ani = GetComponent<Animator>();

        cam = GameObject.FindGameObjectWithTag("Camera").GetComponentInChildren<Camera>();
        lm = 1 << LayerMask.NameToLayer("CrossHair");
        lm = 1 << LayerMask.NameToLayer("Player");
        lm = ~lm;

    }

    // Update is called once per frame
    void Update()
    {

        if (Physics.SphereCast(cam.transform.position, 0.1f, cam.transform.forward, out hit, 10000, lm))
        {
        }
        if (Input.GetButtonDown("F") && Vector3.Distance(hit.transform.position, transform.position) < availableDistanceTogetWeapon && hit.transform.tag == "Weapon")
        {
            Debug.Log("Get Weapon");
            switch ((int)hit.transform.GetComponent<Weapon>().weaponType)
            {
                case 1:
                    CmdPickRifle(hit.transform.GetComponent<NetworkIdentity>().netId, GetComponent<NetworkIdentity>().netId);
                    break;

            }

        }
        if (Input.GetButtonDown("1") && rifle1Check)
        {
            CmdChangeCurrentWeapon(rifle1);
            ani.SetInteger("WeaponType", 1);
            currWeaponNum = 1;
        }
        if (Input.GetButton("aim"))
        {
            switch (currWeaponNum)
            {
                case 1:
                    ani.SetBool("isAiming", true);
                    CmdIsAiming(rifle1);
                    break;
            }
        }
        if (Input.GetButtonUp("aim"))
        {
            switch (currWeaponNum)
            {
                case 1:
                    ani.SetBool("isAiming", true);
                    CmdIsAimingFinish(rifle1);
                    break;
            }
        }

    }
    private void LateUpdate()
    {
        if (Input.GetButton("aim"))
        {
            spine.localRotation = Quaternion.Euler(spineRot);
        }
    }
    

    [Command]
    void CmdIsAimingFinish(NetworkInstanceId weaponNetId)
    {
        GameObject targetWeapon = NetworkServer.FindLocalObject(weaponNetId);
        targetWeapon.GetComponent<Weapon>().isAiming = false;
    }
    [Command]
    void CmdIsAiming(NetworkInstanceId weaponNetId)
    {
        GameObject targetWeapon = NetworkServer.FindLocalObject(weaponNetId);
        targetWeapon.GetComponent<Weapon>().isAiming = true;
    }
    [Command]
    void CmdChangeCurrentWeapon(NetworkInstanceId weaponNetId)
    {
        GameObject targetWeapon = NetworkServer.FindLocalObject(weaponNetId);
        targetWeapon.GetComponent<Weapon>().isCurrent = true;
    }
    [Command]
    void CmdPickRifle(NetworkInstanceId weaponNetId, NetworkInstanceId clientId)
    {
        GameObject targetWeapon = NetworkServer.FindLocalObject(weaponNetId);
        switch ((int)targetWeapon.GetComponent<Weapon>().weaponType)
        {
            case 1:
                switch ((int)targetWeapon.GetComponent<Weapon>().name)
                {
                    case 1:

                        Debug.Log("CallCmd");
                        targetWeapon.GetComponent<Weapon>().ownerId = (int)clientId.Value;
                        if (!rifle1Check)
                        {
                            targetWeapon.GetComponent<Weapon>().charged1 = true;
                            rifle1 = weaponNetId;
                            rifle1Check = true;
                        }
                        else if(!rifle2Check)
                        {
                            targetWeapon.GetComponent<Weapon>().charged2 = true;
                            rifle2 = weaponNetId;
                            rifle2Check = true;
                        }
                        break;
                }
                break;
        }
        targetWeapon.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);



    }

}

