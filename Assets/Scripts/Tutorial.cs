using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] [Min(0)] private int charsPerSec = 0;

    [Header("References")]
    [SerializeField] private Controls controls = null;
    [SerializeField] private TextMeshProUGUI screenTextMesh = null;
    [SerializeField] private Coroutine coroutine;

    bool skip = false;
    bool showAllText = false;

    private void Start()
    {
        skip = false;
        controls.HideControls();
        StartCoroutine(TutorialCoroutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) showAllText = true;
        if (Input.GetKeyDown(KeyCode.S))
        {
            showAllText = true;
            skip = true;
        }
    }

    private IEnumerator TutorialCoroutine()
    {
        controls.HideControls();
        if (!skip) yield return ShowTextCoroutine("Welcome to the train!\nPress S to skip the tutorial");
        if (!skip) yield return new WaitForSeconds(2);
        if (!skip) yield return ShowTextCoroutine("First we should learn how to drive the train");
        if (!skip) yield return new WaitForSeconds(1);

        if (!skip) controls.ShowAndEnableSpeedLever();
        if (!skip) yield return ShowTextCoroutine("This is the speed lever");
        if (!skip) yield return new WaitForSeconds(0.5f);
        if (!skip) yield return ShowTextCoroutine("You can move it with the mouse,\nthe up and down arrows or the W and S Keys");
        if (!skip) yield return new WaitForSeconds(6);

        if (!skip) controls.HideControls();
        if (!skip) controls.ShowAndEnableSteeringWheel();
        if (!skip) yield return ShowTextCoroutine("This is the steering wheel");
        if (!skip) yield return new WaitForSeconds(0.5f);
        if (!skip) yield return ShowTextCoroutine("You can move it with the mouse,\nthe left and right arrows or the A and D Keys");
        if (!skip) yield return new WaitForSeconds(6);

        if (!skip) controls.HideControls();
        if (!skip) controls.ShowAndEnableCarriageScreenButton();
        if (!skip) yield return ShowTextCoroutine("This is the carriage screen");
        if (!skip) yield return new WaitForSeconds(0.5f);
        if (!skip) yield return ShowTextCoroutine("You can open it with the mouse or the E key");
        if (!skip) yield return new WaitForSeconds(2);

        controls.HideControls();
        if (!skip) yield return ShowTextCoroutine("In this screen you can see the state of all carriages in the train");
        if (!skip) yield return new WaitForSeconds(2);
        if (!skip) yield return ShowTextCoroutine("On the top left you have the number\nof (alive) people you carry");
        if (!skip) yield return new WaitForSeconds(2);
        if (!skip) yield return ShowTextCoroutine("The more you carry to their destination, the more money you'll gain");
        if (!skip) yield return new WaitForSeconds(2);
        if (!skip) yield return ShowTextCoroutine("Next to that you have the amount\n of meds you are carrying");
        if (!skip) yield return new WaitForSeconds(2);
        if (!skip) yield return ShowTextCoroutine("We hope nothing happens\nbut you can buy more if needed");
        if (!skip) yield return new WaitForSeconds(2);
        if (!skip) yield return ShowTextCoroutine("Also note that they\nare not 100% effective to everybody");
        if (!skip) yield return new WaitForSeconds(2);
        if (!skip) yield return ShowTextCoroutine("But research keeps increasing\ntheir effectiveness over time");
        if (!skip) yield return new WaitForSeconds(2);
        if (!skip) yield return ShowTextCoroutine("At the top right we have\nour current amount of money");
        if (!skip) yield return new WaitForSeconds(2);
        if (!skip) yield return ShowTextCoroutine("Always save some,\nyou never know when you'll need it");
        if (!skip) yield return new WaitForSeconds(2);
        if (!skip) yield return ShowTextCoroutine("You can move people to adjacent carriages\nor send meds to take care of them");
        if (!skip) yield return new WaitForSeconds(2);
        if (!skip) yield return ShowTextCoroutine("Finally we have the ability to\ndrop the whole carriage");
        if (!skip) yield return new WaitForSeconds(2);
        if (!skip) yield return ShowTextCoroutine("Only Recommended for EMERGENCIES!");
        if (!skip) yield return new WaitForSeconds(2);

        if (!skip) yield return ShowTextCoroutine("Try to reach the destination\nwithout any train crashes");
        if (!skip) yield return new WaitForSeconds(2);
        if (!skip) yield return ShowTextCoroutine("And don't forget about the people you carry!");
        if (!skip) yield return new WaitForSeconds(2);
        if (!skip) yield return ShowTextCoroutine("Stay Safe and Good Luck!");
        if (!skip) yield return new WaitForSeconds(1);

        yield return ShowTextCoroutine("");
        screenTextMesh.maxVisibleCharacters = 200;
        CarriageManager.Instance.Restart();
    }

    private IEnumerator ShowTextCoroutine(string text)
    {
        screenTextMesh.maxVisibleCharacters = 0;
        screenTextMesh.text = text;
        float secsPerChar = 1f / charsPerSec;
        showAllText = false;
        while (screenTextMesh.maxVisibleCharacters < text.Length)
        {
            if (showAllText)
            {
                screenTextMesh.maxVisibleCharacters = text.Length;
                break;
            }
            yield return new WaitForSeconds(secsPerChar);
            screenTextMesh.maxVisibleCharacters++;
        }
    }
}
