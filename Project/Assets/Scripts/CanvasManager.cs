using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager instance;
    public GameObject uiAlive;
    public GameObject uiDead;
    public AllGunsScript allGunsScript;
    public Text textCurrentAmmo;
    public Text textMaxAmmo;
    public Text textHealthAmount;
    public Slider sliderHealthAmount;
    
    public TextMeshProUGUI textKills;
    public TextMeshProUGUI textDeaths;


    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ChangePlayerState(bool isAlive)
    {
        uiAlive.SetActive(isAlive);
        uiDead.SetActive(!isAlive);
        if(!isAlive)
        {
            textKills.text = "Kills: " + allGunsScript.Kills;
            textDeaths.text = "Deaths: " + allGunsScript.Deaths;
            
        }
    }

    public void UpdateHp(int currentHp)
    {
        sliderHealthAmount.value = currentHp;
        textHealthAmount.text = "" + currentHp;
    }

    public void UpdateMaxHp(int maxHP)
    {
        sliderHealthAmount.maxValue = maxHP;
        textHealthAmount.text = "" + maxHP;
    }

    public void UpdateMaxAmmo(int maxAmmo)
    {
        textMaxAmmo.text = "" + maxAmmo;
    }

    public void UpdateCurrentAmmo(int currentAmmo)
    {
        textCurrentAmmo.text = " " + currentAmmo;
    }

    public void HideUI()
    {
        uiAlive.SetActive(false);
        uiDead.SetActive(false);
    }

    public void RespawnButton()
    {
        if (allGunsScript != null)
            allGunsScript.CmdRespawn();
    }
   
}
