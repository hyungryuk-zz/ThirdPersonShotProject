using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Weapon : NetworkBehaviour {

    public enum type
    {
        rifle=1, pistol=2, tool=3
    }
    public enum weaponName
    {
        m4 = 1
    }

    public Image weaponImage;
    public Vector3 rifleUnEquipedPos;
    public Vector3 rifleUnEquipedRot;

    public Vector3 rifleUnEquipedPos1;
    public Vector3 rifleUnEquipedRot1;

    public Vector3 rifleEquipedPos;
    public Vector3 rifleEquipedRot;

    public Vector3 rifleAimedPos;
    public Vector3 rifleAimedRot;


    public type weaponType;
    public weaponName name;
    [SyncVar]
    public int ownerId;
    [SyncVar]
    public bool isCurrent;
    [SyncVar]
    public bool charged1;
    [SyncVar]
    public bool charged2;
    [SyncVar]
    public bool isAiming;
    [SyncVar]
    public bool isFired;

    bool isOldFired;
    public float damage;
    public float range;

    public float MaxAmmo;
    public float CurrentAmmo;


    AudioSource audio;

    // Use this for initialization
    void Start () {
        audio = GetComponent<AudioSource>();
        isFired = false;
        isOldFired = isFired;
        charged1 = false;
        charged2 = false;
        isCurrent = false;
        isAiming = false;
        rifleUnEquipedPos.x = -0.018f;
        rifleUnEquipedPos.y = 0.225f;
        rifleUnEquipedPos.z = -0.152f;
        rifleUnEquipedRot.x = 72.028f;
        rifleUnEquipedRot.y = -105.74f;
        rifleUnEquipedRot.z = -11.453f;
        rifleUnEquipedPos1.x = 0.012f;
        rifleUnEquipedPos1.y = 0.272f;
        rifleUnEquipedPos1.z = -0.173f;
        rifleUnEquipedRot1.x = 120.092f;
        rifleUnEquipedRot1.y = -92.513f;
        rifleUnEquipedRot1.z = -8.752f;
        rifleEquipedPos.x = -0.0579f;
        rifleEquipedPos.y = 0.2605f;
        rifleEquipedPos.z = 0.0515f;
        rifleEquipedRot.x = 65.803f;
        rifleEquipedRot.y = 171.707f;
        rifleEquipedRot.z = -77.912f;
        rifleAimedPos.x = -0.027f;
        rifleAimedPos.y = 0.252f;
        rifleAimedPos.z = 0.033f;
        rifleAimedRot.x = 81.052f;
        rifleAimedRot.y = -134.12f;
        rifleAimedRot.z = -56.87f;
    }
	
	// Update is called once per frame
	void Update () {
        if (charged1&&!isCurrent)
        {

            GameObject[] golist = GameObject.FindGameObjectsWithTag("Player");
            for(int i=0;i< golist.Length; i++)
            {
                if(golist[i].GetComponent<NetworkIdentity>().netId.Value == ownerId)
                {
                    transform.SetParent(golist[i].GetComponent<WeaponHandler>().spine);
                }
            }
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<BoxCollider>().enabled = false;
            transform.localPosition = rifleUnEquipedPos;
            transform.localRotation = Quaternion.Euler(rifleUnEquipedRot);
        }
        if (charged2 && !isCurrent)
        {

            GameObject[] golist = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < golist.Length; i++)
            {
                if (golist[i].GetComponent<NetworkIdentity>().netId.Value == ownerId)
                {
                    transform.SetParent(golist[i].GetComponent<WeaponHandler>().spine);
                }
            }
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<BoxCollider>().enabled = false;
            transform.localPosition = rifleUnEquipedPos1;
            transform.localRotation = Quaternion.Euler(rifleUnEquipedRot1);
        }
        if (isCurrent && !isAiming)
        {
            GameObject[] golist = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < golist.Length; i++)
            {
                if (golist[i].GetComponent<NetworkIdentity>().netId.Value == ownerId)
                {
                    transform.SetParent(golist[i].GetComponent<WeaponHandler>().rightHand);
                }
            }
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<BoxCollider>().enabled = false;
            transform.localPosition = rifleEquipedPos;
            transform.localRotation = Quaternion.Euler(rifleEquipedRot);
        }
        if (isAiming&& isCurrent)
        {
            GameObject[] golist = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < golist.Length; i++)
            {
                if (golist[i].GetComponent<NetworkIdentity>().netId.Value == ownerId)
                {
                    transform.SetParent(golist[i].GetComponent<WeaponHandler>().rightHand);
                }
            }
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<BoxCollider>().enabled = false;
            transform.localPosition = rifleAimedPos;
            transform.localRotation = Quaternion.Euler(rifleAimedRot);
        }
        if(isOldFired != isFired)
        {
            CurrentAmmo--;
            audio.Play();
            isOldFired = isFired;
        }
    }
}
