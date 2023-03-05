using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс, обеспечивающий ход сражения между Лидерами за Район.
/// </summary>
public class FightManager : MonoBehaviour
{
    // Переменные UI.
    [SerializeField] GameObject fightUI;
    [SerializeField] TMP_Text districtAtStakeText;
    [SerializeField] TMP_Text admitDefeatHintText;
    [SerializeField] TMP_Text readyToFightHintText;

    [SerializeField] GameObject defendingUnitPlacement;
    [SerializeField] GameObject attackingUnitPlacement;

    [SerializeField] Button buttonReadyToFight;
    [SerializeField] Button buttonAdmitDefeat;

    [SerializeField] ArmyUI defenderArmyUI;
    [SerializeField] ArmyUI attackerArmyUI;

    // Экран окончания.
    [SerializeField] GameObject battleOverScreen;
    [SerializeField] TMP_Text whoWonHintText;

    // Поля.
    Country attackingCountry;
    Country defendingCountry;
    DistrictInfo districtAtStake;

    UnitUI attackingUnit = null;
    UnitUI defendingUnit = null;

    /// <summary>
    /// Предпринимаем действия, когда какая-нибудь страна выставила юнита для боя.
    /// </summary>
    /// <param name="byDefender">Защита выставила юнита?</param>
    /// <param name="unitUIOfChoise">Юнит, которого выбрала страна.</param>
    public void UnitChoosed(bool byDefender, UnitUI unitUIOfChoise)
    {
        Debug.Log("Юнит выбран!");
        Debug.LogWarning("Армия атаки состоит из " + attackingCountry.Army.Units.Count + " юнитов");
        Debug.LogWarning("Армия защиты состоит из " + defendingCountry.Army.Units.Count + " юнитов");

        if (byDefender)
        {
            // Избавляемся от родителя у unitUIOfChoise, перемещаем в место.
            unitUIOfChoise.gameObject.transform.SetParent(fightUI.transform);
            unitUIOfChoise.gameObject.transform.position = defendingUnitPlacement.transform.position;
            defendingUnit = unitUIOfChoise;

            // Если это был игрок, блокируем выбор ещё одного юнита.
            if (defendingCountry.Leader.IsPlayer)
            {
                defenderArmyUI.Open(defendingCountry, 2, this, true);
                // Если ии ещё не атаковал, заставляем.
                if (attackingUnit == null)
                {
                    MakeAILeaderToChooseUnit();
                    return;
                }
            }
        }
        else
        {
            // Избавляемся от родителя у unitUIOfChoise, перемещаем в место.
            unitUIOfChoise.gameObject.transform.SetParent(fightUI.transform);
            unitUIOfChoise.gameObject.transform.position = attackingUnitPlacement.transform.position;
            attackingUnit = unitUIOfChoise;

            // Если это был игрок, блокируем выбор ещё одного юнита.
            if (attackingCountry.Leader.IsPlayer)
            {
                attackerArmyUI.Open(attackingCountry, 2, this, false);
                // Если ии ещё не атаковал, заставляем.
                if (defendingUnit == null)
                {
                    MakeAILeaderToChooseUnit();
                    return;
                }
            }
        }

        // Проверяем, оба ли юнита готовы к бою.
        if (attackingUnit != null && defendingUnit != null)
        {
            // Начинаем бой между юнитами.
            StartFightBetweenUnits();
        }
    }

