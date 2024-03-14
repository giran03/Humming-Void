using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graffiti_Interaction : MonoBehaviour, IInteractable
{
    PlayerInteractionHandler playerInteractionHandler;
    private void Awake()
    {
        var tempObj = GameObject.FindGameObjectWithTag("Player");
        playerInteractionHandler = tempObj.transform.parent.GetComponent<PlayerInteractionHandler>();
    }
    public void Interact()
    {
        AudioManager.Instance.PlaySFX("Graffiti_Erase", transform.position);
        playerInteractionHandler.graffitiCount++;
        
        PlayerPrefs.SetInt("graffitiCount", playerInteractionHandler.graffitiCount); // save graffiti collected to player prefs
        
        var graffiti = gameObject;
        Destroy(graffiti);
    }
}
