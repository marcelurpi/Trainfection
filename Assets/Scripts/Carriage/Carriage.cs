using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Carriage : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private People people = null;
    [SerializeField] private Meds meds = null;
    [SerializeField] private MovePeople movePeople = null;
    [SerializeField] private Button dropButton = null;
    [SerializeField] private Button lockButton = null;
    [SerializeField] private GameObject lockOutline = null;

    private bool locked = false;

    public bool IsUnlocked() => !locked;

    private void Start()
    {
        dropButton.gameObject.SetActive(transform.GetSiblingIndex() == 0);
        dropButton.onClick.AddListener(DropCarriage);

        locked = false;
        lockButton.onClick.AddListener(ToggleLockCarriage);
    }

    private void DropCarriage()
    {
        gameObject.SetActive(false);
        CarriageManager.Instance.CarriageDropped(people.GetAlive());

        if (transform.GetSiblingIndex() + 1 < transform.parent.childCount)
        {
            Transform nextChild = transform.parent.GetChild(transform.GetSiblingIndex() + 1);
            nextChild.GetComponent<Carriage>().dropButton.gameObject.SetActive(true);
        }
    }

    private void ToggleLockCarriage()
    {
        locked = !locked;
        lockOutline.gameObject.SetActive(locked);

        if (locked)
        {
            meds.HideSendButton();
            movePeople.HideMoveButton();
        }
        else
        {
            meds.ShowSendButton();
            movePeople.ShowMoveButton();
        }
    }
}
