using System.Collections.Generic;

/// <summary>
/// Класс, содержащий в себе переменные Игрового Баланса. 
/// Как работать: Значения полей при инициализации - это значения по-умолчанию.
/// Для вычитания значения в игре, необходимо здесь его установить со знаком минус.
/// </summary>
public class GameRules
{
    // Захваты и потери Районов и победы над Странами.
    /// <summary>
    /// Сколько Очков Победы должна получить Страна за добросовестный захват нейтрального Района.
    /// </summary>
    public int DistrictAchieved { get => districtAchieved; set => districtAchieved = value; }
    /// <summary>
    /// Сколько Очков Победы должна потерять Страна за добросовестный уход из Района.
    /// </summary>
    public int DistrictAbandoned { get => districtAbandoned; set => districtAbandoned = value; }
    /// <summary>
    /// Сколько Очков Победы должен заработать Лидер, который захватил Район в бою.
    /// </summary>
    public int DistrictAchievedInCombat { get => districtAchievedInCombat; set => districtAchievedInCombat = value; }
    /// <summary>
    /// Сколько Очков Победы должен потерять Лидер, из-за потери Района в бою.
    /// </summary>
    public int DistrictLostInCombat { get => districtLostInCombat; set => districtLostInCombat = value; }
    /// <summary>
    /// Сколько Очков Победы должен получить Лидер за победу в бою.
    /// </summary>
    public int VictoryBonusInVictoryPoints { get => victoryBonusInVictoryPoints; set => victoryBonusInVictoryPoints = value; }
    /// <summary>
    /// Сколько Очков Победы должен потерять Лидер за проигрыш в бою.
    /// </summary>
    public int DefeatFineInVictoryPoints { get => defeatFineInVictoryPoints; set => defeatFineInVictoryPoints = value; }
    /// <summary>
    /// Сколько Золота должен получить Лидер за победу в бою.
    /// </summary>
    public int VictoryBonusInGold { get => victoryBonusInGold; set => victoryBonusInGold = value; }
    /// <summary>
    /// Сколько Очков Победы должен получить Лидер за победу над Страной (при захвате её последнего Района).
    /// </summary>
    public int VictoryOverCountryBonusInVictoryPoints { get => victoryOverCountryBonusInVictoryPoints; set => victoryOverCountryBonusInVictoryPoints = value; }

    // Обслуживание.
    /// <summary>
    /// Сколько Золота отнимает один Район у своего владельца за ход.
    /// </summary>
    public int DistrictMaintenance { get => districtMaintenance; set => districtMaintenance = value; }

    // Прочие правила.
    /// <summary>
    /// Сколько нейтральных Районов может захватить Страна за ход.
    /// </summary>
    public int NumberOfAllowedDistrictsToConquerPerMove { get => numberOfAllowedDistrictsToConquerPerMove; set => numberOfAllowedDistrictsToConquerPerMove = value; }
    /// <summary>
    /// Давать ли бесплатных временных юнитов защищающей Район Стране?
    /// </summary>
    public bool GiveFreeUnitsForDefending { get => giveFreeUnitsForDefending; set => giveFreeUnitsForDefending = value; }
    /// <summary>
    /// Через сколько ходов после объявления войны вновь можно будет попробовать заключить мир?
    /// </summary>
    public int PeaceNegotiationsDelayAfterWar { get => peaceNegotiationsDelayAfterWar; set => peaceNegotiationsDelayAfterWar = value; }
    /// <summary>
    /// Через сколько ходов после неудачной попытки заключить мир можно будет попробовать заключить мир?
    /// </summary>
    public int PeaceNegotiationsDelayAfterPeaceSuggestion { get => peaceNegotiationsDelayAfterPeaceSuggestion; set => peaceNegotiationsDelayAfterPeaceSuggestion = value; }
    /// <summary>
    /// Через сколько ходов после заключения мира можно будет объявить войну?
    /// </summary>
    public int WarDeclarationDelayAfterPeace { get => warDeclarationDelayAfterPeace; set => warDeclarationDelayAfterPeace = value; }

