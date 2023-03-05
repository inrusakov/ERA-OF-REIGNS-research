using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс, предназначенный для отображения элемента списка (улучшение района) в UI района.
/// </summary>
public class DistrictProductionItem : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text hintText;
    [SerializeField] TMP_Text costText;

    [SerializeField] Button button;
    [SerializeField] TMP_Text buttonText;

    public Button Button { get => button; set => button = value; }

    public void SetItem(string nameText, string hintText, string costText, string Buttontext, bool lockButton)
    {
        this.nameText.text = nameText;
        this.hintText.text = hintText;
        this.costText.text = costText;
        buttonText.text = Buttontext;

        if (lockButton) button.interactable = false;
        else button.interactable = true;
    }
}
