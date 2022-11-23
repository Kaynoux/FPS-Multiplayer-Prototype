
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Gun : NetworkBehaviour
{
    public int maxAmmo = 12;
    public float reloadTime = 1f;
    public float impactForce = 30f;
    public int damage = 10;
    public float fireRate = 15f;
    public float range = 100f;
    public float ejectSpeed = 10f;

    
    public Transform ejectTrans;
    public Rigidbody shellRigidbody;
    public Camera fpsCam;
    public ParticleSystem muzzleflash;
    public GameObject impactEffect;
    public Animator anim;

    private bool isReloading = false;
    private int currentAmmo;
    private float nexTimeToFire = 0f;
    // Update is called once per frame
    
    private void Start()
    {
        if(isLocalPlayer)
        {
          currentAmmo = maxAmmo;
        }
        
        
    }

    private void Awake()
    {
        CanvasManager.instance.UpdateMaxAmmo(maxAmmo);
        CanvasManager.instance.UpdateCurrentAmmo(currentAmmo);
    }
    private void OnEnable()
    {
        if (isLocalPlayer)
        {
            isReloading = false;
            anim.SetBool("isReloading", false);
            CanvasManager.instance.UpdateMaxAmmo(maxAmmo);
            CanvasManager.instance.UpdateCurrentAmmo(currentAmmo);
        }
       


    }


    private void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown("r") && isReloading == false)
            {
                StartCoroutine(Reload());
            }

            if (Input.GetButton("Fire1") && Time.time >= nexTimeToFire && currentAmmo >= 1)
            {
                nexTimeToFire = Time.time + 1f / fireRate;
                Shoot();

            }
        }
       
           
    }

    
    IEnumerator Reload ()
    {
        anim.SetBool("isReloading", true);
        isReloading = true;
        Debug.Log("isReloading");
        yield return new WaitForSeconds(reloadTime - .25f);
        anim.SetBool("isReloading", false);
        yield return new WaitForSeconds(.25f);
        currentAmmo = maxAmmo;
        CanvasManager.instance.UpdateCurrentAmmo(currentAmmo);
       
        isReloading = false;
    }
  
    void Shoot()
    {
        anim.SetTrigger("triggerShoot");
        muzzleflash.Play();

        currentAmmo--;
        CanvasManager.instance.UpdateCurrentAmmo(currentAmmo);
       
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
               
            }
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            Rigidbody shellRigid = Instantiate(shellRigidbody, ejectTrans.transform.position, ejectTrans.transform.rotation);
            shellRigid.velocity = ejectTrans.transform.TransformDirection(Vector3.left * ejectSpeed);
            GameObject impactGo = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGo, 2f);
        }


    }

    


}
