using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Описывает поведение Лидера "Коллекционер".
/// </summary>
public class TheСollector : Leader
{
    public TheСollector() : base()
    {

    }

    public TheСollector(LeaderMonoBehaviour leaderMonoBehaviour) : base(leaderMonoBehaviour)
    {
        // Назначим имя.
        leaderName = "Коллекционер";

        // Назначим цвет Страны.
        Country.SetCountryColor(Color.yellow);
        Country.CountryName = "Торговый Союз";
        // Установим начальные значения для Страны этого Лидера.
        Country.Gold = 30;
        Country.Iron = 4;
        Country.Horses = 4;
        Country.Agents = 0;

        DefaultLine = "Приветствую тебя, друг! Мы ценители прекрасного и коллекционеры редкого. Мы не желаем никому зла, ответьте тем же!";
        WarLine = "Я всё ещё считаю, что мир нам всем просто необходим!";
        PeaceAcceptedLine = "Наконец, мы можем сложить оружие. Будет непросто, но я верю, мы подружимся вновь.";
        WarDeclarationFromThisLine = "Прошу меня понять: на кону существование моей страны! Очень жаль, но военный конфликт неизбежен.";
        WarDeclarationToThisLine = "Очень жаль, что вы опускаетесь до такого уровня... Ещё не поздно остановить это... Правда!";

        MultiplyStartVariablesByDifficultyInfluence();
    }

    public override void MakeMove()
    {
        Debug.Log("The Сollector делает ход. Золото: " + Country.Gold);
        //Debug.Log("Лидер " + LeaderName + " имеет кризис: " + CheckCrysis());
        base.MakeMove();

        if (!battleIsGoingToHappen) LeaderMonoBehaviour.GameManager.NextMove();
        //LeaderMonoBehaviour.GameManager.NextMove();
    }


    public override void RequestPeace(Country otherCountry)
    {
        Relationship relationship = LeaderMonoBehaviour.GameManager.gameSession.FindRelationship(otherCountry.CountryId, Country.CountryId);
        // Принимает мир с 80% вероятностью.
        if (MakeProbabilityWeightedDecision(80))
        {
            // В любом случае, запретим заключать мир на этом ходу.
            LockPeace(relationship);
            relationship.AtWar = false;
            Debug.Log($"Лидер {otherCountry.Leader.LeaderName} предложил мир Лидеру {leaderName}, и он согласился.");

            if (otherCountry.Leader.IsPlayer) LeaderMonoBehaviour.OpenDialogUI(PeaceAcceptedLine, false);
        }
        else
        {
            // В любом случае, запретим заключать мир на этом ходу.
            LockPeace(relationship);
            Debug.Log($"Лидер {otherCountry.Leader.LeaderName} предложил мир Лидеру {leaderName}, но он не согласился.");

            if (otherCountry.Leader.IsPlayer) LeaderMonoBehaviour.OpenDialogUI("Простите, мы согласны, но не прямо сейчас. Предложите ещё раз немного позже", false);
        }
    }

    protected override bool ConquerNeutralOrEnemyDistrict(List<DistrictInfo> neutralDistricts, List<DistrictInfo> enemyDistricts)
    {
        // Атакуем нейтральный.
        return ConquerNeutralDistrict(neutralDistricts);
    }
}
