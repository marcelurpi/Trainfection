using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MovementForward : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private Level firstLevel = null;

    [Header("References")]
    [SerializeField] private Controls controls = null;
    [SerializeField] private Obstacle obstacle = null;
    [SerializeField] private Outbreak outbreak = null;
    [SerializeField] private TextMeshProUGUI metersLeftMesh = null;
    [SerializeField] private TextMeshProUGUI screenTextMesh = null;

    private bool reached = false;
    private float metersToDestination = 0;
    private Level currentLevel = null;

    private void Awake()
    {
        currentLevel = firstLevel;
    }

    private void Start()
    {
        outbreak.SetParameters(currentLevel);

        CarriageManager.Instance.OnRestart += ResetRoute;
        ResetRoute();
    }

    private void Update()
    {
        if (reached && currentLevel.GetNextLevel() != null && Input.GetKeyDown(KeyCode.N))
        {
            currentLevel = currentLevel.GetNextLevel();

            outbreak.SetParameters(currentLevel);
            CarriageManager.Instance.NextLevel();
        }

        if (!CarriageManager.Instance.IsRunning()) return;

        metersToDestination -= controls.GetSpeed() * Time.deltaTime;
        if (metersToDestination <= 0)
        {
            metersToDestination = 0;

            reached = true;
            controls.StopControls();
            obstacle.StopDisplayingObstacle();

            string score = $"Destination Reached and gained { CarriageManager.Instance.GetMoneyWon() } $";
            if (currentLevel.GetNextLevel() != null) screenTextMesh.text = $"{ score }\nPress N to Start Next Level";
            else screenTextMesh.text = $"Congrats you finished the game!\nScore: { CarriageManager.Instance.GetTotalMoney() } $";
        }
        UpdateMetersLeftMesh();
    }

    private void ResetRoute()
    {
        reached = false;
        metersToDestination = currentLevel.GetTotalRouteMeters();
        UpdateMetersLeftMesh();
    }

    private void UpdateMetersLeftMesh()
    {
        int kmToDestination = Mathf.RoundToInt(metersToDestination / 100) / 10;
        int decimalKm = Mathf.RoundToInt(metersToDestination / 100) % 10;
        metersLeftMesh.text = $"{ kmToDestination }.{ decimalKm } km to destination";
    }
}