    /// <summary>
    /// Начинает бой между двумя выставленными юнитами.
    /// </summary>
    private void StartFightBetweenUnits()
    {
        int damageOfDefender = defendingUnit.Unit.Damage;
        int damageOfAttacker = attackingUnit.Unit.Damage;

        // Наносим урон атакующему юниту.
        // Если юнит погибает, убиваем его.
        if (attackingUnit.Unit.TakeDamage(damageOfDefender))
        {
            Debug.Log("Юнит погибает у атакующего!");
            //attackingCountry.Army.Units.Remove(attackingUnit.Unit);
            attackingCountry.Army.KillUnit(attackingUnit.Unit, attackingCountry);
            attackingUnit.Kill();
        }
        // иначе возвращаем в список (удаляем UnitUI, он заново создастся при обновлении списка).
        else
        {
            Destroy(attackingUnit.gameObject, 1);
        }
        attackingUnit = null;

        // Наносим урон защищающему юниту.
        // Если юнит погибает, убиваем его.
        if (defendingUnit.Unit.TakeDamage(damageOfAttacker))
        {
            Debug.Log("Юнит погибает у защищающегося!");
            //defendingCountry.Army.Units.Remove(attackingUnit.Unit);
            defendingCountry.Army.KillUnit(defendingUnit.Unit, defendingCountry);
            defendingUnit.Kill();
        }
        // иначе возвращаем в список (удаляем UnitUI, он заново создастся при обновлении списка).
        else
        {
            Destroy(defendingUnit.gameObject, 1);
        }
        defendingUnit = null;

        //Debug.LogWarning("Армия атаки состоит из " + attackingCountry.Army.Units.Count + " юнитов");
        //Debug.LogWarning("Армия защиты состоит из " + defendingCountry.Army.Units.Count + " юнитов");

        UpdatePanelsAfterFight();

        // Проверяем, нет ли проигрыша. 
        // Обновляем панели, если нет.
        if (!CheckIfSomebodyIsDefeated()) MakeAILeaderToChooseUnit();
    }

    /// <summary>
    /// Обновляет Панели Армий после сражения.
    /// </summary>
    public void UpdatePanelsAfterFight()
    {
        ArmyUI playersArmyUI;
        ArmyUI computerLeaderArmyUI;
        Country playerCountry;
        Country computerLeaderCountry;

        if (attackingCountry.Leader.IsPlayer)
        {
            playersArmyUI = attackerArmyUI;
            playerCountry = attackingCountry;

            computerLeaderArmyUI = defenderArmyUI;
            computerLeaderCountry = defendingCountry;
        }
        else
        {
            playersArmyUI = defenderArmyUI;
            playerCountry = defendingCountry;

            computerLeaderArmyUI = attackerArmyUI;
            computerLeaderCountry = attackingCountry;
        }

        computerLeaderArmyUI.Open(computerLeaderCountry, 2, this, computerLeaderArmyUI.IsDefender);
        playersArmyUI.Open(playerCountry, 1, this, playersArmyUI.IsDefender);
    }

    /// <summary>
    /// Просит ИИ-Лидера выбрать юнита для атаки из своей Армии.
    /// </summary>
    public void MakeAILeaderToChooseUnit()
    {
        Unit unit;
        if (defendingCountry.Leader.IsPlayer)
        {
            unit = attackingCountry.Leader.ChooseUnitToFight();
            // Ищем юнита в потомках и выбираем.
            FindUnitUIinChildrenOf(attackerArmyUI.gameObject, unit);
        }
        else
        {
            unit = defendingCountry.Leader.ChooseUnitToFight();
            // Ищем юнита в потомках и выбираем.
            FindUnitUIinChildrenOf(defenderArmyUI.gameObject, unit);
        }
    }

    private void FindUnitUIinChildrenOf(GameObject armyUI, Unit unit)
    {
        UnitUI[] childs = armyUI.GetComponentsInChildren<UnitUI>();
        for (int i = 0; i < childs.Length; i++)
        {
            if (childs[i].Unit == unit)
            {
                if (defendingCountry.Leader.IsPlayer) UnitChoosed(false, childs[i]);
                else UnitChoosed(true, childs[i]);
                return;
            }
        }
    }

