using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс, предназначенный для управления интерфейсом внутриигровой справки.
/// </summary>
public class HelpUI : MonoBehaviour
{
    [SerializeField] GameObject helpUI;

    [SerializeField] List<Button> buttons;
    [SerializeField] List<GameObject> helpSections;

    public void Open()
    {
        helpUI.SetActive(true);
    }

    public void Close()
    {
        helpUI.SetActive(false);
    }

    public void ChangeSections(int sectionId)
    {
        // Заблокируем кнопку выбранного раздела.
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].interactable = true;
            helpSections[i].SetActive(false);

            if (i == sectionId)
            {
                buttons[i].interactable = false;
                helpSections[i].SetActive(true);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            int x = i;
            buttons[i].onClick.AddListener(() => ChangeSections(x));
        }

        ChangeSections(0);
    }
}
