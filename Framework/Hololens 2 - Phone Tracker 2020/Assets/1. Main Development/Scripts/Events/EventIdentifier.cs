using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Phone Controller/Event")]
public class EventIdentifier : ScriptableObject
{
    [SerializeField] short identifier;

    //This should be unique
    public short Id => identifier;
}
