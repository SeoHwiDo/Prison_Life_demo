using ReadOnlyDrawer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private ItemStacker playerMoney;
    [SerializeField] private ItemStacker playerHandCuffs;
    [SerializeField] private ItemStacker playerOre;
    [SerializeField] private EquipmentManager playerEquip;
    [SerializeField] private int maxCapacity = 10;
    [SerializeField] private PrisonerManager prisonerManager;
    public GameObject PlayerPrefab => playerPrefab;
    public ItemStacker PlayerMoney => playerMoney;
    public ItemStacker PlayerHandCuffs => playerHandCuffs;
    public ItemStacker PlayerOre => playerOre;

    public EquipmentManager PlayerEquip => playerEquip;
    public PrisonerManager PrisonerManager => prisonerManager;
    public void SetMaxCapacity(int maxCapacity)
    {
        playerHandCuffs.MaxCapacity = maxCapacity;
        playerMoney.MaxCapacity = maxCapacity;
        playerOre.MaxCapacity = maxCapacity;
    }
    private void Awake()
    {   

        // 인스턴스가 이미 있는지 확인 후 처리
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않음
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
        }
        SetMaxCapacity(10);
    }

}
