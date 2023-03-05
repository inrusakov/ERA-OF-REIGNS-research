using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Представляет собой Район-информацию.
/// </summary>
public class DistrictInfo
{
    // Название Района.
    private string districtName;
    // Текущий владелец Района.
    [JsonIgnore] public Country holder;
    // Ссылка на Район-функционал.
    [JsonIgnore] public District districtFunc;

    public DistrictInfo()
    {

    }

    public DistrictInfo(District district)
    {
        DistrictName = district.name;
        GenerateBonus();
        DestroyAllBuildings();
    }

    /// <summary>
    /// Название Района.
    /// </summary>
    public string DistrictName { get => districtName; set => districtName = value; }

    #region Бонус Района
    /// <summary>
    /// Бонус Района. 0 - нет бонуса, 1 - золото, 2 - железо, 3 - животноводство, 4 - очки победы.
    /// </summary>
    public int DistrictBonus { get => districtBonus; set => districtBonus = value; }
    int districtBonus;
    public void GenerateBonus()
    {
        districtBonus = Random.Range(0, 5);
    }
    #endregion

    #region Улучшения Района..
    /// <summary>
    /// Улучшение "Разработка Бонуса Района".
    /// </summary>
    public bool HasBonusProduction { get => hasBonusProduction; set => hasBonusProduction = value; }
    bool hasBonusProduction;
    public void BuildBonusProduction()
    {
        holder.Gold += District.gameManager.gameSession.GameRules.CostOfBonusProduction;
        hasBonusProduction = true;
        switch (DistrictBonus)
        {
            case 2:
                holder.Iron += District.gameManager.gameSession.GameRules.IronBonus;
                break;
            case 3:
                holder.Horses += District.gameManager.gameSession.GameRules.HorsesBonus;
                break;
        }

        District.gameManager.UpdateStats();
        District.gameManager.districtUI.UpdateIfNeeded();

        Debug.Log("Лидер " + holder.Leader.LeaderName + " построил улучшение \"Разработка Бонуса Района\" в Районе " + DistrictName);
    }

    /// <summary>
    /// Улучшение "Добыча Золота".
    /// </summary>
    public bool HasGoldProduction { get => hasGoldProduction; set => hasGoldProduction = value; }
    bool hasGoldProduction;
    public void BuildGoldProduction()
    {
        holder.Gold += District.gameManager.gameSession.GameRules.CostOfGoldProduction;
        hasGoldProduction = true;

        District.gameManager.UpdateStats();
        District.gameManager.districtUI.UpdateIfNeeded();

        Debug.Log("Лидер " + holder.Leader.LeaderName + " построил улучшение \"Добыча Золота\" в Районе " + DistrictName);
    }

    /// <summary>
    /// Улучшение "Бюро Безопасности".
    /// </summary>
    public bool HasSecurityBureau { get => hasSecurityBureau; set => hasSecurityBureau = value; }
    bool hasSecurityBureau;
    public void BuildSecurityBureau()
    {
        holder.Gold += District.gameManager.gameSession.GameRules.CostOfAgentsProduction;
        holder.Agents += District.gameManager.gameSession.GameRules.AgentsProductionBonus;
        hasSecurityBureau = true;

        District.gameManager.UpdateStats();
        District.gameManager.districtUI.UpdateIfNeeded();

        Debug.Log("Лидер " + holder.Leader.LeaderName + " построил улучшение \"Бюро Безопасности\" в Районе " + DistrictName);

    }

    /// <summary>
    /// Улучшение "Монумент".
    /// </summary>
    public bool HasMonument { get => hasMonument; set => hasMonument = value; }
    bool hasMonument;
    public void BuildMonument()
    {
        holder.Gold += District.gameManager.gameSession.GameRules.CostOfMonument;
        holder.VictoryPoints += District.gameManager.gameSession.GameRules.MonumentBonus;
        hasMonument = true;

        District.gameManager.UpdateStats();
        District.gameManager.districtUI.UpdateIfNeeded();

        Debug.Log("Лидер " + holder.Leader.LeaderName + " построил улучшение \"Монумент\" в Районе " + DistrictName);
    }

    /// <summary>
    /// Уничтожить все улучшения (постройки) в этом Районе.
    /// </summary>
    public void DestroyAllBuildings()
    {
        hasBonusProduction = false;
        hasGoldProduction = false;
        hasSecurityBureau = false;
        hasMonument = false;
    }

    #endregion
}
