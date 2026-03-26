using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrisonNumberUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TextMeshPro;
    // Update is called once per frame
   

    private void Update()
    {
        var manager = GameManager.Instance.PrisonerManager;
        m_TextMeshPro.text = $"{manager.CurrentPrisonerCount} / {manager.MaxCapacity}";
    }
}
