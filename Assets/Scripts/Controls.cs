using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
    

    public enum WheelDirection
    {
        Forward,
        Left,
        Right
    }

    [Header("Parameters")]
    [SerializeField] [Min(0)] private int acceleration = 0;
    [SerializeField] [Min(0)] private float minSecondsLastKeyPressed = 0;
    [SerializeField] [Min(0)] private int[] goalSpeeds = null;

    [Header("References")]
    [SerializeField] private Slider steeringWheelSlider = null;
    [SerializeField] private Slider speedLeverSlider = null;
    [SerializeField] private TextMeshProUGUI speedMeterMesh = null;
    [SerializeField] private Button carriageScreenButton = null;
    [SerializeField] private GameObject carriageScreen = null;

    private float speed = 0;
    private float secondsSinceLastKeyPress = 0;
    private bool interactable = false;
    private bool carriageScreenOpen = false;
    private WheelDirection wheelDirection;

    private void Start()
    {
        CarriageManager.Instance.OnRestart += StartControls;
        StopControls();

        carriageScreenButton.onClick.AddListener(ToggleCarriageScreen);
        carriageScreen.SetActive(false);
        carriageScreenOpen = false;
    }

    private void Update()
    {
        secondsSinceLastKeyPress += Time.deltaTime;

        if (!interactable) return;

        HandleInput();

        int goalSpeed = goalSpeeds[4 - Mathf.RoundToInt(speedLeverSlider.value)];
        if(goalSpeed > speed) UpdateSpeed(Mathf.Min(speed + acceleration * Time.deltaTime, goalSpeed));
        else if(goalSpeed < speed) UpdateSpeed(Mathf.Max(speed - acceleration * Time.deltaTime, goalSpeed));

        if (Input.GetKeyDown(KeyCode.E)) ToggleCarriageScreen();
    }

    public void HideControls()
    {
        interactable = false;

        steeringWheelSlider.gameObject.SetActive(false);
        speedLeverSlider.gameObject.SetActive(false);
        speedMeterMesh.transform.parent.gameObject.SetActive(false);
        carriageScreenButton.gameObject.SetActive(false);
    }

    public void ShowAndEnableSteeringWheel()
    {
        interactable = true;

        steeringWheelSlider.value = 1;
        wheelDirection = WheelDirection.Forward;

        steeringWheelSlider.gameObject.SetActive(true);
        steeringWheelSlider.interactable = true;
    }

    public void ShowAndEnableSpeedLever()
    {
        interactable = true;

        speedLeverSlider.value = 0;
        UpdateSpeed(0);

        speedLeverSlider.gameObject.SetActive(true);
        speedMeterMesh.transform.parent.gameObject.SetActive(true);
        speedLeverSlider.interactable = true;
    }

    public void ShowAndEnableCarriageScreenButton()
    {
        interactable = true;

        carriageScreenButton.gameObject.SetActive(true);
        carriageScreenButton.interactable = true;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public WheelDirection GetWheelDirection()
    {
        return wheelDirection;
    }

    public void StartControls()
    {
        ShowAndEnableSteeringWheel();
        ShowAndEnableSpeedLever();
        ShowAndEnableCarriageScreenButton();

        interactable = true;
    }

    public void StopControls()
    {
        interactable = false;
        speedLeverSlider.interactable = false;
        steeringWheelSlider.interactable = false;
        carriageScreenButton.interactable = false;
        UpdateSpeed(0);

        carriageScreen.SetActive(false);
        carriageScreenOpen = false;
    }

    private void HandleInput()
    {
        HandleKeys(KeyCode.LeftArrow, KeyCode.A, () =>
        {
            if(steeringWheelSlider.gameObject.activeSelf)
            {
                steeringWheelSlider.value--;
                secondsSinceLastKeyPress = 0;
            }
        });
        HandleKeys(KeyCode.RightArrow, KeyCode.D, () =>
        {
            if (steeringWheelSlider.gameObject.activeSelf)
            {
                steeringWheelSlider.value++;
                secondsSinceLastKeyPress = 0;
            }
        });
        HandleKeys(KeyCode.UpArrow, KeyCode.W, () =>
        {
            if (speedLeverSlider.gameObject.activeSelf)
            {
                speedLeverSlider.value++;
                secondsSinceLastKeyPress = 0;
            }
        });
        HandleKeys(KeyCode.DownArrow, KeyCode.S, () =>
        {
            if (speedLeverSlider.gameObject.activeSelf)
            {
                speedLeverSlider.value--;
                secondsSinceLastKeyPress = 0;
            }
        });

        UpdateWheelDirection();
    }

    private void HandleKeys(KeyCode key, KeyCode key2, System.Action action)
    {
        bool canHoldKey = secondsSinceLastKeyPress >= minSecondsLastKeyPressed;
        bool keyDown = Input.GetKeyDown(key) || Input.GetKeyDown(key2);
        bool keyHold = Input.GetKey(key) || Input.GetKey(key2);
        if (keyDown || (canHoldKey && keyHold)) action();
    }

    private void ToggleCarriageScreen()
    {
        carriageScreenOpen = !carriageScreenOpen;
        carriageScreen.SetActive(carriageScreenOpen);
    }

    private void UpdateWheelDirection()
    {
        if (steeringWheelSlider.value == 0) wheelDirection = WheelDirection.Left;
        else if (steeringWheelSlider.value == 1) wheelDirection = WheelDirection.Forward;
        else wheelDirection = WheelDirection.Right;
    }

    private void UpdateSpeed(float value)
    {
        speed = value;
        speedMeterMesh.text = $"{ Mathf.RoundToInt(speed) } km/h";
    }
}
