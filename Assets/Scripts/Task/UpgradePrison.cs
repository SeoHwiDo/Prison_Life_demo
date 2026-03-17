using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePrison : MonoBehaviour
{
    [SerializeField]private PrisonerManager prisonerManager;
    [SerializeField]private int price;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")&&GameManager.Instance.PlayerMoney.CurrentCount * 10 > price)
        {
            GameManager.Instance.PlayerMoney.PopItems(price/10);
            prisonerManager.ExpandPrison();
            prisonerManager.SetMaxCapacity(prisonerManager.MaxCapacity*prisonerManager.ExpandLevel);
        }
    }
}
