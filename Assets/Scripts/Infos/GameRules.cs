using System.Collections.Generic;

/// <summary>
/// �����, ���������� � ���� ���������� �������� �������. 
/// ��� ��������: �������� ����� ��� ������������� - ��� �������� ��-���������.
/// ��� ��������� �������� � ����, ���������� ����� ��� ���������� �� ������ �����.
/// </summary>
public class GameRules
{
    // ������� � ������ ������� � ������ ��� ��������.
    /// <summary>
    /// ������� ����� ������ ������ �������� ������ �� �������������� ������ ������������ ������.
    /// </summary>
    public int DistrictAchieved { get => districtAchieved; set => districtAchieved = value; }
    /// <summary>
    /// ������� ����� ������ ������ �������� ������ �� �������������� ���� �� ������.
    /// </summary>
    public int DistrictAbandoned { get => districtAbandoned; set => districtAbandoned = value; }
    /// <summary>
    /// ������� ����� ������ ������ ���������� �����, ������� �������� ����� � ���.
    /// </summary>
    public int DistrictAchievedInCombat { get => districtAchievedInCombat; set => districtAchievedInCombat = value; }
    /// <summary>
    /// ������� ����� ������ ������ �������� �����, ��-�� ������ ������ � ���.
    /// </summary>
    public int DistrictLostInCombat { get => districtLostInCombat; set => districtLostInCombat = value; }
    /// <summary>
    /// ������� ����� ������ ������ �������� ����� �� ������ � ���.
    /// </summary>
    public int VictoryBonusInVictoryPoints { get => victoryBonusInVictoryPoints; set => victoryBonusInVictoryPoints = value; }
    /// <summary>
    /// ������� ����� ������ ������ �������� ����� �� �������� � ���.
    /// </summary>
    public int DefeatFineInVictoryPoints { get => defeatFineInVictoryPoints; set => defeatFineInVictoryPoints = value; }
    /// <summary>
    /// ������� ������ ������ �������� ����� �� ������ � ���.
    /// </summary>
    public int VictoryBonusInGold { get => victoryBonusInGold; set => victoryBonusInGold = value; }
    /// <summary>
    /// ������� ����� ������ ������ �������� ����� �� ������ ��� ������� (��� ������� � ���������� ������).
    /// </summary>
    public int VictoryOverCountryBonusInVictoryPoints { get => victoryOverCountryBonusInVictoryPoints; set => victoryOverCountryBonusInVictoryPoints = value; }

    // ������������.
    /// <summary>
    /// ������� ������ �������� ���� ����� � ������ ��������� �� ���.
    /// </summary>
    public int DistrictMaintenance { get => districtMaintenance; set => districtMaintenance = value; }

    // ������ �������.
    /// <summary>
    /// ������� ����������� ������� ����� ��������� ������ �� ���.
    /// </summary>
    public int NumberOfAllowedDistrictsToConquerPerMove { get => numberOfAllowedDistrictsToConquerPerMove; set => numberOfAllowedDistrictsToConquerPerMove = value; }
    /// <summary>
    /// ������ �� ���������� ��������� ������ ���������� ����� ������?
    /// </summary>
    public bool GiveFreeUnitsForDefending { get => giveFreeUnitsForDefending; set => giveFreeUnitsForDefending = value; }
    /// <summary>
    /// ����� ������� ����� ����� ���������� ����� ����� ����� ����� ����������� ��������� ���?
    /// </summary>
    public int PeaceNegotiationsDelayAfterWar { get => peaceNegotiationsDelayAfterWar; set => peaceNegotiationsDelayAfterWar = value; }
    /// <summary>
    /// ����� ������� ����� ����� ��������� ������� ��������� ��� ����� ����� ����������� ��������� ���?
    /// </summary>
    public int PeaceNegotiationsDelayAfterPeaceSuggestion { get => peaceNegotiationsDelayAfterPeaceSuggestion; set => peaceNegotiationsDelayAfterPeaceSuggestion = value; }
    /// <summary>
    /// ����� ������� ����� ����� ���������� ���� ����� ����� �������� �����?
    /// </summary>
    public int WarDeclarationDelayAfterPeace { get => warDeclarationDelayAfterPeace; set => warDeclarationDelayAfterPeace = value; }

