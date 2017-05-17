using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class WeaponHandler : NetworkBehaviour
{

    public Camera cam;
    RaycastHit hit;
    public float availableDistanceTogetWeapon;
    public Transform spine;
    public Transform rightHand;
    public Vector3 spineRot;

    public Text weaponName;
    public Text ammo;

    public Transform currWeapon;

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

    public Weapon currentWeapon;
    public GameObject Flare;

    [SerializeField]
    private LayerMask mask;
    
    Animator ani;

    public int currWeaponNum;

    int lm;
    // Use this for initialization
    void Start()
    {
        weaponName = GameObject.FindGameObjectWithTag("HealthBar").transform.GetChild(2).GetComponent<Text>();
        ammo = GameObject.FindGameObjectWithTag("HealthBar").transform.GetChild(1).GetComponent<Text>();
        currWeaponNum = 0;
        if (!isLocalPlayer) enabled = false; ;
        spineRot.x = -4.3f;
        spineRot.y = 57.14f;
        spineRot.z = 18.4f;
        rifle1Check = false;
        rifle2Check = false;
        pistolCheck = false;
        toolCheck = false;
        if (isLocalPlayer)
        {
            gameObject.layer = LayerMask.NameToLayer("LocalPlayer");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("RemotePlayer");
        }
        ani = GetComponent<Animator>();

        cam = GameObject.FindGameObjectWithTag("Camera").GetComponentInChildren<Camera>();
        lm = 1 << LayerMask.NameToLayer("CrossHair");
        lm = 1 << LayerMask.NameToLayer("LocalPlayer");
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
        if(currentWeapon != null)
        {
            ammo.text = currentWeapon.CurrentAmmo + " / " + currentWeapon.MaxAmmo;
        }
        if (Input.GetButtonDown("1") && rifle1Check)
        {
            CmdChangeCurrentWeapon(rifle1);
            ani.SetInteger("WeaponType", 1);
            currWeaponNum = 1;
            currentWeapon = spine.GetChild(1).GetComponent<Weapon>();
            Debug.Log(currentWeapon.damage);
            weaponName.text = currentWeapon.name.ToString();
            ammo.text = currentWeapon.CurrentAmmo + " / " + currentWeapon.MaxAmmo;
            GameObject.FindGameObjectWithTag("HealthBar").transform.GetChild((int)currentWeapon.name+5).GetComponent<Image>().enabled = true;

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
                    ani.SetBool("isAiming", false);
                    CmdIsAimingFinish(rifle1);
                    break;
            }
        }
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

    }
    
    void Shoot()
    {
        if (currentWeapon.CurrentAmmo <= 0)
        {
            return;
        }
        Debug.Log("Shoot");
        RaycastHit BulletHit;
        if (currentWeapon != null)
        {
            if (isLocalPlayer)
            {
                CmdShootSound(currentWeapon.GetComponent<NetworkIdentity>().netId);
            }

            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out BulletHit, currentWeapon.range,mask))
            {
                Debug.Log(BulletHit.transform.tag);
                if(BulletHit.transform.gameObject.layer == LayerMask.NameToLayer("RemotePlayer"))
                {
                    Debug.Log("ShootRemotePlayer");
                    CmdHitPlayer(BulletHit.transform.GetComponent<NetworkIdentity>().netId, currentWeapon.damage);
                }
                else
                {                   
                    CmdFlare(BulletHit.point, BulletHit.transform.up);
                }
            }
        }   

    }
    [Command]
    void CmdFlare(Vector3 pos, Vector3 rot)
    {
        GameObject flare = (GameObject)Instantiate(Flare, pos, Quaternion.Euler(rot));
        NetworkServer.Spawn(flare);
        StartCoroutine(FlareEffectTime(flare.GetComponent<NetworkIdentity>().netId));
    }
    IEnumerator FlareEffectTime(NetworkInstanceId flareId)
    {
        yield return new WaitForSeconds(0.2f);
        CmdFlareDelete(flareId);

    }    
    [Command]
    void CmdFlareDelete(NetworkInstanceId flareId)
    {
        GameObject targetEffect = NetworkServer.FindLocalObject(flareId);
        Destroy(targetEffect);
    }

    [Command]
    void CmdShootSound(NetworkInstanceId weaponNetId)
    {
        GameObject targetWeapon = NetworkServer.FindLocalObject(weaponNetId);
        targetWeapon.GetComponent<Weapon>().isFired = !targetWeapon.GetComponent<Weapon>().isFired;
    }
    [Command]
    void CmdHitPlayer(NetworkInstanceId weaponNetId,float damage)
    {
        GameObject target = NetworkServer.FindLocalObject(weaponNetId);
        target.GetComponent<HealthController>().damage += damage;
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

