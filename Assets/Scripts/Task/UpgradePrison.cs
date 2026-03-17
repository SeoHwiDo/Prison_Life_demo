using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePrison : MonoBehaviour
{
    [SerializeField]private PrisonerManager prisonerManager;
    private void OnTriggerEnter(Collider other)
    {
        prisonerManager.ExpandPrison();
    }
}
