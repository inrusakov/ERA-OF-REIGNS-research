using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Скрипт-абстракция Лидера. Это некий интерфейс для взаимодействия с Unity. За характер Лидера отвечает поле Leader leader.
/// </summary>
public class LeaderMonoBehaviour : MonoBehaviour
{
    // Ссылка на главный скрипт.
    public static GameManager GameManager { get => gameManager; set => gameManager = value; }
    private static GameManager gameManager;

    // Лидер.
    public Leader leader;

    // Интерфейс Общения.
    public Canvas dialogUI;
    public TMP_Text leaderNameText;
    public TMP_Text lineText;

    [SerializeField] Button warPeaceButton;
    [SerializeField] TMP_Text warPeaceButtonText;
    [SerializeField] Button specialButton1;
    [SerializeField] Button specialButton2;

    // Интерфейс Хода.
    public Canvas MovingUI;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetLinkToLeaderMonoBehaviourForLeader()
    {
        leader.LeaderMonoBehaviour = this;
    }

    /// <summary>
    /// Действие при нажатии кнопки война/мир.
    /// </summary>
    public void ButtonAction()
    {
        Relationship relationship = gameManager.gameSession.FindRelationship(gameManager.gameSession.CurrentCountry, leader.Country.CountryId);
        // Если страны воюют, то пробуем заключить мир.
        if (relationship.AtWar)
        {
            leader.RequestPeace(gameManager.gameSession.Countries[gameManager.gameSession.CurrentCountry]);
            //relationship.AtWar = false;
            //relationship.NumberOfMoveToUnlockWar = gameManager.gameSession.currentMove + gameManager.gameSession.gameRules.WarDeclarationDelayAfterPeace;
            //OpenDialogUI(leader.peaceAcceptedLine, false);
        }
        // Если страны не воюют, то объявляем войну.
        else
        {
            relationship.AtWar = true;
            relationship.NumberOfMoveToUnlockPeace = gameManager.gameSession.CurrentMove + gameManager.gameSession.GameRules.PeaceNegotiationsDelayAfterWar;
            OpenDialogUI(leader.WarDeclarationToThisLine, false);
        }

        //OpenDialogUI();
        gameManager.districtUI.UpdateIfNeeded();
    }

    public void OpenDialogUI()
    {
        Debug.Log("Открыт диалог Лидера " + leader.LeaderName);

        leaderNameText.text = leader.LeaderName;
        lineText.text = "\"" + leader.DefaultLine + "\"";

        // Проверим, не открыли ли мы окно со своим лидером.
        if (gameManager.gameSession.CurrentCountry == leader.Country.CountryId)
        {
            lineText.text = "Это Вы. Запомните ваш цвет в игре.";
        }
        else
        {
            warPeaceButton.gameObject.SetActive(true);
            warPeaceButton.interactable = true;

            if (specialButton1 != null)
            {
                specialButton1.interactable = true;
                specialButton1.gameObject.SetActive(true);
            }

            if (specialButton2 != null)
            {
                specialButton2.interactable = true;
                specialButton2.gameObject.SetActive(true);
            }

            // Проверим, нужно ли разрешать действия и какой текст повесить на кнопку войны/мира.
            Relationship relationship = gameManager.gameSession.FindRelationship(gameManager.gameSession.CurrentCountry, leader.Country.CountryId);
            // Если идёт война...
            if (relationship.AtWar)
            {
                lineText.text = "\"" + leader.WarLine + "\"";
                warPeaceButtonText.text = "Предложить мир";
                // Проверим, можно ли разрешить попытаться заключить мир.
                if (relationship.NumberOfMoveToUnlockPeace <= gameManager.gameSession.CurrentMove) warPeaceButton.interactable = true;
                else warPeaceButton.interactable = false;
            }
            else
            {
                warPeaceButtonText.text = "Объявить войну";
                // Проверим, можно ли разрешить объявление войны.
                if (relationship.NumberOfMoveToUnlockWar <= gameManager.gameSession.CurrentMove) warPeaceButton.interactable = true;
                else warPeaceButton.interactable = false;
            }
        }

        dialogUI.gameObject.SetActive(true);
        gameManager.CameraController.LockCamera(true);
    }

    public void OpenDialogUI(string line, bool disableButtons)
    {
        OpenDialogUI();

        lineText.text = "\"" + line + "\"";

        if (disableButtons)
        {
            warPeaceButton.gameObject.SetActive(false);
            if (specialButton1 != null) specialButton1.gameObject.SetActive(false);
            if (specialButton2 != null) specialButton2.gameObject.SetActive(false);
        }
    }

    public void CloseDialogUI()
    {
        Debug.Log("Закрыт диалог Лидера " + leader.LeaderName);
        dialogUI.gameObject.SetActive(false);
        //gameManager.CameraController.FreeCamera();
        gameManager.CameraController.LockCamera(false);
    }

    public void SpecialDialogOption1()
    {
        leader.SpecialDialogOption1();
    }
}
