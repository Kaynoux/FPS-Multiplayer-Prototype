using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using Random = UnityEngine.Random;
using UnityEngine;
using System;
using Mirror;


public class AudioManager : NetworkBehaviour
{
    public float masterVolume;
    public Sound[] sounds;
    private float distance;
    private float volumeDistance;
    private void Awake()
    {
        
        masterVolume = .5f;
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.beginVolume;
            s.source.pitch = s.pitch;

            s.source.loop = s.loop;
        }



    }


    private void Update()
    {
        if(isLocalPlayer)
        {
            if (GameObject.Find("SettingsCanvas").GetComponent<SettingsScript>().menuIsOpen == true)
            {
                masterVolume = GameObject.Find("GameManager").GetComponent<NameTransfer>().masterVolume;
            }

            foreach (Sound s in sounds)
            { 


                s.source.volume = s.beginVolume * masterVolume;
                s.source.pitch = Random.Range(s.pitch - .1f, s.pitch + .1f);

            }
        }
        

        
    }

    public void SendSound(string soundName)
    {
        CmdSendServerSoundID(soundName);
    }

    [Command]
    void CmdSendServerSoundID(string soundName)
    {
        RpcSendSoundToClient(soundName, GetComponent<NetworkIdentity>().netId);
    }

    [ClientRpc]
    void RpcSendSoundToClient(string soundName, uint shooterId)
    {

        foreach (uint Id in NetworkIdentity.spawned.Keys)
        {
            NetworkIdentity.spawned[Id].GetComponent<AudioManager>().PlaySound(soundName, shooterId);
        }


    }


    public void PlaySound(string soundName, uint shooterID)
    {
        if(isLocalPlayer)
        {
            Sound s = Array.Find(sounds, sound => sound.name == soundName);
            if (s == null)
            {
                Debug.LogWarning("Sound; " + soundName + "nicht gefunden");
                return;
            }


            distance = Vector3.Distance(transform.position, NetworkIdentity.spawned[shooterID].transform.position);
            if (distance <= 0)
            {
                volumeDistance = 1f;
            }
            else if (distance >= 50)
            {
                volumeDistance = .2f;
            }
            else if (distance >= 40)
            {
                volumeDistance = .3f;
            }
            else if (distance >= 30)
            {
                volumeDistance = .4f;
            }
            else if (distance >= 20)
            {
                volumeDistance = .5f;
            }
            else if (distance > 10)
            {
                volumeDistance = .7f;
            }
            else if (distance <= 10)
            {
                volumeDistance = .8f;
            }






            s.source.volume = s.source.volume * volumeDistance;
            s.source.Play();
            Debug.Log("player1 " + transform.position + " player2 " + NetworkIdentity.spawned[shooterID].transform.position + " Distance " + volumeDistance + " " + shooterID + " " + NetworkIdentity.spawned[shooterID].name);

        }

    }
}
/*{public float masterVolume;
    public Sound[] sounds;
    private float distance;
    private void Awake()
    {
        /* if (instance == null)
             instance = this;
         else
         {
             Destroy(gameObject);
             return;
         }*/
         /*
        DontDestroyOnLoad(gameObject);
        masterVolume = .5f;
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.beginVolume;
            s.source.pitch = s.pitch;

            s.source.loop = s.loop;
        }



    }


    private void Update()
    {
        if (GameObject.Find("SettingsCanvas").GetComponent<SettingsScript>().menuIsOpen == true)
        {
            masterVolume = GameObject.Find("GameManager").GetComponent<NameTransfer>().masterVolume;
        }

        foreach (Sound s in sounds)
        {


            s.source.volume = s.beginVolume * masterVolume;
            s.source.pitch = Random.Range(s.pitch - .1f, s.pitch + .1f);

        }
    }
    public void PlaySound(string soundName, Transform player1trans, Transform player2trans)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound; " + soundName + "nicht gefunden");
            return;
        }

        
        distance = Vector3.Distance(player1trans.position, player2trans.position);
        //s.source.volume = s.source.volume * distance;
        s.source.Play();

        Debug.Log("player1" + player1trans.position + "player2" + player2trans.position + "Distance" + distance);
        
    }
}
    private Transform player2trans;
    
    
    public void SendSound (string soundName, Transform player1trans)
    {

        
        CmdSendServerSoundID(soundName, player1trans);
        



    }

    [Command]
    void CmdSendServerSoundID(string soundName, Transform player1trans)
    {
        RpcSendSoundToClient(soundName, player1trans);
    }

    [ClientRpc]
    void RpcSendSoundToClient(string soundName, Transform player1trans)
    {
        PlayOnPlayer(soundName, player1trans);
    }

    public void PlayOnPlayer(string soundName, Transform player1trans)
    {
        player2trans = gameObject.transform;
        GameObject.Find("AudioPlayer").GetComponent<AudioPlayer>().PlaySound(soundName, player1trans, player2trans);
    }
}*/
