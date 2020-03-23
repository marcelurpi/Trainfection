using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovePeople : MonoBehaviour
{
    private static MovePeople selected = null;

    [Header("Parameters")]
    [SerializeField] private Color buttonSelectedColor = Color.white;

    [Header("References")]
    [SerializeField] private Button button = null;
    [SerializeField] private Image buttonImage = null;
    [SerializeField] private People people = null;

    private Color baseButtonColor = Color.white;

    private void Awake()
    {
        people.OnZeroAlive += HideMoveButton;
        people.OnZeroAlive += Deselect;
        people.OnMoreThanZeroAlive += ShowMoveButton;
    }

    private void Start()
    {
        CarriageManager.Instance.OnOneCarriageLeft += HideMoveButton;

        button.onClick.AddListener(MoveButtonPressed);
        baseButtonColor = buttonImage.color;
    }

    private void HideMoveButton()
    {
        button.gameObject.SetActive(false);
    }

    private void ShowMoveButton()
    {
        button.gameObject.SetActive(true);
    }

    private void MoveButtonPressed()
    {
        if (selected == null) Select();
        else if (selected == this) Deselect();
        else if (selected.people.AnyAlive() && people.AnySpaceLeft()) MovePerson();
    }

    private void Select()
    {
        selected = this;
        buttonImage.color = buttonSelectedColor;
        HideAllMoveButtonsExceptAdjacent();
    }

    private void Deselect()
    {
        selected = null;
        buttonImage.color = baseButtonColor;
        ShowAllMoveButtons();
    }

    private void MovePerson()
    {
        bool isInfected = selected.people.RemovePerson();
        people.AddPerson(isInfected);
    }

    private void HideAllMoveButtonsExceptAdjacent()
    {
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            bool adjacent = Mathf.Abs(transform.GetSiblingIndex() - i) <= 1;

            MovePeople people = transform.parent.GetChild(i).GetComponent<MovePeople>();
            people.button.gameObject.SetActive(adjacent);
        }
    }

    private void ShowAllMoveButtons()
    {
        foreach (Transform child in transform.parent)
        {
            MovePeople movePeople = child.GetComponent<MovePeople>();

            bool active = movePeople.people.AnyAlive();
            movePeople.button.gameObject.SetActive(active);
        }
    }
}
