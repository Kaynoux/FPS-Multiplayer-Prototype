using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class NameTransfer : MonoBehaviour
{
    public string playerName;
    public TMP_InputField inputField;
    public TMP_Text textName;
    public Animator animGameMenu;
    public Animator animNameMenu;
    public GameObject nameMenu;
    

    public float masterVolume;
    public Slider volumeSlider;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void StoreName()
    {
        animGameMenu.SetTrigger("triggerPlayGameMenu");
        animNameMenu.SetTrigger("triggerHideNameMenu");
        playerName = inputField.text;
        textName.SetText("Dein Name ist  " + playerName);
        Debug.Log(playerName + " ist beigetreten");
        nameMenu.SetActive(false);
        
        
    }

    public void StoreVolume()
    {
        if(volumeSlider.value == 0)
        {
            masterVolume = 0.01f;
            
        }
        else
        {
            masterVolume = volumeSlider.value;
        }
        
        Debug.Log("MasterVolume changed" + masterVolume);
    }

}