    // Условия конца игры.
    /// <summary>
    /// Должна ли игра закончится по достижению определённого количества ходов?
    /// </summary>
    public bool EndGameAfterMoves { get => endGameAfterMoves; set => endGameAfterMoves = value; }
    /// <summary>
    /// Через сколько ходов игра должна закончиться, если нужно.
    /// </summary>
    public int EndGameAfterNumberOfMoves { get => endGameAfterNumberOfMoves; set => endGameAfterNumberOfMoves = value; }
    /// <summary>
    /// Должна ли игра закончится по достижению определённого кол-ва Очков Победы.
    /// </summary>
    public bool EndGameAfterVictoryPoints { get => endGameAfterVictoryPoints; set => endGameAfterVictoryPoints = value; }
    /// <summary>
    /// Сколько Очков Победы должна заработать Страна, чтобы игра закончилась, если нужно.
    /// </summary>
    public int EndGameAfterNumberOfVictoryPoints { get => endGameAfterNumberOfVictoryPoints; set => endGameAfterNumberOfVictoryPoints = value; }

    // Бонусы Районов.
    /// <summary>
    /// Сколько золота будет давать за ход Район с соответствующим бонусом Района.
    /// </summary>
    public int GoldBonus { get => goldBonus; set => goldBonus = value; }
    /// <summary>
    /// Сколько железа будет давать за ход Район с соответствующим бонусом.
    /// </summary>
    public int IronBonus { get => ironBonus; set => ironBonus = value; }
    /// <summary>
    /// Сколько животноводства будет давать за ход Район с соответствующим бонусом.
    /// </summary>
    public int HorsesBonus { get => horsesBonus; set => horsesBonus = value; }
    /// <summary>
    /// Сколько Очков Победы будет давать Район с Бонусом Района "Очки Победы" при захвате .
    /// </summary>
    public int VictoryPointsBonus { get => victoryPointsBonus; set => victoryPointsBonus = value; }
    /// <summary>
    /// Сколько Очков Победы будет вычитать Район с Бонусов Района "Очки Победы при утере".
    /// </summary>
    public int VictoryPointsBonusLost { get => victoryPointsBonusLost; set => victoryPointsBonusLost = value; }

    // Улучшения Районов.
    /// <summary>
    /// Сколько стоит постройка улучшения "Разработка Бонуса Района".
    /// </summary>
    public int CostOfBonusProduction { get => costOfBonusProduction; set => costOfBonusProduction = value; }
    /// <summary>
    /// Сколько стоит постройка улучшения "Добыча золота".
    /// </summary>
    public int CostOfGoldProduction { get => costOfGoldProduction; set => costOfGoldProduction = value; }
    /// <summary>
    /// Сколько золота за ход будет давать улучшение "Добыча Золота".
    /// </summary>
    public int GoldProductionBonus { get => goldProductionBonus; set => goldProductionBonus = value; }
    /// <summary>
    /// Сколько стоит постройка улучшения "Бюро Безопасности".
    /// </summary>
    public int CostOfAgentsProduction { get => costOfAgentsProduction; set => costOfAgentsProduction = value; }
    /// <summary>
    /// На сколько Бюро Безопасности увеличивает/уменьшает вместительность "Тайная Полиция".
    /// </summary>
    public int AgentsProductionBonus { get => agentsProductionBonus; set => agentsProductionBonus = value; }
    /// <summary>
    /// Сколько стоит постройка улучшения "Монумент".
    /// </summary>
    public int CostOfMonument { get => costOfMonument; set => costOfMonument = value; }
    /// <summary>
    /// Сколько Очков Победы даст/вычтет постройка улучшения "Монумент".
    /// </summary>
    public int MonumentBonus { get => monumentBonus; set => monumentBonus = value; }
    
