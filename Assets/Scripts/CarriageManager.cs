using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarriageManager : MonoBehaviour
{
    public static CarriageManager Instance = null;

    public event System.Action OnRestart = null;
    public event System.Action OnOneCarriageLeft = null;

    [Header("Parameters")]
    [SerializeField] [Min(0)] private int medsPrice = 0;
    [SerializeField] [Min(0)] private int startingMoney = 0;
    [SerializeField] [Min(0)] private int startingTotalMeds = 0;
    [SerializeField] [Min(0)] private int startingMedsEffectiveness = 0;
    [SerializeField] [Min(0)] private int startingCarriagePeople = 0;
    [SerializeField] [Min(0)] private int moneyWonPerPerson = 0;
    [SerializeField] [Min(0)] private int carriagePeopleCapacity = 0;
    [SerializeField] [Min(0)] private int secondsToIncreaseMedsEffect = 0;
    [SerializeField] [Min(0)] private int amountToIncreaseMedsEffect = 0;
    [SerializeField] [Min(0)] private int maxMedsEffectiveness = 0;

    [Header("References")]
    [SerializeField] private Transform carriagesParent = null;
    [SerializeField] private Button buyMedsButton = null;
    [SerializeField] private TextMeshProUGUI buyMedsButtonMesh = null;
    [SerializeField] private TextMeshProUGUI totalMedsMesh = null;
    [SerializeField] private TextMeshProUGUI totalPeopleMesh = null;
    [SerializeField] private TextMeshProUGUI totalMoneyMesh = null;

    private bool running = false;
    private int totalMoney = 0;
    private int totalCarriages = 0;
    private int totalMeds = 0;
    private int totalPeople = 0;
    private int medsEffectiveness = 0;
    private float secondsLeftToIncreaseMedsEffect = 0;

    public bool IsRunning() => running;
    public int GetMedsEffectiveness() => medsEffectiveness;
    public int GetStartingCarriagePeople() => startingCarriagePeople;
    public int GetCarriagePeopleCapacity() => carriagePeopleCapacity;
    public int GetTotalPeopleAlive() => totalPeople;
    public int GetMoneyWon() => totalPeople * moneyWonPerPerson;
    public int GetTotalMoney() => totalMoney;

    private void Awake()
    {
        Instance = this;

        People.OnOnePersonKilled += PersonKilled;

        for (int i = 0; i < carriagesParent.childCount; i++)
        {
            Transform dropButton = carriagesParent.GetChild(i).Find("DropButton");

            int index = i;
            dropButton.gameObject.SetActive(i == 0);
            dropButton.GetComponent<Button>().onClick.AddListener(() => DropCarriage(index));
        }
    }

    private void Start()
    {
        running = false;

        totalMoney = startingMoney;
        UpdateTotalMoneyMesh();

        buyMedsButton.onClick.AddListener(BuyMeds);
        buyMedsButtonMesh.text = $"Buy Meds -{ medsPrice } $";
    }

    private void Update()
    {
        if (!running && Input.GetKeyDown(KeyCode.R)) Restart();

        secondsLeftToIncreaseMedsEffect -= Time.deltaTime;
        if (secondsLeftToIncreaseMedsEffect <= 0)
        {
            secondsLeftToIncreaseMedsEffect = secondsToIncreaseMedsEffect;
            IncreaseMedsEffectiveness();
        }
    }

    public void NextLevel()
    {
        totalMoney += moneyWonPerPerson * totalPeople;
        UpdateTotalMoneyMesh();

        Restart();
    }

    public void StopRunning()
    {
        running = false;
    }

    public void DropCarriage(int index)
    {
        totalCarriages--;
        if (totalCarriages == 1) OnOneCarriageLeft?.Invoke();

        HideCarriage(index);
        if (totalCarriages > 1) ShowDropCarriageButton(index + 1);

        int peopleKilled = carriagesParent.GetChild(index).GetComponent<People>().GetAlive();
        totalPeople -= peopleKilled;
        UpdateTotalPeopleMesh();
    }

    public void PersonKilled()
    {
        totalPeople--;
        UpdateTotalPeopleMesh();
    }

    public void IncreaseMedsEffectiveness()
    {
        medsEffectiveness = Mathf.Min(medsEffectiveness + amountToIncreaseMedsEffect, maxMedsEffectiveness);
        UpdateTotalMedsMesh();
    }

    public bool AskForMeds()
    {
        if (totalMeds > 0)
        {
            totalMeds--;
            UpdateTotalMedsMesh();
            return true;
        }
        return false;
    }

    public void Restart()
    {
        running = true;

        totalCarriages = carriagesParent.childCount;

        totalMeds = startingTotalMeds;
        medsEffectiveness = startingMedsEffectiveness;
        UpdateTotalMedsMesh();

        totalPeople = startingCarriagePeople * totalCarriages;
        UpdateTotalPeopleMesh();

        secondsLeftToIncreaseMedsEffect = secondsToIncreaseMedsEffect;

        OnRestart?.Invoke();
    }

    private void BuyMeds()
    {
        if(totalMoney > medsPrice)
        {
            totalMoney -= medsPrice;
            UpdateTotalMoneyMesh();

            totalMeds++;
            UpdateTotalMedsMesh();
        }
    }

    private void HideCarriage(int index)
    {
        foreach (Transform child in carriagesParent.GetChild(index))
        {
            child.gameObject.SetActive(false);
        }
    }

    private void ShowDropCarriageButton(int index)
    {
        carriagesParent.GetChild(index).Find("DropButton").gameObject.SetActive(true);
    }

    private void UpdateTotalMoneyMesh()
    {
        totalMoneyMesh.text = $"{ totalMoney } $";
    }

    private void UpdateTotalMedsMesh()
    {
        totalMedsMesh.text = $"{ totalMeds } Meds { medsEffectiveness }% effective";
    }

    private void UpdateTotalPeopleMesh()
    {
        totalPeopleMesh.text = $"{ totalPeople } People";
    }
}
