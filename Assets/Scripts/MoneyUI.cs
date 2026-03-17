using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TextMeshPro;
    // Update is called once per frame

    // Update is called once per frame
    void Update()
    {
        m_TextMeshPro.text = $"{GameManager.Instance.PlayerMoney.CurrentCount}";
    }
}
