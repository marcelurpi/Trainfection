using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaler : MonoBehaviour
{
    [SerializeField] [Range(0.5f, 16f)] private float timeScale = 1;

    private void OnValidate()
    {
        Time.timeScale = timeScale;
    }
}
