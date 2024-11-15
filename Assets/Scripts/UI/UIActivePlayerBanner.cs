using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIActivePlayerBanner : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.OnPlayerActiveUpdated += SetActivePlayer;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnPlayerActiveUpdated -= SetActivePlayer;
    }

    private void SetActivePlayer(PlayerInteractionHandler playerInteractionHandler)
    {
        if (playerInteractionHandler)
        {
            gameObject.SetActive(true);
            transform.position = playerInteractionHandler.transform.position;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