    // Юниты.
    /// <summary>
    /// Требования для очков вместительности (0 - воин, 1 - усиленный воин, 2 - всадник, 3 - тайная полиция).
    /// </summary>
    public List<int> CapacityRequirements { get => capacityRequirements; set => capacityRequirements = value; }
    /// <summary>
    /// Требования для золота. (0 - воин, 1 - усиленный воин, 2 - всадник, 3 - тайная полиция).
    /// </summary>
    public List<int> GoldRequirements { get => goldRequirements; set => goldRequirements = value; }
    /// <summary>
    /// Требования для железа. (0 - воин, 1 - усиленный воин, 2 - всадник, 3 - тайная полиция).
    /// </summary>
    public List<int> IronRequirements { get => ironRequirements; set => ironRequirements = value; }
    /// <summary>
    /// Требования для животноводства. (0 - воин, 1 - усиленный воин, 2 - всадник, 3 - тайная полиция).
    /// </summary>
    public List<int> HorsesRequirements { get => horsesRequirements; set => horsesRequirements = value; }
    /// <summary>
    /// Требования для тайной полиции. (0 - воин, 1 - усиленный воин, 2 - всадник, 3 - тайная полиция).
    /// </summary>
    public List<int> AgentsRequirements { get => agentsRequirements; set => agentsRequirements = value; }
    /// <summary>
    /// Урон юнита. (0 - воин, 1 - усиленный воин, 2 - всадник, 3 - тайная полиция).
    /// </summary>
    public List<int> Damages { get => damages; set => damages = value; }
    /// <summary>
    /// Здоровье юнита. (0 - воин, 1 - усиленный воин, 2 - всадник, 3 - тайная полиция).
    /// </summary>
    public List<int> Healths { get => healths; set => healths = value; }



    #region Поля
    // Захваты и потери Районов и победы над Странами.
    int districtAchieved = 1;
    int districtAbandoned = -1;
    int districtAchievedInCombat = 1;
    int districtLostInCombat = -1;

    int victoryBonusInVictoryPoints = 10;
    int defeatFineInVictoryPoints = -5;
    int victoryBonusInGold = 5;
    int victoryOverCountryBonusInVictoryPoints = 30;

    // Обслуживание.
    int districtMaintenance = -1;

    // Прочие правила.
    int numberOfAllowedDistrictsToConquerPerMove = 1;
    bool giveFreeUnitsForDefending = true;
    int peaceNegotiationsDelayAfterWar = 1;
    int peaceNegotiationsDelayAfterPeaceSuggestion = 1;
    int warDeclarationDelayAfterPeace = 0;

    // Условия конца игры.
    bool endGameAfterMoves = true;
    int endGameAfterNumberOfMoves = 250;
    bool endGameAfterVictoryPoints = true;
    int endGameAfterNumberOfVictoryPoints = 750;

    // Бонусы Районов.
    int goldBonus = 3;
    int ironBonus = 3;
    int horsesBonus = 3;
    int victoryPointsBonus = 10;
    int victoryPointsBonusLost = -10;

    // Улучшения Районов.
    int costOfBonusProduction = -10;

    int costOfGoldProduction = -5;
    int goldProductionBonus = 2;

    int costOfAgentsProduction = -15;
    int agentsProductionBonus = 3;

    int costOfMonument = -25;
    int monumentBonus = 15;

    // Юниты (0 - воин, 1 - усиленный воин, 2 - всадник, 3 - тайная полиция).
    List<int> capacityRequirements = new List<int>() { 1, 1, 1, 2};

    List<int> goldRequirements = new List<int>() { 2, 2, 3, 5};
    List<int> ironRequirements = new List<int>() { 0, 1, 0, 0};
    List<int> horsesRequirements = new List<int>() { 0, 0, 1, 0};
    List<int> agentsRequirements = new List<int>() { 0, 0, 0, 1};

    List<int> damages = new List<int>() { 2, 2, 3, 4};
    List<int> healths = new List<int>() { 1, 2, 2, 3};

    #endregion
}
