using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using Random = UnityEngine.Random;
public class AudioPlayer : MonoBehaviour
{
    public float masterVolume;
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
