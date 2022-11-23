using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Security.Permissions;
using Mirror.Websocket;
using UnityEngine.Networking.Types;

public class AllGunsScript : NetworkBehaviour, IDamageable
{
    public int selectedWeapon = 0;
    public Vector3 ejectVector = new Vector3(20f, 3f, 4f);


    
    private bool isReloading = false;
    private float nexTimeToFire = 0f;
    private GameObject impactEffect;
    private GameObject settingsCanvas;
    private bool settingsMenuIsOpen = false;



    [Header("PlayerObjects")]
    [SyncVar] public int Health;
    [SyncVar] public int HealthMax = 100;
    [SyncVar] public int Kills;
    [SyncVar] public int Deaths;
    [SyncVar] public bool isDead;
    public GameObject bulletHoleFXPrefab;
    public AudioManager audioManager;
    

    [Header("GFX")]
    [SerializeField] GameObject[] disableOnClient;
    [SerializeField] GameObject[] disableOnDeath;


    [Header("Pistol")]
    public int pistolCurrentAmmo = 12;
    public int pistolMaxAmmo = 12;
    public float pistolReloadTime = 0.25f;
    public float pistolImpactForce = 30f;
    public int pistolDamage = 10;
    public float pistolFireRate = 3f;
    public float pistolRange = 100f;
    public float pistolEjectSpeed = 10f;
    public Animator pistolAnim;
    public ParticleSystem pistolEjectPS;
    public ParticleSystem pistolMuzzleflash;

    [Header("Ar")]
    public int arCurrentAmmo = 30;
    public int arMaxAmmo = 30;
    public float arReloadTime = 1.5f;
    public float arImpactForce = 30f;
    public int arDamage = 10;
    public float arFireRate = 10f;
    public float arRange = 100f;
    public float arEjectSpeed = 10f;
    public Animator arAnim;
    public ParticleSystem arEjectPS;
    public ParticleSystem arMuzzleflash;

    








    void Start()
    {
       
        
        if (!isLocalPlayer)
        {
            return;
        }
        if(isLocalPlayer)
        {
            settingsCanvas = GameObject.Find("SettingsCanvas").gameObject;
            audioManager = gameObject.GetComponent<AudioManager>();
            CanvasManager.instance.ChangePlayerState(true);
            CanvasManager.instance.allGunsScript = this;
            Cursor.lockState = CursorLockMode.Locked;
            isDead = false;
            LookForWeapon();
            pistolCurrentAmmo = pistolMaxAmmo;
            arCurrentAmmo = arMaxAmmo;
            WeaponSwitched();
            DisplayWeaponChanged(selectedWeapon);
            
            foreach(GameObject go in disableOnClient)
            {
                go.SetActive(false);
            }

        }
        
    }




    
    void Update()
    {
       
        if(!isLocalPlayer)
        {
            return;
        }
        if( isLocalPlayer)
        {
            if (settingsCanvas.GetComponent<SettingsScript>().menuIsOpen == false)
            {
                if (!isDead)
                {
                    LookForWeapon();
                    TryShoot();
                    TryReloading();
                }

                if (isDead)
                {

                }

            }



        }

       

    }

    
    [Command]
    void CmdSelectWeapon(int selectedWeapon)
    {
        if(selectedWeapon == 0)
        {
            RpcSelectWeapon(GetComponent<NetworkIdentity>().netId, 0);
        }

        if (selectedWeapon == 1)
        {
            RpcSelectWeapon(GetComponent<NetworkIdentity>().netId, 1);
        }
        
    }
    [Command]
    void CmdReload(int selectedWeapon)
    {
        
      StartCoroutine(Reload(GetComponent<NetworkIdentity>().netId, selectedWeapon));

    } 