    // ������� ����� ����.
    /// <summary>
    /// ������ �� ���� ���������� �� ���������� ������������ ���������� �����?
    /// </summary>
    public bool EndGameAfterMoves { get => endGameAfterMoves; set => endGameAfterMoves = value; }
    /// <summary>
    /// ����� ������� ����� ���� ������ �����������, ���� �����.
    /// </summary>
    public int EndGameAfterNumberOfMoves { get => endGameAfterNumberOfMoves; set => endGameAfterNumberOfMoves = value; }
    /// <summary>
    /// ������ �� ���� ���������� �� ���������� ������������ ���-�� ����� ������.
    /// </summary>
    public bool EndGameAfterVictoryPoints { get => endGameAfterVictoryPoints; set => endGameAfterVictoryPoints = value; }
    /// <summary>
    /// ������� ����� ������ ������ ���������� ������, ����� ���� �����������, ���� �����.
    /// </summary>
    public int EndGameAfterNumberOfVictoryPoints { get => endGameAfterNumberOfVictoryPoints; set => endGameAfterNumberOfVictoryPoints = value; }

    // ������ �������.
    /// <summary>
    /// ������� ������ ����� ������ �� ��� ����� � ��������������� ������� ������.
    /// </summary>
    public int GoldBonus { get => goldBonus; set => goldBonus = value; }
    /// <summary>
    /// ������� ������ ����� ������ �� ��� ����� � ��������������� �������.
    /// </summary>
    public int IronBonus { get => ironBonus; set => ironBonus = value; }
    /// <summary>
    /// ������� �������������� ����� ������ �� ��� ����� � ��������������� �������.
    /// </summary>
    public int HorsesBonus { get => horsesBonus; set => horsesBonus = value; }
    /// <summary>
    /// ������� ����� ������ ����� ������ ����� � ������� ������ "���� ������" ��� ������� .
    /// </summary>
    public int VictoryPointsBonus { get => victoryPointsBonus; set => victoryPointsBonus = value; }
    /// <summary>
    /// ������� ����� ������ ����� �������� ����� � ������� ������ "���� ������ ��� �����".
    /// </summary>
    public int VictoryPointsBonusLost { get => victoryPointsBonusLost; set => victoryPointsBonusLost = value; }

    // ��������� �������.
    /// <summary>
    /// ������� ����� ��������� ��������� "���������� ������ ������".
    /// </summary>
    public int CostOfBonusProduction { get => costOfBonusProduction; set => costOfBonusProduction = value; }
    /// <summary>
    /// ������� ����� ��������� ��������� "������ ������".
    /// </summary>
    public int CostOfGoldProduction { get => costOfGoldProduction; set => costOfGoldProduction = value; }
    /// <summary>
    /// ������� ������ �� ��� ����� ������ ��������� "������ ������".
    /// </summary>
    public int GoldProductionBonus { get => goldProductionBonus; set => goldProductionBonus = value; }
    /// <summary>
    /// ������� ����� ��������� ��������� "���� ������������".
    /// </summary>
    public int CostOfAgentsProduction { get => costOfAgentsProduction; set => costOfAgentsProduction = value; }
    /// <summary>
    /// �� ������� ���� ������������ �����������/��������� ��������������� "������ �������".
    /// </summary>
    public int AgentsProductionBonus { get => agentsProductionBonus; set => agentsProductionBonus = value; }
    /// <summary>
    /// ������� ����� ��������� ��������� "��������".
    /// </summary>
    public int CostOfMonument { get => costOfMonument; set => costOfMonument = value; }
    /// <summary>
    /// ������� ����� ������ ����/������ ��������� ��������� "��������".
    /// </summary>
    public int MonumentBonus { get => monumentBonus; set => monumentBonus = value; }
    