    /// <summary>
    /// Открывает интерфейс боя, настривает менеджер боя.
    /// </summary>
    /// <param name="attackingCountry">Атакующая страна.</param>
    /// <param name="defendingCountry">Защищающаяся страна</param>
    /// <param name="districtAtStake">Район, за который идёт бой.</param>
    public void StartBattle(Country attackingCountry, Country defendingCountry, DistrictInfo districtAtStake)
    {
        buttonReadyToFight.gameObject.SetActive(true);
        readyToFightHintText.gameObject.SetActive(true);

        districtAtStakeText.text = districtAtStake.DistrictName;
        this.attackingCountry = attackingCountry;
        this.defendingCountry = defendingCountry;
        this.districtAtStake = districtAtStake;

        Debug.Log("Армия атаки состоит из " + attackingCountry.Army.Units.Count + " юнитов");
        Debug.Log("Армия защиты состоит из " + defendingCountry.Army.Units.Count + " юнитов");

        // В любом случае даём защищающийся стороне временных юнитов, если нужно.
        if (LeaderMonoBehaviour.GameManager.gameSession.GameRules.GiveFreeUnitsForDefending) GiveFreeUnits();

        // Если игрок учавствует в битве...
        if (attackingCountry.Leader.IsPlayer || defendingCountry.Leader.IsPlayer)
        {
            // то открываем интрефейс и начинаем полноценный бой.
            StartBattleWithPlayer();
        }
        else
        {
            // не открываем интерфейс и начинаем упрощённый бой.
            StartBattleWithotPlayer();
        }
    }

    /// <summary>
    /// Выиграл кто-то.
    /// </summary>
    /// <param name="defenderWon">Победил защищающийся?</param>
    public void EndBattle(bool defenderWon)
    {
        Debug.Log("Закончился бой");
        Debug.LogWarning("Армия атаки состоит из " + attackingCountry.Army.Units.Count + " юнитов");
        Debug.LogWarning("Армия защиты состоит из " + defendingCountry.Army.Units.Count + " юнитов");

        GameManager gameManager = LeaderMonoBehaviour.GameManager;
        string whoWonMessage;
        Color whoWontTextColor;

        // Дадим стандартный бонус победившей стране, изменим владельца района.
        if (defenderWon)
        {
            Debug.Log("Победил защищающийся!");
            whoWonMessage = "Защищающийся лидер " + defendingCountry.Leader.LeaderName + " одержал победу!\n" +
                "Район " + districtAtStake.DistrictName + " остаётся в его владении. ";
            whoWontTextColor = defendingCountry.GetCountryColor();

            defendingCountry.Gold += gameManager.gameSession.GameRules.VictoryBonusInGold;
            defendingCountry.VictoryPoints += gameManager.gameSession.GameRules.VictoryBonusInVictoryPoints;

            attackingCountry.VictoryPoints += gameManager.gameSession.GameRules.DefeatFineInVictoryPoints;
        }
        else
        {
            Debug.Log("Победил атакующий!");
            whoWonMessage = "Атакующий лидер " + attackingCountry.Leader.LeaderName + " одержал победу!\n" +
                "Район " + districtAtStake.DistrictName + " переходит в его владение. ";
            whoWontTextColor = attackingCountry.GetCountryColor();

            attackingCountry.Gold += gameManager.gameSession.GameRules.VictoryBonusInGold;
            attackingCountry.VictoryPoints += gameManager.gameSession.GameRules.VictoryBonusInVictoryPoints;

            defendingCountry.VictoryPoints += gameManager.gameSession.GameRules.DefeatFineInVictoryPoints;

            // Теперь передадим атакующей стороне Район.
            defendingCountry.RemoveDistrict(districtAtStake.districtFunc, true, attackingCountry);

            // Если это победа над страной, то дадим ещё бонус.
            if (defendingCountry.DistrictsIds.Count == 0)
            {
                attackingCountry.VictoryPoints += gameManager.gameSession.GameRules.VictoryOverCountryBonusInVictoryPoints;
                whoWonMessage += "Кроме того, эта победа озноменовала собой конец правления лидера " + defendingCountry.Leader.LeaderName + "!";
            }
        }

        // Показывваем экран, только если игрок принимал участие в битве.
        if (defendingCountry.Leader.IsPlayer || attackingCountry.Leader.IsPlayer)
        {
            OpenBattleOverScreen(whoWonMessage, whoWontTextColor);
        }
        else
        {
            CloseTheWholeThing();
        }
    }

