using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    public bool menuIsOpen = false;
    public bool isInLockedScene = false;
    public GameObject settingsGameObject;
    
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        settingsGameObject.SetActive(false);
    }

     


    // Update is called once per frame
    void Update()
    {
        
        
        if (Input.GetKeyDown("m") && menuIsOpen == false)
        {
            if (Cursor.lockState == CursorLockMode.None)
            {
                isInLockedScene = false;
            }
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                isInLockedScene = true;
                Cursor.lockState = CursorLockMode.None;
                

            }
            
            settingsGameObject.SetActive(true);
            menuIsOpen = true;
            return;
        }
        if (Input.GetKeyDown("m") && menuIsOpen == true)
        {
            settingsGameObject.SetActive(false);
            menuIsOpen = false;
            if(isInLockedScene == true)
            {
                Cursor.lockState = CursorLockMode.Locked;
                
            }
        }
        
    }

    
}
