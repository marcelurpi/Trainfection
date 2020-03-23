using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Level : ScriptableObject
{
    [SerializeField] [Min(0)] private int totalRouteMeters = 0;
    [SerializeField] [Min(0)] private int secondsToOutbreak = 0;
    [SerializeField] [Min(0)] private int secondsToInfectOthers = 0;
    [SerializeField] [Min(0)] private int secondsToKillInfected = 0;
    [SerializeField] private Level nextLevel = null;

    public int GetTotalRouteMeters() => totalRouteMeters;
    public int GetSecondsToOutbreak() => secondsToOutbreak;
    public int GetSecondsToInfectOthers() => secondsToInfectOthers;
    public int GetSecondsToKillInfected() => secondsToKillInfected;
    public Level GetNextLevel() => nextLevel;
}
