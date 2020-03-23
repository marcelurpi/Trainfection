using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MovementTurn : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] [Min(0)] private int maxSpeedToTurn = 0;
    [SerializeField] [Min(0)] private int maxMetersFromObstacleToTurn = 0;
    [SerializeField] [Min(0)] private int maxSecondsTurningOutsideTurn = 0;
    [SerializeField] [MinMaxSlider(0, 500)] private MinMax metersToTurn = new MinMax(0, 500);

    [Header("References")]
    [SerializeField] private TextMeshProUGUI screenTextMesh = null;
    [SerializeField] private Obstacle obstacle = null;
    [SerializeField] private Controls controls = null;

    private bool turning = false;
    private bool turningOutsideTurn = false;
    private float metersLeftToTurn = 0;
    private float secondsLeftTurningOutsideTurn = 0;

    private void Start()
    {
        CarriageManager.Instance.OnRestart += ResetTurn;
        ResetTurn();

        obstacle.SetMaxSpeedToTurn(maxSpeedToTurn);
    }

    private void Update()
    {
        if (!CarriageManager.Instance.IsRunning()) return;

        if (turning) HandleTurning();
        else if (turningOutsideTurn) HandleTurningOutsideTurn();
        else
        {
            bool wheelTurned = controls.GetWheelDirection() != Controls.WheelDirection.Forward;
            if (controls.GetSpeed() > 0 && wheelTurned) CheckTurning();
        }
    }

    private void CheckTurning()
    {
        if(IsTurningOutsideTurn())
        {
            turningOutsideTurn = true;
            secondsLeftTurningOutsideTurn = maxSecondsTurningOutsideTurn;
        }
        else if (obstacle.GetMetersLeftToNextObstacle() <= 0) CheckEncounteringTurn();
    }

    private void CheckEncounteringTurn()
    {
        if (!CorrectWheelDirection() || controls.GetSpeed() > maxSpeedToTurn) obstacle.TrainCrashed();
        else
        {
            turning = true;
            turningOutsideTurn = false;
            obstacle.StopDisplayingObstacle();
            metersLeftToTurn = metersToTurn.GetRandomValue();
            screenTextMesh.text = $"Keep turning { obstacle.GetTurningDirection() }";
        }
    }

    private void HandleTurning()
    {
        if (!CorrectWheelDirection() || controls.GetSpeed() > maxSpeedToTurn) obstacle.TrainCrashed();
        else
        {
            metersLeftToTurn -= controls.GetSpeed() * Time.deltaTime;
            if (metersLeftToTurn <= 0)
            {
                ResetTurn();
                obstacle.SetNextObstacle();
            }
        }
    }

    private void HandleTurningOutsideTurn()
    {
        if (!IsTurningOutsideTurn()) turningOutsideTurn = false;
        else
        {
            secondsLeftTurningOutsideTurn -= Time.deltaTime;
            if (secondsLeftTurningOutsideTurn <= 0) obstacle.TrainCrashed();
        }
    }

    private bool IsTurningOutsideTurn()
    {
        bool wheelTurned = controls.GetWheelDirection() != Controls.WheelDirection.Forward;
        bool isTurning = controls.GetSpeed() > 0 && wheelTurned;
        bool farFromObstacle = obstacle.GetMetersLeftToNextObstacle() > maxMetersFromObstacleToTurn;
        bool closeButNotEncounter = !farFromObstacle && obstacle.GetMetersLeftToNextObstacle() > 0;
        bool closeButWrongDir = closeButNotEncounter && !CorrectWheelDirection();
        return isTurning && (farFromObstacle || closeButWrongDir);
    }

    private bool CorrectWheelDirection()
    {
        string turningDirection = obstacle.GetTurningDirection();
        Controls.WheelDirection wheelDirection = controls.GetWheelDirection();
        bool wrongLeft = turningDirection == "left" && wheelDirection != Controls.WheelDirection.Left;
        bool wrongRight = turningDirection == "right" && wheelDirection != Controls.WheelDirection.Right;
        return !wrongLeft && !wrongRight;
    }

    private void ResetTurn()
    {
        turning = false;
        turningOutsideTurn = false;
    }
}
