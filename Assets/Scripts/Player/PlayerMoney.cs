using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]private ItemStacker moneyStacker;
    public int CurrentMoney => moneyStacker.CurrentCount*10;
    public void SpendMoney(int amount)
    {
        int itemsToRemove = Mathf.CeilToInt(amount / 10f);
        moneyStacker.PopItems(itemsToRemove);
    }
}
