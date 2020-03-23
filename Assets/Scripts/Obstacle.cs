using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public enum ObstacleType
    {
        LeftTurn,
        RightTurn,
        TrafficLight,
        RailObstacle,
    }

    [Header("Parameters")]
    [SerializeField] [Min(0)] private int minMetersToDisplayNextObstacle = 0;
    [SerializeField] [MinMaxSlider(0, 5000)] private MinMax metersToNextObstacle = new MinMax(0, 5000);

    [Header("References")]
    [SerializeField] private TextMeshProUGUI screenTextMesh = null;
    [SerializeField] private Controls controls = null;

    private bool displayObstacle = false;
    private int maxSpeedToTurn = 0;
    private float metersLeftToNextObstacle = 0;
    private ObstacleType nextObstacle = ObstacleType.RailObstacle;

    public float GetMetersLeftToNextObstacle() => metersLeftToNextObstacle;
    public bool NextObstacleStop() => nextObstacle == ObstacleType.RailObstacle || nextObstacle == ObstacleType.TrafficLight;
    public bool NextObstacleTurn() => nextObstacle == ObstacleType.LeftTurn || nextObstacle == ObstacleType.RightTurn;
    public string GetTurningDirection() => nextObstacle == ObstacleType.LeftTurn ? "left" : nextObstacle == ObstacleType.RightTurn ? "right" : "none";

    private void Start()
    {
        displayObstacle = false;
        CarriageManager.Instance.OnRestart += SetNextObstacle;
    }

    [ContextMenu("Set Next Obstacle")]
    public void SetNextObstacle()
    {
        displayObstacle = true;
        metersLeftToNextObstacle = metersToNextObstacle.GetRandomValue() - 50;
        nextObstacle = (ObstacleType)Random.Range(0, System.Enum.GetValues(typeof(ObstacleType)).Length);
        screenTextMesh.text = "Nothing on sight\nGo straight on";
    }

    public void SetMaxSpeedToTurn(int maxSpeedToTurn)
    {
        this.maxSpeedToTurn = maxSpeedToTurn;
    }

    public void StopDisplayingObstacle()
    {
        displayObstacle = false;
    }

    public void TrainCrashed()
    {
        controls.StopControls();
        CarriageManager.Instance.StopRunning();
        screenTextMesh.text = "The train CRASHED\nPress R to Restart the level";
    }

    private void Update()
    {
        if (!CarriageManager.Instance.IsRunning()) return;

        metersLeftToNextObstacle -= controls.GetSpeed() * Time.deltaTime;
        if (displayObstacle)
        {
            if (metersLeftToNextObstacle <= (minMetersToDisplayNextObstacle - 50)) DisplayNextObstacle();
            else screenTextMesh.text = "Nothing on sight\nGo straight on";
        }
    }

    private void DisplayNextObstacle()
    {
        int metersRoundedTo50s = Mathf.CeilToInt(metersLeftToNextObstacle / 50) * 50;
        switch (nextObstacle)
        {
            case ObstacleType.LeftTurn:
                screenTextMesh.text = $"Left Turn in { metersRoundedTo50s + 50 } meters\nReduce speed to { maxSpeedToTurn - 5 } and turn left";
                break;
            case ObstacleType.RightTurn:
                screenTextMesh.text = $"Right Turn in { metersRoundedTo50s + 50 } meters\nReduce speed to { maxSpeedToTurn - 5 } and turn right";
                break;
            case ObstacleType.TrafficLight:
                screenTextMesh.text = $"RED Traffic Light in { metersRoundedTo50s + 50 } meters\nReduce speed and stop";
                break;
            case ObstacleType.RailObstacle:
                screenTextMesh.text = $"Obstacle on the rail in { metersRoundedTo50s + 50 } meters\nReduce speed and stop";
                break;
            default:
                break;
        }
    }
}
