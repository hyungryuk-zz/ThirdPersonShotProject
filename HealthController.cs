using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HealthController : NetworkBehaviour {

    GameObject healthBar;
    float maxhealth;
    float oldcurrentHealth;
    float currentHealth;
    [SyncVar]
    public float damage;
    public bool isDead;
    Animator ani;

 
	// Use this for initialization
	void Start () {
        isDead = false;
        maxhealth = 1.0f;
        damage = 0.0f;
        oldcurrentHealth = 1.0f;
        ani = GetComponent<Animator>();
        healthBar = GameObject.FindGameObjectWithTag("HealthBar");
	}
	
	// Update is called once per frame
	void Update () {
        currentHealth = maxhealth - damage;
        if(oldcurrentHealth != currentHealth)
        {
            if(oldcurrentHealth - currentHealth > 0 && currentHealth>0)
            {
                ani.SetBool("isHit", true);
                StartCoroutine(hitDuration());
                transform.GetChild(6).GetComponent<AudioSource>().Play();
            }
            if (currentHealth <= 0)
            {
                ani.SetBool("isDead", true);
                isDead = true;
            }
            oldcurrentHealth = currentHealth;
        }
        if (isLocalPlayer)
        {
            healthBar.transform.GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = currentHealth;
        }

    }
    IEnumerator hitDuration()
    {
        yield return new WaitForSeconds(0.5f);
        ani.SetBool("isHit", false);

    }

}
