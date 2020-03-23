using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MovementStop : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] [Min(0)] private int maxMetersFromObstacleToWait = 0;
    [SerializeField] [MinMaxSlider(0, 10)] private MinMax secondsToWaitStopped = new MinMax(0, 10);

    [Header("References")]
    [SerializeField] private TextMeshProUGUI screenTextMesh = null;
    [SerializeField] private Obstacle obstacle = null;
    [SerializeField] private Controls controls = null;

    private bool stopped = false;
    private float secondsLeftToWaitStopped = 0;

    private void Start()
    {
        CarriageManager.Instance.OnRestart += ResetStop;
        ResetStop();
    }

    private void Update()
    {
        if (!CarriageManager.Instance.IsRunning()) return;

        if (stopped) HandleStopping();
        else
        {
            bool obstacleClose = obstacle.GetMetersLeftToNextObstacle() <= maxMetersFromObstacleToWait;
            if (obstacleClose && obstacle.NextObstacleStop()) CheckStopping();
        }
    }

    private void CheckStopping()
    {
        if (controls.GetSpeed() > 0)
        {
            if (obstacle.GetMetersLeftToNextObstacle() <= 0) obstacle.TrainCrashed();
            return;
        }

        stopped = true;
        obstacle.StopDisplayingObstacle();
        secondsLeftToWaitStopped = secondsToWaitStopped.GetRandomValue();
        screenTextMesh.text = "Wait until you can continue";
    }

    private void HandleStopping()
    {
        if (controls.GetSpeed() > 0) obstacle.TrainCrashed();
        else
        {
            secondsLeftToWaitStopped -= Time.deltaTime;
            if (secondsLeftToWaitStopped <= 0)
            {
                ResetStop();
                obstacle.SetNextObstacle();
            }
        }
    }

    private void ResetStop()
    {
        stopped = false;
    }
}