    public void OpenBattleOverScreen(string whoWonHint, Color colorOfTheHintText)
    {
        battleOverScreen.SetActive(true);
        whoWonHintText.text = whoWonHint;
        whoWonHintText.color = colorOfTheHintText;
    }

    public void CloseTheWholeThing()
    {
        battleOverScreen.SetActive(false);

        GameManager gameManager = LeaderMonoBehaviour.GameManager;

        //Destroy(attackingUnit.gameObject);
        //Destroy(defendingUnit.gameObject);
        attackingUnit = null;
        defendingUnit = null;

        districtAtStake.districtFunc.HighlightDistrict(7);
        fightUI.SetActive(false);
        gameManager.CameraController.LockCamera(false);

        gameManager.UpdateStats();
        gameManager.districtUI.UpdateIfNeeded();
        defendingCountry.Army.DeleteTemporaryUnits();

        gameManager.gameSession.Countries[LeaderMonoBehaviour.GameManager.gameSession.CurrentCountry].Leader.BattleEnded();
    }

    /// <summary>
    /// Проиграли оба лидера.
    /// </summary>
    public void EndBattle()
    {
        Debug.Log("Проиграли оба лидера!");
        defendingCountry.RemoveDistrict(districtAtStake.districtFunc, true, null);

        // Показывваем экран, только если игрок принимал участие в битве.
        if (defendingCountry.Leader.IsPlayer || attackingCountry.Leader.IsPlayer)
        {
            OpenBattleOverScreen("Оба Лидера потеряли всю свою армию, поэтому Район становиться нейтральным!", Color.black);
        }
        else
        {
            CloseTheWholeThing();
        }
        
    }

    private void StartBattleWithPlayer()
    {
        // Переместим камеру на район и заблочим.
        LeaderMonoBehaviour.GameManager.CameraController.FocusCamera(districtAtStake.districtFunc.gameObject);
        LeaderMonoBehaviour.GameManager.CameraController.LockCamera(true);
        fightUI.SetActive(true);

        // Пусть ИИ-подумает над улучшением армии.
        if (!attackingCountry.Leader.IsPlayer) attackingCountry.Leader.ConsiderToImproveArmyBeforeFight(defendingCountry.Army.Power);
        else if (!defendingCountry.Leader.IsPlayer) defendingCountry.Leader.ConsiderToImproveArmyBeforeFight(attackingCountry.Army.Power);

        // Настроим отображение панелей армий для подготовки к бою.
        // Для защиты.
        if (defendingCountry.Leader.IsPlayer)
        {
            defenderArmyUI.Open(defendingCountry, 0, this, true);
            admitDefeatHintText.text = "Если считаете, что проигрываете или претензии противника на этот Район оправданы, " +
                "то можете уступить Район. В этом случае вам будет засчитано поражение в бою, но живые юниты остануться в вашей армии.";
        }
        else
        {
            defenderArmyUI.Open(defendingCountry, 2, this, true);
        }

        // Для нападения.
        if (attackingCountry.Leader.IsPlayer)
        {
            attackerArmyUI.Open(attackingCountry, 0, this, false);
            admitDefeatHintText.text = "Если считаете, что проигрываете, " +
                "то можете отменить атаку. В этом случае вам будет засчитано поражение в бою, но живые юниты остануться в вашей армии.";
        }
        else
        {
            attackerArmyUI.Open(attackingCountry, 2, this, false);
        }
    }

