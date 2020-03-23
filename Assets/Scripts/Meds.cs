using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Meds : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button sendButton = null;
    [SerializeField] private TextMeshProUGUI textMesh = null;

    private int meds = 0;

    public bool AnyLeft() => meds > 0;

    private void Start()
    {
        meds = 0;
        UpdateTextMesh();

        sendButton.onClick.AddListener(Send);
    }

    public bool Use()
    {
        meds--;
        UpdateTextMesh();
        bool effective = Random.Range(0, 100) < CarriageManager.Instance.GetMedsEffectiveness();
        return effective;
    }

    private void Send()
    {
        if(CarriageManager.Instance.AskForMeds())
        {
            meds++;
            UpdateTextMesh();
        }
    }

    private void UpdateTextMesh()
    {
        textMesh.text = $"{ meds } Meds";
    }
}
