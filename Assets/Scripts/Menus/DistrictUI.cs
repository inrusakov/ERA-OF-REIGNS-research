using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс, предназначенный для управления UI Района.
/// </summary>
public class DistrictUI : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    int buttonAction;
    DistrictInfo district;

    [SerializeField] TMP_Text districtName;
    [SerializeField] TMP_Text leaderName;
    [SerializeField] TMP_Text bonusName;
    [SerializeField] TMP_Text hint;
    [SerializeField] TMP_Text hintForButtonText;

    public Image bonusImage;
    public Button button;
    public TMP_Text buttonText;

    public List<Sprite> bonusImages;

    public RectTransform panel;

    [SerializeField] List<DistrictProductionItem> districtProductionItems;

    public void Open(DistrictInfo district)
    {
        panel.gameObject.SetActive(true);
        button.interactable = true;
        buttonAction = 0;
        this.district = district;
        hintForButtonText.text = "";

        // Отобразим имя Лидера-владельца.
        ShowLeader();

        // Отобразим Бонус Района и подсказку по нему.
        ShowDistrictBonus();

        // Установим для кнопки текст.
        ShowButton();


        // Разберёмся с листом улучшений.
        ShowDistrictProductions();
    }

    public void Close()
    {
        this.district = null;
        panel.gameObject.SetActive(false);
    }

    public void UpdateIfNeeded()
    {
        if (district != null)
        {
            Open(district);
        }
    }

    public void LockButton(string text, string hintText)
    {
        buttonText.text = text;
        buttonAction = 0;
        button.interactable = false;
        hintForButtonText.text = hintText;
    }

    public void UnlockButton(string text, string hintText)
    {
        buttonText.text = text;
        button.interactable = true;
        hintForButtonText.text = hintText;
    }

    public void MakeButtonAction()
    {
        switch (buttonAction)
        {
            // Объявить Район своим.
            case 1:
                gameManager.gameSession.Countries[gameManager.gameSession.CurrentCountry].AddNewDistrict(district.districtFunc, true);
                ++gameManager.gameSession.ConqueredNeutralDistrict;
                // Обновим эту панель.
                Open(district);
                break;
            // Начать бой за Район.
            case 2:
                gameManager.FightManager.StartBattle(gameManager.gameSession.Countries[gameManager.gameSession.CurrentCountry],
                    district.holder, district);
                break;
            // Покинуть Район.
            case 3:
                gameManager.gameSession.Countries[gameManager.gameSession.CurrentCountry].RemoveDistrict(district.districtFunc, true, null);
                // Обновим эту панель.
                Open(district);
                break;
            // Ничего не делаем.
            default:
                break;
        }

        gameManager.UpdateStats();
    }

    void ShowLeader()
    {
        districtName.text = district.DistrictName;
        if (district.holder != null)
        {
            leaderName.text = district.holder.Leader.LeaderName;
            leaderName.color = district.holder.GetCountryColor();
        }
        else
        {
            leaderName.text = "Отсутствует";
            leaderName.color = Color.black;
        }
    }

    void ShowDistrictBonus()
    {
        bool hintNeeded = true;
        switch (district.DistrictBonus)
        {
            case 1:
                bonusName.text = "   золото";
                bonusImage.sprite = bonusImages[1];
                break;
            case 2:
                bonusName.text = "   железо";
                bonusImage.sprite = bonusImages[2];
                break;
            case 3:
                bonusName.text = "животноводство";
                bonusImage.sprite = bonusImages[3];
                break;
            case 4:
                bonusName.text = "очки победы";
                bonusImage.sprite = bonusImages[4];
                hint.text = $"Даёт {gameManager.gameSession.GameRules.VictoryPointsBonus} Очков Победы за захват. " +
                    $"При утере этого Района, {gameManager.gameSession.GameRules.VictoryPointsBonus} Очков Победы также будут утеряны.";
                hintNeeded = false;
                break;
            default:
                bonusName.text = "Отсутствует";
                bonusImage.sprite = bonusImages[0];
                hint.text = "Этот Район не обладает особыми качествами...";
                hintNeeded = false;
                break;
        }
        if (hintNeeded)
        {
            if (district.HasBonusProduction)
            {
                hint.text = "Активен!";
            }
            else
            {
                hint.text = "Не активен! Необходимо построить улучшение \"Разработка Бонуса Района\", чтобы активировать его...";
            }
        }
    }

    void ShowButton()
    {
        // Если этот Район - нейтральный.
        if (district.holder == null)
        {
            // Проверим, соседний ли он.
            if (district.districtFunc.CheckForBeingNearCountry(gameManager.gameSession.Countries[gameManager.gameSession.CurrentCountry]))
            {
                // Проверим, Игровые Правила на возможность.
                if (gameManager.gameSession.GameRules.NumberOfAllowedDistrictsToConquerPerMove > gameManager.gameSession.ConqueredNeutralDistrict)
                {
                    UnlockButton("Объявить Район своим", "Заявить, что отныне этот нейтральный Район принадлежит вам. " +
                        "Другим Лидерам придётся объявить вам войну, если они захотят завладеть этим Районом.");
                    buttonAction = 1;
                }
                else
                {
                    LockButton("Объявить Район своим", "Вы больше не можете совершать подобные действия на этом ходу!");
                }
            }
            else
            {
                LockButton("Объявить Район своим", "Район не является соседним!");
            }
        }
        // Если это Район совершающей сейчас ход Страны:
        else if (gameManager.gameSession.CurrentCountry == district.holder.CountryId)
        {
            // Проверим, последний ли он.
            if (district.holder.DistrictsIds.Count == 1)
            {
                LockButton("Покинуть Район", "Нельзя покинуть последний Район в стране!");
            }
            else
            {
                UnlockButton("Покинуть Район", "Заявить, что больше не владеете этим Районом. После этого, он станет нейтральным.");
                buttonAction = 3;
            }
        }
        // Если это Район не принадлежит совершающей сейчас ход Стране:
        else if (gameManager.gameSession.CurrentCountry != district.holder.CountryId)
        {
            // Проверим, соседний ли он.
            if (district.districtFunc.CheckForBeingNearCountry(gameManager.gameSession.Countries[gameManager.gameSession.CurrentCountry]))
            {
                Relationship relationship = gameManager.gameSession.FindRelationship(gameManager.gameSession.CurrentCountry, district.holder.CountryId);
                // Проверим в состоянии войны ли эти Страны.
                if (relationship.AtWar)
                {
                    UnlockButton("Начать бой за Район", "Напасть на этот Район. В случае победы, вы станете его владельцем.");
                    buttonAction = 2;
                }
                else
                {
                    LockButton("Начать бой за Район", "Чтобы атаковать этот Район, вы должны объявить войну его владельцу!");
                }
            }
            else
            {
                LockButton("Начать бой за Район", "Район не является соседним!");
            }
        }
    }

    string buttonForDistrictProductionItemText;
    bool lockButtonForDistrictProductionItem;
    void ShowDistrictProductions()
    {
        for (int i = 0; i < districtProductionItems.Count; i++)
        {
            districtProductionItems[i].gameObject.SetActive(true);
        }
        // Если Район не принадлежит открывшему Лидеру, заблокируем функции кнопок и просто покажем постройки.
        if (district.holder == null || gameManager.gameSession.CurrentCountry != district.holder.CountryId)
        {
            if (district.HasBonusProduction) districtProductionItems[0].SetItem("Разработка Бонуса Района", "", "", "Построено", true);
            else districtProductionItems[0].gameObject.SetActive(false);

            if (district.HasGoldProduction) districtProductionItems[1].SetItem("Добыча Золота", "", "", "Построено", true);
            else districtProductionItems[1].gameObject.SetActive(false);

            if (district.HasSecurityBureau) districtProductionItems[2].SetItem("Бюро Безопасности", "", "", "Построено", true); 
            else districtProductionItems[2].gameObject.SetActive(false); 

            if (district.HasMonument) districtProductionItems[3].SetItem("Монумент", "", "", "Построено", true);
            else districtProductionItems[3].gameObject.SetActive(false);
        }
        else
        {
            buttonForDistrictProductionItemText = "";
            lockButtonForDistrictProductionItem = false;
            // Сначала обработаем улучшение "Разработка Бонуса Района".
            // Если нет Бонуса или если это Очки Победы, это улучшение не нужно показывать.
            if (district.DistrictBonus == 0 || district.DistrictBonus == 4)
            {
                districtProductionItems[0].gameObject.SetActive(false);
            }
            else
            {
                districtProductionItems[0].gameObject.SetActive(true);
                string productionName = "Разработка Бонуса Района";
                string cost = $"Цена: {-gameManager.gameSession.GameRules.CostOfBonusProduction} Золота";

                if (district.HasBonusProduction)
                {
                    buttonForDistrictProductionItemText = "Построено";
                    lockButtonForDistrictProductionItem = true;

                }
                else
                {
                    // Проверим возможность купить.
                    CheckBuyingPossibility(gameManager.gameSession.GameRules.CostOfBonusProduction);

                }

                switch (district.DistrictBonus)
                {
                    // Золото.
                    case 1:
                        districtProductionItems[0].SetItem(productionName + " (Золото)",
                            $"Даёт {gameManager.gameSession.GameRules.GoldBonus} Золота за ход",
                            cost, buttonForDistrictProductionItemText, lockButtonForDistrictProductionItem);
                        break;
                    // Железо.
                    case 2:
                        districtProductionItems[0].SetItem(productionName + " (Железо)",
                            $"Даёт {gameManager.gameSession.GameRules.IronBonus} Железа за ход",
                            cost, buttonForDistrictProductionItemText, lockButtonForDistrictProductionItem);
                        break;
                    // Животноводство.
                    case 3:
                        districtProductionItems[0].SetItem(productionName + " (Животноводство)",
                            $"Даёт {gameManager.gameSession.GameRules.HorsesBonus} Животноводства за ход",
                            cost, buttonForDistrictProductionItemText, lockButtonForDistrictProductionItem);
                        break;
                    default:
                        break;
                }
                districtProductionItems[0].Button.onClick.RemoveAllListeners();
                districtProductionItems[0].Button.onClick.AddListener(district.BuildBonusProduction);
            }


            // Теперь с улучшением "Добыча Золота".
            if (district.HasGoldProduction)
            {
                buttonForDistrictProductionItemText = "Построено";
                lockButtonForDistrictProductionItem = true;

                districtProductionItems[1].SetItem("Добыча Золота",
                                $"Даёт {gameManager.gameSession.GameRules.GoldProductionBonus} Золота за ход",
                                $"Цена: {-gameManager.gameSession.GameRules.CostOfGoldProduction} Золота",
                                buttonForDistrictProductionItemText,
                                lockButtonForDistrictProductionItem);
            }
            else
            {
                CheckBuyingPossibility(gameManager.gameSession.GameRules.CostOfGoldProduction);
                districtProductionItems[1].SetItem("Добыча Золота",
                                $"Даёт {gameManager.gameSession.GameRules.GoldProductionBonus} Золота за ход",
                                $"Цена: {-gameManager.gameSession.GameRules.CostOfGoldProduction} Золота",
                                buttonForDistrictProductionItemText,
                                lockButtonForDistrictProductionItem);
                districtProductionItems[1].Button.onClick.RemoveAllListeners();
                districtProductionItems[1].Button.onClick.AddListener(district.BuildGoldProduction);
            }

            // Теперь с улучшением "Бюро Безопасности".
            if (district.HasSecurityBureau)
            {
                buttonForDistrictProductionItemText = "Построено";
                lockButtonForDistrictProductionItem = true;

                districtProductionItems[2].SetItem("Бюро Безопасности",
                                $"Увеличивает вместительность на {gameManager.gameSession.GameRules.AgentsProductionBonus} для юнитов типа \"Тайная Полиция\"",
                                $"Цена: {-gameManager.gameSession.GameRules.CostOfAgentsProduction} Золота",
                                buttonForDistrictProductionItemText,
                                lockButtonForDistrictProductionItem);
            }
            else
            {
                CheckBuyingPossibility(gameManager.gameSession.GameRules.CostOfAgentsProduction);
                districtProductionItems[2].SetItem("Бюро Безопасности",
                                $"Увеличивает вместительность на {gameManager.gameSession.GameRules.AgentsProductionBonus} для юнитов типа \"Тайная Полиция\"",
                                $"Цена: {-gameManager.gameSession.GameRules.CostOfAgentsProduction} Золота",
                                buttonForDistrictProductionItemText,
                                lockButtonForDistrictProductionItem);
                districtProductionItems[2].Button.onClick.RemoveAllListeners();
                districtProductionItems[2].Button.onClick.AddListener(district.BuildSecurityBureau);
            }

            // Теперь с улучшением "Монумент".
            if (district.HasMonument)
            {
                buttonForDistrictProductionItemText = "Построено";
                lockButtonForDistrictProductionItem = true;

                districtProductionItems[3].SetItem("Монумент",
                                $"Даёт {gameManager.gameSession.GameRules.MonumentBonus} Очков Победы за постройку. При утере Района, эти Очки победы будут изъяты, а улучшение уничтожено.",
                                $"Цена: {-gameManager.gameSession.GameRules.CostOfMonument} Золота",
                                buttonForDistrictProductionItemText,
                                lockButtonForDistrictProductionItem);

            }
            else
            {
                CheckBuyingPossibility(gameManager.gameSession.GameRules.CostOfMonument);
                districtProductionItems[3].SetItem("Монумент",
                                $"Даёт {gameManager.gameSession.GameRules.MonumentBonus} Очков Победы за постройку. При утере Района, эти Очки победы будут изъяты, а улучшение уничтожено.",
                                $"Цена: {-gameManager.gameSession.GameRules.CostOfMonument} Золота",
                                buttonForDistrictProductionItemText,
                                lockButtonForDistrictProductionItem);
                districtProductionItems[3].Button.onClick.RemoveAllListeners();
                districtProductionItems[3].Button.onClick.AddListener(district.BuildMonument);
            }
        }
    }

    void CheckBuyingPossibility(int cost)
    {
        // Проверим возможность купить. Если можем:
        if (gameManager.gameSession.Countries[gameManager.gameSession.CurrentCountry].Gold + cost >= 0)
        {
            buttonForDistrictProductionItemText = "Построить";
            lockButtonForDistrictProductionItem = false;
        }
        else
        {
            buttonForDistrictProductionItemText = "Не хватает!";
            lockButtonForDistrictProductionItem = true;
        }
    }
}
