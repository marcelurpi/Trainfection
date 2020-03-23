using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class People : MonoBehaviour
{
    public static event System.Action OnOnePersonKilled = null;

    public event System.Action OnZeroAlive = null;
    public event System.Action OnMoreThanZeroAlive = null;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI aliveTextMesh = null;
    [SerializeField] private Image infectedImage = null;
    [SerializeField] private Image deadImage = null;
    [SerializeField] private Meds meds = null;

    private int alive = 0;
    private int infected = 0;
    private int dead = 0;
    private int capacity = 0;
    private float secondsToCureInfected = 0;
    private float secondsToInfectOthers = 0;
    private float secondsToKillInfected = 0;

    public int GetAlive() => alive;
    public bool AnyAlive() => alive > 0;
    public bool AnyToInfect() => alive > infected;
    public bool AnySpaceLeft() => alive + dead < capacity;

    private void Start()
    {
        alive = CarriageManager.Instance.GetStartingCarriagePeople();
        infected = 0;
        dead = 0;
        capacity = CarriageManager.Instance.GetCarriagePeopleCapacity();

        UpdateAliveTextMesh();
        UpdateInfectedImageFill();
        UpdateDeadImageFill();

        secondsToCureInfected = -1;
        secondsToInfectOthers = -1;
        secondsToKillInfected = -1;
    }

    private void Update()
    {
        HandleCureInfected();
        HandleInfectOthers();
        HandleKillInfected();
    }

    public void InfectPerson()
    {
        infected++;
        UpdateInfectedImageFill();
    }

    public void AddPerson(bool isInfected)
    {
        IncrementAlive();
        if (isInfected) infected++;

        UpdateAliveTextMesh();
        UpdateInfectedImageFill();
        UpdateDeadImageFill();
    }

    public bool RemovePerson()
    {
        DecrementAlive();

        bool isInfected = Random.Range(0, capacity) < infected;
        if (isInfected) DecrementInfected();

        UpdateAliveTextMesh();
        UpdateInfectedImageFill();
        UpdateDeadImageFill();

        return isInfected;
    }

    private void HandleCureInfected()
    {
        if (secondsToCureInfected == -1)
        {
            if (infected > 0 && meds.AnyLeft())
            {
                secondsToCureInfected = Outbreak.Instance.GetSecondsToCureInfected();
            }
            else return;
        }

        secondsToCureInfected -= Time.deltaTime;
        if (secondsToCureInfected < 0)
        {
            CureInfectedPerson();
            secondsToCureInfected = -1;
        }
    }

    private void HandleInfectOthers()
    {
        if (secondsToInfectOthers == -1)
        {
            if (infected > 0 && alive > infected)
            {
                secondsToInfectOthers = Outbreak.Instance.GetSecondsToInfectOthers();
            }
            else return;
        }

        if (alive == infected)
        {
            secondsToInfectOthers = -1;
            return;
        }

        secondsToInfectOthers -= Time.deltaTime;
        if (secondsToInfectOthers < 0)
        {
            InfectPerson();
            secondsToInfectOthers = -1;
        }
    }

    private void HandleKillInfected()
    {
        if (secondsToKillInfected == -1)
        {
            if (infected > 0)
            {
                secondsToKillInfected = Outbreak.Instance.GetSecondsToKillInfected();
            }
            else return;
        }

        secondsToKillInfected -= Time.deltaTime;
        if (secondsToKillInfected < 0)
        {
            KillInfectedPerson();
            secondsToKillInfected = -1;
        }
    }

    private void CureInfectedPerson()
    {
        bool effective = meds.Use();
        if (effective)
        {
            DecrementInfected();
            UpdateInfectedImageFill();
        }
    }

    private void KillInfectedPerson()
    {
        DecrementAlive();
        DecrementInfected();
        dead++;

        UpdateAliveTextMesh();
        UpdateInfectedImageFill();
        UpdateDeadImageFill();

        OnOnePersonKilled?.Invoke();
    }

    private void IncrementAlive()
    {
        alive++;
        if (alive == 1) OnMoreThanZeroAlive?.Invoke();
    }

    private void DecrementAlive()
    {
        alive--;
        if (alive == 0) OnZeroAlive?.Invoke();
    }

    private void DecrementInfected()
    {
        infected--;
        if (infected == 0)
        {
            secondsToInfectOthers = -1;
            secondsToKillInfected = -1;
        }
    }

    private void UpdateAliveTextMesh()
    {
        aliveTextMesh.text = $"{ alive }/{ capacity }";
    }

    private void UpdateInfectedImageFill()
    {
        infectedImage.fillAmount = (float)(infected + dead) / (alive + dead);
    }
    private void UpdateDeadImageFill()
    {
        deadImage.fillAmount = (float)dead / (alive + dead);
    }
}
