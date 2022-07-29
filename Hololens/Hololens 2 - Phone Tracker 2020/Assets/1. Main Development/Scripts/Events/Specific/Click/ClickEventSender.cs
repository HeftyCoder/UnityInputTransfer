using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ClickEventSender : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] EventIdentifier identifier;
    [SerializeField] ClientEventManager eventManager;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked!");
        var clickEvent = new ClickEvent(eventData);
        eventManager.SendEvent(identifier, clickEvent);
    }
}