    IEnumerator Reload(uint shooterID, int selectedWeapon)
    {
        if(selectedWeapon == 0)
        {
            RpcReloadPistol(shooterID);
            //NetworkIdentity.spawned[GetComponent<NetworkIdentity>().netId].GetComponent<AllGunsScript>().PlayReloadingPistol(GetComponent<NetworkIdentity>().netId);
            
            Debug.Log("isReloading");
            yield return new WaitForSeconds(pistolReloadTime - .25f);
            yield return new WaitForSeconds(.25f);
            
    
            TargetReloadEnded();
            RpcReloadPistolEnded(shooterID);
            //NetworkIdentity.spawned[GetComponent<NetworkIdentity>().netId].GetComponent<AllGunsScript>().PlayReloadingPistolEnded(GetComponent<NetworkIdentity>().netId);
        }

        if (selectedWeapon == 1)
        {
            //NetworkIdentity.spawned[GetComponent<NetworkIdentity>().netId].GetComponent<AllGunsScript>().PlayReloadingAr(GetComponent<NetworkIdentity>().netId);
            RpcReloadAr(shooterID);
            
            Debug.Log("isReloading");
            yield return new WaitForSeconds(arReloadTime - .25f);
            yield return new WaitForSeconds(.25f);
            arCurrentAmmo = arMaxAmmo;
           
            TargetReloadEnded();
            RpcReloadArEnded(shooterID);
            //NetworkIdentity.spawned[GetComponent<NetworkIdentity>().netId].GetComponent<AllGunsScript>().PlayReloadingArEnded(GetComponent<NetworkIdentity>().netId);
        }




    }
    


    [Command]
    void CmdShootPistol(Vector3 clientCam, Vector3 clientCamPos, int selectedWeapon)
    {
        RpcFiredPistol(GetComponent<NetworkIdentity>().netId);
        RaycastHit hit;
        
        if (Physics.Raycast(clientCamPos, clientCam * 500, out hit))
        {
            Debug.Log(hit.collider.name);

            
            if (hit.collider.CompareTag("Player") && hit.collider.GetComponent<NetworkIdentity>().netId != GetComponent<NetworkIdentity>().netId)
                //verhindert das man sich selbts abschießt
            {
                RpcPlayerFiredEntity(GetComponent<NetworkIdentity>().netId, hit.collider.GetComponent<NetworkIdentity>().netId, hit.point, hit.normal);
                Debug.Log("Spieler getroffen");
                hit.collider.GetComponent<AllGunsScript>().Damage(pistolDamage, GetComponent<NetworkIdentity>().netId);

            }
            else 
            {
                RpcPlayerFired(GetComponent<NetworkIdentity>().netId, hit.point, hit.normal);
                Debug.Log("Schuss abgeben");
            }

           
        }
        TargetShoot();


    }

    [Command]
    void CmdShootAr(Vector3 clientCam, Vector3 clientCamPos, int selectedWeapon)
    {
        RpcFiredAr(GetComponent<NetworkIdentity>().netId);   
        RaycastHit hit;

        if (Physics.Raycast(clientCamPos, clientCam * 500, out hit))
        {
            Debug.Log(hit.collider.name);


            if (hit.collider.CompareTag("Player") && hit.collider.GetComponent<NetworkIdentity>().netId != GetComponent<NetworkIdentity>().netId)
            {
                RpcPlayerFiredEntity(GetComponent<NetworkIdentity>().netId, hit.collider.GetComponent<NetworkIdentity>().netId, hit.point, hit.normal);
                Debug.Log("Spieler getroffen");
                hit.collider.GetComponent<AllGunsScript>().Damage(arDamage, GetComponent<NetworkIdentity>().netId);

            }
            else
            {
                RpcPlayerFired(GetComponent<NetworkIdentity>().netId, hit.point, hit.normal);
                Debug.Log("Schuss abgeben");
            }


        }
        TargetShoot();

    }


    [Client]
    internal void LookForWeapon()
    {
        int previousSelectedWeapon = selectedWeapon;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon >= transform.childCount - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon <= 0)
                selectedWeapon = transform.childCount - 1;

