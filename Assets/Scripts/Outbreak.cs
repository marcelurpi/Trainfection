using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Outbreak : MonoBehaviour
{
    public static Outbreak Instance = null;

    [Header("Parameters")]
    [SerializeField] private int secondsToCureInfected = 0;

    [Header("References")]
    [SerializeField] private Obstacle obstacle = null;
    [SerializeField] private Transform carriagesParent = null;
    [SerializeField] private TextMeshProUGUI screenTextMesh = null;

    private int secondsToOutbreak = 0;
    private int secondsToInfectOthers = 0;
    private int secondsToKillInfected = 0;

    private float secondsLeftToOutbreak = 0;

    public int GetSecondsToInfectOthers() => secondsToInfectOthers;
    public int GetSecondsToCureInfected() => secondsToCureInfected;
    public int GetSecondsToKillInfected() => secondsToKillInfected;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!CarriageManager.Instance.IsRunning()) return;

        if (CarriageManager.Instance.GetTotalPeopleAlive() == 0)
        {
            obstacle.TrainCrashed();
            screenTextMesh.text = $"Everybody is DEAD in the train\nPress R to Restart the level";
        }

        HandleOutbreak();
    }

    public void SetParameters(Level level)
    {
        secondsToOutbreak = level.GetSecondsToOutbreak();
        secondsToInfectOthers = level.GetSecondsToInfectOthers();
        secondsToKillInfected = level.GetSecondsToKillInfected();

        secondsLeftToOutbreak = secondsToOutbreak;
    }

    private void HandleOutbreak()
    {
        secondsLeftToOutbreak -= Time.deltaTime;
        if (secondsLeftToOutbreak > 0) return;

        secondsLeftToOutbreak = secondsToOutbreak;

        bool infected = false;
        int maxTries = 10;
        while(!infected && maxTries > 0)
        {
            int randomChildIndex = Random.Range(0, carriagesParent.childCount);
            People carriagePeople = carriagesParent.GetChild(randomChildIndex).GetComponent<People>();
            if (carriagePeople.AnyToInfect()) 
            {
                carriagePeople.InfectPerson();
                infected = true;
            }
            maxTries--;
        }
    }
}