    /// <summary>
    /// Проверить, не пора ли заканчивать сражение.
    /// </summary>
    /// <returns>bool, если пора, иначе false.</returns>
    private bool CheckIfSomebodyIsDefeated()
    {
        // Если проиграли оба.
        if (attackingCountry.Army.Units.Count == 0 && defendingCountry.Army.Units.Count == 0)
        {
            EndBattle();
            return true;
        }
        // Если проиграл защищающийся.
        else if (attackingCountry.Army.Units.Count != 0 && defendingCountry.Army.Units.Count == 0)
        {
            EndBattle(false);
            return true;
        }
        // Если проиграл атакующий.
        else if (attackingCountry.Army.Units.Count == 0 && defendingCountry.Army.Units.Count != 0)
        {
            EndBattle(true);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ConfirmReady()
    {
        // Начинается бой.
        buttonReadyToFight.gameObject.SetActive(false);
        readyToFightHintText.gameObject.SetActive(false);

        // Изменим режим панели армии игрока на бой.
        if (defendingCountry.Leader.IsPlayer)
        {
            defenderArmyUI.Open(defendingCountry, 1, this, true);
        }
        else
        {
            attackerArmyUI.Open(attackingCountry, 1, this, false);
        }

        // Проверяем, нет ли проигрыша.
        if (!CheckIfSomebodyIsDefeated()) MakeAILeaderToChooseUnit();
    }

    /// <summary>
    /// Игрок сдался доссрочно, засчитывем ему поражение.
    /// </summary>
    public void AdmitDefeat()
    {
        if (defendingCountry.Leader.IsPlayer)
        {
            EndBattle(false);
        }
        else
        {
            EndBattle(true);
        }
    }

    private void GiveFreeUnits()
    {
        defendingCountry.Army.GiveTemporaryUnits(districtAtStake);
    }

    private void StartBattleWithotPlayer()
    {
        // Выигрывает тот, у которого мощь армии была больше.
        // У проигравшего убиваем всю армию. У победившего - количество равное разнице.

        // Если у атаки больше.
        if (attackingCountry.Army.Power > defendingCountry.Army.Power)
        {
            // Выиграл атакующий.
            Debug.Log("В ходе столкновения двух ИИ-Лидеров, выиграл атакующий.");
            killUnitsInAIArmy(attackingCountry, System.Math.Abs(attackingCountry.Army.Units.Count - defendingCountry.Army.Units.Count));
            killUnitsInAIArmy(defendingCountry, defendingCountry.Army.Units.Count);

            EndBattle(false);
        }
        // Если у защиты больше.
        else if (defendingCountry.Army.Power > attackingCountry.Army.Power)
        {
            // Выиграл защищающийся.
            Debug.Log("В ходе столкновения двух ИИ-Лидеров, выиграл защищающийся.");
            killUnitsInAIArmy(defendingCountry, System.Math.Abs(defendingCountry.Army.Units.Count - attackingCountry.Army.Units.Count));
            killUnitsInAIArmy(attackingCountry, attackingCountry.Army.Units.Count);

            EndBattle(true);
        }
        // Если поровну.
        else
        {
            // Выиграл защищающийся.
            Debug.Log("В ходе столкновения двух ИИ-Лидеров, ни один не выиграл.");
            killUnitsInAIArmy(attackingCountry, attackingCountry.Army.Units.Count);
            killUnitsInAIArmy(defendingCountry, defendingCountry.Army.Units.Count);

            EndBattle();
        }

        //if (Leader.MakeProbabilityWeightedDecision(50))
        //{
        //    
        //    EndBattle(true);
        //}
        //else
        //{
        //    // Выиграл атакующий.
        //    Debug.Log("В ходе столкновения двух ИИ-Лидеров, выиграл атакующий.");
        //    EndBattle(false);
        //}
    }

    private void killUnitsInAIArmy(Country country, int numberOfUnitsToKill)
    {
        for (int i = 0; i < numberOfUnitsToKill; i++)
        {
            if (numberOfUnitsToKill < country.Army.Units.Count)
            {
                country.Army.Units[numberOfUnitsToKill].Health = -10;
            }
        }

        country.Army.DeleteDeadUnits(country);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