    // �����.
    /// <summary>
    /// ���������� ��� ����� ��������������� (0 - ����, 1 - ��������� ����, 2 - �������, 3 - ������ �������).
    /// </summary>
    public List<int> CapacityRequirements { get => capacityRequirements; set => capacityRequirements = value; }
    /// <summary>
    /// ���������� ��� ������. (0 - ����, 1 - ��������� ����, 2 - �������, 3 - ������ �������).
    /// </summary>
    public List<int> GoldRequirements { get => goldRequirements; set => goldRequirements = value; }
    /// <summary>
    /// ���������� ��� ������. (0 - ����, 1 - ��������� ����, 2 - �������, 3 - ������ �������).
    /// </summary>
    public List<int> IronRequirements { get => ironRequirements; set => ironRequirements = value; }
    /// <summary>
    /// ���������� ��� ��������������. (0 - ����, 1 - ��������� ����, 2 - �������, 3 - ������ �������).
    /// </summary>
    public List<int> HorsesRequirements { get => horsesRequirements; set => horsesRequirements = value; }
    /// <summary>
    /// ���������� ��� ������ �������. (0 - ����, 1 - ��������� ����, 2 - �������, 3 - ������ �������).
    /// </summary>
    public List<int> AgentsRequirements { get => agentsRequirements; set => agentsRequirements = value; }
    /// <summary>
    /// ���� �����. (0 - ����, 1 - ��������� ����, 2 - �������, 3 - ������ �������).
    /// </summary>
    public List<int> Damages { get => damages; set => damages = value; }
    /// <summary>
    /// �������� �����. (0 - ����, 1 - ��������� ����, 2 - �������, 3 - ������ �������).
    /// </summary>
    public List<int> Healths { get => healths; set => healths = value; }
    /// <summary>
    /// �����������, �� ������� ��������� ��������� �������� ��-�������.
    /// </summary>
    public int DifficultyInfluence { get => difficultyInfluence; set => difficultyInfluence = value; }



    #region ����
    // ������� � ������ ������� � ������ ��� ��������.
    int districtAchieved = 1;
    int districtAbandoned = -1;
    int districtAchievedInCombat = 1;
    int districtLostInCombat = -1;

    int victoryBonusInVictoryPoints = 10;
    int defeatFineInVictoryPoints = -5;
    int victoryBonusInGold = 5;
    int victoryOverCountryBonusInVictoryPoints = 30;

    // ������������.
    int districtMaintenance = -1;

    // ������ �������.
    int numberOfAllowedDistrictsToConquerPerMove = 1;
    bool giveFreeUnitsForDefending = true;
    int peaceNegotiationsDelayAfterWar = 1;
    int peaceNegotiationsDelayAfterPeaceSuggestion = 1;
    int warDeclarationDelayAfterPeace = 0;

    // ������� ����� ����.
    bool endGameAfterMoves = true;
    int endGameAfterNumberOfMoves = 250;
    bool endGameAfterVictoryPoints = true;
    int endGameAfterNumberOfVictoryPoints = 750;

    // ������ �������.
    int goldBonus = 3;
    int ironBonus = 3;
    int horsesBonus = 3;
    int victoryPointsBonus = 10;
    int victoryPointsBonusLost = -10;

    // ��������� �������.
    int costOfBonusProduction = -10;

    int costOfGoldProduction = -5;
    int goldProductionBonus = 2;

    int costOfAgentsProduction = -15;
    int agentsProductionBonus = 3;

    int costOfMonument = -25;
    int monumentBonus = 15;

    // ����� (0 - ����, 1 - ��������� ����, 2 - �������, 3 - ������ �������).
    List<int> capacityRequirements = new List<int>() { 1, 1, 1, 2};

    List<int> goldRequirements = new List<int>() { 2, 2, 3, 5};
    List<int> ironRequirements = new List<int>() { 0, 1, 0, 0};
    List<int> horsesRequirements = new List<int>() { 0, 0, 1, 0};
    List<int> agentsRequirements = new List<int>() { 0, 0, 0, 1};

    List<int> damages = new List<int>() { 2, 2, 3, 4};
    List<int> healths = new List<int>() { 1, 2, 2, 3};

    // �����������, �� ������� ���������� ��������� ��������� �������.
    int difficultyInfluence = 1;

    #endregion
}
