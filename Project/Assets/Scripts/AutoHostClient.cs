using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class AutoHostClient : MonoBehaviour
{
   public TMP_InputField ipInputfield; 
   public NetworkManager networkManager;
   public string networkAdress = "localhost";
   public VideoPlayer introVideo;
   public bool isPlayerStarted = false;

    void Update()
    {
      if (isPlayerStarted == false && introVideo.isPlaying == true)
        {
            isPlayerStarted = true;
        }
      if (isPlayerStarted == true && introVideo.isPlaying == false)
        {
            introVideo.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if(!Application.isBatchMode)
        {
            Debug.Log("Client connected");
            
        }
        else
        {
            Debug.Log("Server is starting");
        }
    }
    public void JoinLocal()
    {
        
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
    }

    public void JoinIP()
    {
        networkAdress = ipInputfield.text;
        networkManager.networkAddress = networkAdress;
        networkManager.StartClient();
    }

    public void doExitGame()
    {
        Application.Quit();
    }

    

   
}
