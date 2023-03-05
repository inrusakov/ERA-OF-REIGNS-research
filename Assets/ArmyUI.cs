using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс, предназначенный для управления UI Армии.
/// </summary>
public class ArmyUI : MonoBehaviour
{
    [SerializeField] GameObject ContentForUnits;

    [SerializeField] Button button;
    [SerializeField] TMP_Text buttonText;

    [SerializeField] GameObject warriorPrefab;
    [SerializeField] GameObject enhancedWarrior;
    [SerializeField] GameObject horsemanPrefab;
    [SerializeField] GameObject agentPrefab;

    [SerializeField] TMP_Text leaderNameText;
    [SerializeField] TMP_Text capacityText;
    [SerializeField] TMP_Text powerText;

    // 0 - Воин, 1 - усил. воин, 2- всадник, 3 - агент.
    [SerializeField] List<Button> buttonsForBuying;
    [SerializeField] List<TMP_Text> buttonForBuyingTexts;
    [SerializeField] GameObject store;

    private Country country;
    // 0 - нет боя (можно уволнять и покупать юнитов), 1 - идёт бой (можно выбрать юнита), 2 - идёт бой (нельзя выбрать юнита).
    private int state;

    private bool isDefender;
    private FightManager fightManager;

    public Country Country { get => country; set => country = value; }
    public FightManager FightManager { get => fightManager; set => fightManager = value; }
    public bool IsDefender { get => isDefender; set => isDefender = value; }

    /// <summary>
    /// Настроить панель Армии под лидера.
    /// </summary>
    /// <param name="country">Страна Лидера, для которой нужно настроить отображение.</param>
    /// <param name="state">Состояние панели (0 - нет боя (можно уволнять и покупать юнитов),
    /// 1 - идёт бой (можно выбрать юнита), 2 - идёт бой (нельзя выбрать юнита)).
    /// </param>
    public void Open(Country country, int state)
    {
        this.country = country;
        this.state = state;

        if (state != 0)
        {
            button.gameObject.SetActive(false);
            CloseStore();
        }
        else
        {
            button.gameObject.SetActive(true);
        }

        leaderNameText.text = country.Leader.LeaderName;
        leaderNameText.color = country.GetCountryColor();

        UpdateList();
    }

    public void Open(Country country, int state, FightManager fightManager, bool isDefender)
    {
        this.FightManager = fightManager;
        this.IsDefender = isDefender;

        Open(country, state);
    }

    public void UpdateList()
    {
        LeaderMonoBehaviour.GameManager.UpdateStats();

        Army army = Country.Army;

        capacityText.text = army.FilledCapacity + "/" + Country.CounterOfDistrictsEverHelded;
        powerText.text = army.Power.ToString();

        // Очистим лист.
        ClearList();

        // Заполним лист.
        for (int i = 0; i < army.Units.Count; i++)
        {
            UnitUI unitItem;
            switch (army.Units[i].Type)
            {
                case 0:
                    unitItem = Instantiate(warriorPrefab, ContentForUnits.transform).GetComponent<UnitUI>();
                    break;
                case 1:
                    unitItem = Instantiate(enhancedWarrior, ContentForUnits.transform).GetComponent<UnitUI>();
                    break;
                case 2:
                    unitItem = Instantiate(horsemanPrefab, ContentForUnits.transform).GetComponent<UnitUI>();
                    break;
                case 3:
                    unitItem = Instantiate(agentPrefab, ContentForUnits.transform).GetComponent<UnitUI>();
                    break;
                default:
                    unitItem = null;
                    Debug.LogError("Новый тип юнита не добавлен!");
                    break;
            }

            unitItem.Create(army.Units[i], this, state);
        }

        // Теперь проверим, каких юнитов можно купить и отобразим это.
        GameRules gameRules = LeaderMonoBehaviour.GameManager.gameSession.GameRules;
        for (int i = 0; i < 4; i++)
        {
            if (country.Army.CheckIfBuyngPossible(i, Country))
            {
                // То можно.
                buttonForBuyingTexts[i].text = "Нанять";
                buttonsForBuying[i].interactable = true;
            }
            else
            {
                // То нельзя.
                buttonsForBuying[i].interactable = false;

                if (!(gameRules.CapacityRequirements[i] + country.Army.FilledCapacity <= country.CounterOfDistrictsEverHelded))
                {
                    buttonForBuyingTexts[i].text = "Нет места!";
                }
                else
                {
                    buttonForBuyingTexts[i].text = "Не хватает ресурсов!";
                }
            }
        }
    }

    public void Close()
    {
        CloseStore();
        // Очистим лист.
        // ClearList();
        // Destroy(gameObject);
    }

    private void ClearList()
    {
        UnitUI[] childs = gameObject.GetComponentsInChildren<UnitUI>();
        for (int i = 0; i < childs.Length; i++)
        {
            Destroy(childs[i].gameObject);
        }
    }

    private void AddUnitToToTheArmy(Unit unit)
    {
        Country.Army.AddNewUnit(unit, country);
        UpdateList();
        //LeaderMonoBehaviour.GameManager.UpdateStats();
    }

    public void BuyUnitWarrior()
    {
        GameRules gameRules = LeaderMonoBehaviour.GameManager.gameSession.GameRules;
        //Unit unit = new Unit("Воин", 0, false, gameRules.Damages[0], gameRules.Healths[0]);
        Unit unit = country.Army.GetUnit(0, false);

        AddUnitToToTheArmy(unit);
    }

    public void BuyUnitCoolerWarrior()
    {
        GameRules gameRules = LeaderMonoBehaviour.GameManager.gameSession.GameRules;
        //Unit unit = new Unit("Усиленный воин", 1, false, gameRules.Damages[1], gameRules.Healths[1]);
        Unit unit = country.Army.GetUnit(1, false);

        AddUnitToToTheArmy(unit);
    }

    public void BuyUnitHorseman()
    {
        GameRules gameRules = LeaderMonoBehaviour.GameManager.gameSession.GameRules;
        //Unit unit = new Unit("Всадник", 2, false, gameRules.Damages[2], gameRules.Healths[2]);
        Unit unit = country.Army.GetUnit(2, false);

        AddUnitToToTheArmy(unit);
    }

    public void BuyUnitAgent()
    {
        GameRules gameRules = LeaderMonoBehaviour.GameManager.gameSession.GameRules;
        //Unit unit = new Unit("Агент Тайной Полиции", 3, false, gameRules.Damages[3], gameRules.Healths[3]);
        Unit unit = country.Army.GetUnit(3, false);

        AddUnitToToTheArmy(unit);
    }

    public void OpenStore()
    {
        store.SetActive(true);
    }

    public void CloseStore()
    {
        store.SetActive(false);
    }
}
