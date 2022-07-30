using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Barebones.Networking;
public class ClickEventSender : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] EventIdentifier identifier;
    [SerializeField] ClientEventManager eventManager;
    [SerializeField] DeliveryMethod deliveryMethod;
    public void OnPointerClick(PointerEventData eventData)
    {
        var clickEvent = new ClickEvent(eventData);
        eventManager.Send(identifier, clickEvent);
    }
}