            else
                selectedWeapon--;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
        {
            selectedWeapon = 1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3)
        {
            selectedWeapon = 2;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) && transform.childCount >= 4)
        {
            selectedWeapon = 3;
        }


        if (previousSelectedWeapon != selectedWeapon)
        {
            CmdSelectWeapon(selectedWeapon);
            WeaponSwitched();

        }
    }

    
    
    

    internal void TryShoot()
    {
        if(Input.GetButton("Fire1"))
        {
            if (selectedWeapon == 0 && Time.time >= nexTimeToFire && pistolCurrentAmmo >= 1 && isReloading == false && selectedWeapon == 0)
            {
                pistolCurrentAmmo--;
                nexTimeToFire = Time.time + 1f / pistolFireRate;
                //FindObjectOfType<AudioManager>().PlaySound("deagleShoot");
                audioManager.SendSound("deagleShoot");
                CmdShootPistol(Camera.main.transform.forward, Camera.main.transform.position, selectedWeapon);
                
            }

            if (selectedWeapon == 1 && Time.time >= nexTimeToFire && arCurrentAmmo >= 1 && isReloading == false && selectedWeapon == 1)
            {
                arCurrentAmmo--;
                nexTimeToFire = Time.time + 1f / arFireRate;
          
                CmdShootAr(Camera.main.transform.forward, Camera.main.transform.position, selectedWeapon);
                //FindObjectOfType<AudioManager>().PlaySound("m4Shoot");
                audioManager.SendSound("m4Shoot");

            }
        }
        
       

    }


    public void PlayReloadingPistol(uint shooterID)
    {
        pistolAnim.SetBool("isReloading", true);
    }

    public void PlayReloadingAr(uint shooterID)
    {

        arAnim.SetBool("isReloading", true);
    }

    public void PlayReloadingPistolEnded(uint shooterID)
    {
        pistolAnim.SetBool("isReloading", false);
    }

    public void PlayReloadingArEnded(uint shooterID)
    {

        arAnim.SetBool("isReloading", false);
    }



    public void PlayShootPistol()
    {
        pistolMuzzleflash.Play();
        pistolEjectPS.Play();
        pistolAnim.SetTrigger("triggerShoot");

    }

    public void PlayShootAr()
    {
        arMuzzleflash.Play();
        arEjectPS.Play();
        arAnim.SetTrigger("triggerShoot");

    }





    internal void TryReloading()
    {
        if (Input.GetKeyDown("r") && isReloading == false)
        {
            isReloading = true;
            if(selectedWeapon == 0)
            {
                //FindObjectOfType<AudioManager>().PlaySound("deagleReload");
                audioManager.SendSound("deagleReload");
            }
            if(selectedWeapon == 1)
            {
                //FindObjectOfType<AudioManager>().PlaySound("m4Reload");
                audioManager.SendSound("m4Reload");
            }
            
            CmdReload(selectedWeapon);
            
            
        }

    }


    [ClientRpc]
    void RpcPlayerFired(uint shooterID, Vector3 impactPos, Vector3 impactRot)
    {
        
        Instantiate(bulletHoleFXPrefab, impactPos, Quaternion.LookRotation(impactRot));
        
        
    }

    [ClientRpc]
    void RpcPlayerFiredEntity(uint shooterID, uint targetID, Vector3 impactPos, Vector3 impactRot)
    {
        
        Instantiate(bulletHoleFXPrefab, impactPos, Quaternion.LookRotation(impactRot));

    }
    [ClientRpc]
    void RpcFiredPistol(uint shooterID)
    {
        NetworkIdentity.spawned[shooterID].GetComponent<AllGunsScript>().PlayShootPistol();
       
    }

    [ClientRpc]
    void RpcReloadPistol(uint shooterID)
    {
        NetworkIdentity.spawned[shooterID].GetComponent<AllGunsScript>().PlayReloadingPistol(shooterID);
    }
    [ClientRpc]
    void RpcReloadAr(uint shooterID)
    {
        NetworkIdentity.spawned[shooterID].GetComponent<AllGunsScript>().PlayReloadingAr(shooterID);
    }
    [ClientRpc]
    void RpcReloadPistolEnded(uint shooterID)
    {
        NetworkIdentity.spawned[shooterID].GetComponent<AllGunsScript>().PlayReloadingPistolEnded(shooterID);
    }
    [ClientRpc]
    void RpcReloadArEnded(uint shooterID)
    {
        NetworkIdentity.spawned[shooterID].GetComponent<AllGunsScript>().PlayReloadingArEnded(shooterID);
    }


    [ClientRpc]
    void RpcFiredAr(uint shooterID)
    {
        NetworkIdentity.spawned[shooterID].GetComponent<AllGunsScript>().PlayShootAr();
    }

    [ClientRpc]
    void RpcSelectWeapon(uint shooterID, int currentWeapon)
    {
        NetworkIdentity.spawned[shooterID].GetComponent<AllGunsScript>().DisplayWeaponChanged(currentWeapon);
    }



    [TargetRpc]
    void TargetShoot()
    {
        
        if (selectedWeapon == 0)
        {
            CanvasManager.instance.UpdateCurrentAmmo(pistolCurrentAmmo);
            
        }

        if (selectedWeapon == 1)
        {
            CanvasManager.instance.UpdateCurrentAmmo(arCurrentAmmo);
           
        }

    }
  

    [TargetRpc]

    void TargetReloadEnded()
    {
        if (selectedWeapon == 0)
        {
            isReloading = false;
            pistolCurrentAmmo = pistolMaxAmmo;
            CanvasManager.instance.UpdateCurrentAmmo(pistolCurrentAmmo);
            
        }

        if (selectedWeapon == 1)
        {
            isReloading = false;
            arCurrentAmmo = arMaxAmmo;
            CanvasManager.instance.UpdateCurrentAmmo(arCurrentAmmo);
            
        }

    }

    void DisplayWeaponChanged(int currentWeapon)
    {
        if (currentWeapon == 0)
        {
           
            transform.Find("Arm/WeaponHolder/Pistol").gameObject.SetActive(true);
            transform.Find("Arm/WeaponHolder/AssaultRifle").gameObject.SetActive(false);
            Debug.Log("Switched to Pistol");

        }
        if (currentWeapon == 1)
        {
           
            transform.Find("Arm/WeaponHolder/Pistol").gameObject.SetActive(false);
            transform.Find("Arm/WeaponHolder/AssaultRifle").gameObject.SetActive(true);
            Debug.Log("Switched to Ar");

        }
    }
    
    void WeaponSwitched()
    {
        PlayReloadingPistolEnded(GetComponent<NetworkIdentity>().netId);
        PlayReloadingArEnded(GetComponent<NetworkIdentity>().netId);
        

        if(selectedWeapon == 0)
        {
            CanvasManager.instance.UpdateCurrentAmmo(pistolCurrentAmmo);
            CanvasManager.instance.UpdateMaxAmmo(pistolMaxAmmo);
        }

        if(selectedWeapon == 1)
        {
            CanvasManager.instance.UpdateCurrentAmmo(arCurrentAmmo);
            CanvasManager.instance.UpdateMaxAmmo(arMaxAmmo);
        }
        
    }

    [Server]
    public void Damage(int amount, uint shooterID)
    {
        Health -= amount;
        TargetGotDamage();
        if (Health < 1)
        {
            Die();
            NetworkIdentity.spawned[shooterID].GetComponent<AllGunsScript>().Kills++;
            NetworkIdentity.spawned[shooterID].GetComponent<AllGunsScript>().TargetGotKill();
        }
    }

    [Server]
    public void Die()
    {
        
        Deaths++;
        isDead = true;
        Debug.Log("Player tot");
        TargetDie();
        RpcPlayerDie();
    }
    

    [TargetRpc]
    void TargetDie()
    {
        Cursor.lockState = CursorLockMode.None;
        CanvasManager.instance.ChangePlayerState(false);
        Debug.Log("Du bist geststorben");
    }

    [TargetRpc]
    void TargetRespawn()
    {
        Debug.Log("Respawning");
       
        CanvasManager.instance.ChangePlayerState(true);
        CanvasManager.instance.UpdateHp(Health);
        CanvasManager.instance.UpdateMaxHp(HealthMax);
        transform.position = NetworkManager.singleton.GetStartPosition().position;
        Cursor.lockState = CursorLockMode.Locked;
    }

    [TargetRpc]
    public void TargetGotDamage()
    {
        CanvasManager.instance.UpdateHp(Health);
       
    }
    [TargetRpc]
    public void TargetGotKill()
    {
        Debug.Log("You got a kill");
    }

    [ClientRpc]
    void RpcPlayerRespawn()
    {
        Cursor.lockState = CursorLockMode.Locked;
        GetComponent<Collider>().enabled = true;
        foreach (GameObject item in disableOnDeath)
        {
            item.SetActive(true);
        }
    }

    [ClientRpc]
    void RpcPlayerDie()
    {
        GetComponent<Collider>().enabled = false;
        foreach (GameObject item in disableOnDeath)
        {
            item.SetActive(false);
        }
        

        
    }
    [Command]
    public void CmdRespawn()
    {
        if (isDead)
        {
            Health = HealthMax;
            arCurrentAmmo = arMaxAmmo;
            pistolCurrentAmmo = pistolMaxAmmo;
            isDead = false;
            TargetRespawn();
            RpcPlayerRespawn();
        }
    }

    
}

