using System;
using UnityEngine;

[Serializable]
public struct MinMax
{
    [SerializeField] private float min;
    [SerializeField] private float max;

    public float GetRandomValue() => UnityEngine.Random.Range(min, max);

    public MinMax(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}