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
    private int maxOutbreaksAtOnce = 0;
    private int secondsToInfectOtherPassengers = 0;
    private int secondsToInfectAdjacentCarriages = 0;
    private int secondsToKillInfected = 0;

    private float secondsLeftToOutbreak = 0;
    private float secondsLeftToInfectAdjacentCarriages = 0;

    public int GetSecondsToInfectOthers() => secondsToInfectOtherPassengers;
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

        int currentOutbreaksAtOnce = GetCurrentOutbreaksAtOnce();
        if (currentOutbreaksAtOnce < maxOutbreaksAtOnce) HandleOutbreak();

        if (currentOutbreaksAtOnce > 0) HandleInfectAdjacentCarriages();
        else secondsLeftToInfectAdjacentCarriages = secondsToInfectAdjacentCarriages;
    }

    public void SetParameters(Level level)
    {
        secondsToOutbreak = level.GetSecondsToOutbreak();
        maxOutbreaksAtOnce = level.GetMaxOutbreaksAtOnce();
        secondsToInfectOtherPassengers = level.GetSecondsToInfectOtherPassengers();
        secondsToInfectAdjacentCarriages = level.GetSecondsToInfectAdjacentCarriages();
        secondsToKillInfected = level.GetSecondsToKillInfected();

        secondsLeftToOutbreak = secondsToOutbreak;
        secondsLeftToInfectAdjacentCarriages = secondsToInfectAdjacentCarriages;
    }

    private void HandleOutbreak()
    {
        secondsLeftToOutbreak -= Time.deltaTime;
        if (secondsLeftToOutbreak <= 0)
        {
            secondsLeftToOutbreak = secondsToOutbreak;

            InfectRandomCarriage();
        }
    }

    private void HandleInfectAdjacentCarriages()
    {
        secondsLeftToInfectAdjacentCarriages -= Time.deltaTime;
        if(secondsLeftToInfectAdjacentCarriages <= 0)
        {
            secondsLeftToInfectAdjacentCarriages = secondsToInfectAdjacentCarriages;

            if (!InfectAdjacentCarriage()) secondsLeftToInfectAdjacentCarriages = -1;
        }
    }

    private int GetCurrentOutbreaksAtOnce()
    {
        int currentOutbreaks = 0;
        for (int i = 0; i < carriagesParent.childCount; i++)
        {
            if (carriagesParent.GetChild(i).GetComponent<People>().AnyInfected()) currentOutbreaks++;
        }
        return currentOutbreaks;
    }

    private void InfectRandomCarriage()
    {
        List<People> infectable = new List<People>();
        for (int i = 0; i < carriagesParent.childCount; i++)
        {
            People carriagePeople = carriagesParent.GetChild(i).GetComponent<People>();
            if (carriagePeople.AnyToInfect() && !carriagePeople.AnyInfected()) infectable.Add(carriagePeople);
        }
        if (infectable.Count > 0) infectable[Random.Range(0, infectable.Count)].InfectPerson();
    }

    private bool InfectAdjacentCarriage()
    {
        List<People> adjacent = new List<People>();

        Carriage[] carriages = new Carriage[carriagesParent.childCount];
        People[] people = new People[carriagesParent.childCount];
        for (int i = 0; i < carriagesParent.childCount; i++)
        {
            Transform carriage = carriagesParent.GetChild(i);
            carriages[i] = carriage.GetComponent<Carriage>();
            people[i] = carriage.GetComponent<People>();
        }

        for (int i = 0; i < carriagesParent.childCount; i++)
        {
            bool previousInfect = i - 1 >= 0 && carriages[i - 1].IsUnlocked() && people[i - 1].AnyInfected();
            bool nextInfect = i + 1 < carriagesParent.childCount && carriages[i + 1].IsUnlocked() && people[i + 1].AnyInfected();
            bool canBeInfected = carriages[i].IsUnlocked() && people[i].AnyToInfect() && !people[i].AnyInfected();
            if ((previousInfect || nextInfect) && canBeInfected) adjacent.Add(people[i]);
        }

        if (adjacent.Count > 0)
        {
            adjacent[Random.Range(0, adjacent.Count)].InfectPerson();
            return true;
        }
        return false;
    }
}
