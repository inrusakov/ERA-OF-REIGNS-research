using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Описывает поведение Лидера "Завоеватель".
/// </summary>
public class TheConqueror : Leader
{
    public TheConqueror() : base()
    {

    }

    public TheConqueror(LeaderMonoBehaviour leaderMonoBehaviour) : base(leaderMonoBehaviour)
    {
        // Назначим имя.
        leaderName = "Завоеватель";

        // Назначим цвет Страны.
        Country.SetCountryColor(Color.red);
        Country.CountryName = "Племя Войны";
        // Установим начальные значения для Страны этого Лидера.
        Country.Gold = 15;
        Country.Iron = 9;
        Country.Horses = 9;
        Country.Agents = 0;

        DefaultLine = "Здравствуйте, я - Завоеватель, и да, это моё имя. Давайте условимся сразу: Война между нами неизбежна, вопрос лишь - когда?";
        WarLine = "Надеюсь, вы пришли не мир заключать!";
        PeaceAcceptedLine = "Хорошо, перерыв. До встречи!";
        WarDeclarationFromThisLine = "Ваши земли слишком привлекательны, я их забираю себе - пожалуйста, не сопротивляйтесь!";
        WarDeclarationToThisLine = "Ха! А вы хороши! Я даже рад, давайте повоюем!";

        MultiplyStartVariablesByDifficultyInfluence();
    }

    public override void MakeMove()
    {
        Debug.Log("The Conqueror делает ход. Золото: " + Country.Gold);
        //Debug.Log("Лидер " + LeaderName + " имеет кризис: " + CheckCrysis());
        base.MakeMove();

        if (!battleIsGoingToHappen) LeaderMonoBehaviour.GameManager.NextMove();
        //LeaderMonoBehaviour.GameManager.NextMove();
    }

    protected override bool ExpandLands()
    {
        // Распределим все не наши соседние Районы по спискам.
        List<DistrictInfo> neutralDistricts = new();
        List<DistrictInfo> someonesDistricts = new();
        List<DistrictInfo> enemyDistricts = new();

        // Проходимся по всем Районам нашей страны...
        for (int i = 0; i < Country.DistrictsIds.Count; i++)
        {
            DistrictInfo mydistrict = LeaderMonoBehaviour.GameManager.gameSession.FindDistrictById(Country.DistrictsIds[i]);
            // Проходимся по всем соседним Районам текущего нашего Района...
            for (int j = 0; j < mydistrict.districtFunc.nearbyDistricts.Count; j++)
            {
                DistrictInfo otherDistrict = mydistrict.districtFunc.nearbyDistricts[j].districtInfo;
                // Если соседний район не имеет хозяина, добавим в нейтральные.
                if (otherDistrict.holder == null)
                {
                    AddIfNotAdded(otherDistrict, neutralDistricts);
                    //neutralDistricts.Add(otherDistrict);
                }
                else
                {
                    // Если соседний Район - наш, пропускаем.
                    if (otherDistrict.holder == Country) continue;
                    // Если соседний район чей-то...
                    if (LeaderMonoBehaviour.GameManager.gameSession.FindRelationship(Country.CountryId, otherDistrict.holder.CountryId).AtWar)
                    {
                        // И он вражеский.
                        AddIfNotAdded(otherDistrict, enemyDistricts);
                        //enemyDistricts.Add(otherDistrict);
                    }
                    else
                    {
                        // И он просто чей-то.
                        AddIfNotAdded(otherDistrict, someonesDistricts);
                        //someonesDistricts.Add(otherDistrict);
                    }
                }
            }
        }
        Debug.Log("Было обнаружено: " + neutralDistricts.Count + " нейтральных районов; "
            + someonesDistricts.Count + " чьих-то районов; " + enemyDistricts.Count + " вражеских районов.");


        // Если есть нейтральные и чьи-то.
        if (enemyDistricts.Count == 0 && neutralDistricts.Count != 0 && someonesDistricts.Count != 0)
        {
            Debug.Log("Лидер \"ЗАВОЕВАТЕЛЬ\" обнаружил только нейтральные и чьи-то районы. Выбирает, какой тип будет рассматривать, чтобы атаковать");
            if (MakeProbabilityWeightedDecision(50)) return ConquerSomeonesDistrict(someonesDistricts);
            else return ConquerNeutralDistrict(neutralDistricts);
        }


        // Если есть нейтральные и нет вражеских.
        if (neutralDistricts.Count != 0 && enemyDistricts.Count == 0) return ConquerNeutralDistrict(neutralDistricts);
        // Если есть нейтральные и вражеские.
        if (neutralDistricts.Count != 0 && enemyDistricts.Count != 0) return ConquerNeutralOrEnemyDistrict(neutralDistricts, enemyDistricts);
        // Если нет нейтральных и есть вражеские.
        if (enemyDistricts.Count != 0 && neutralDistricts.Count == 0) return ConquerEnemyDistrict(enemyDistricts);
        // Если есть только чьи-то.
        if (enemyDistricts.Count == 0 && neutralDistricts.Count == 0 && someonesDistricts.Count != 0) return ConquerSomeonesDistrict(someonesDistricts);

        return false;
    }

    /// <summary>
    /// Соглашается на мир, если нет соседства и если выпало удачное случайное решение.
    /// </summary>
    /// <param name="otherCountry"></param>
    public override void RequestPeace(Country otherCountry)
    {
        Relationship relationship = LeaderMonoBehaviour.GameManager.gameSession.FindRelationship(otherCountry.CountryId, Country.CountryId);

        bool isOkay = true;
        string message = "";
        // Проверяем на соседстсво.
        for (int i = 0; i < Country.DistrictsIds.Count; i++)
        {
            if (LeaderMonoBehaviour.GameManager.gameSession.FindDistrictById(Country.DistrictsIds[i]).districtFunc.CheckForBeingNearCountry(otherCountry))
            {
                message = "Пока вы находитесь у меня под носом, речь о мире не может идти!";
                LockPeace(relationship);
                isOkay = false;
                break;
            }
        }

        if (isOkay)
        {
            // Проверим решением.
            if (MakeProbabilityWeightedDecision(80))
            {
                LeaderMonoBehaviour.GameManager.gameSession.FindRelationship(otherCountry.CountryId, Country.CountryId).AtWar = false;
                if (otherCountry.Leader.IsPlayer) LeaderMonoBehaviour.OpenDialogUI(PeaceAcceptedLine, false);
                Debug.Log($"Лидер {otherCountry.Leader.LeaderName} предложил мир Лидеру {LeaderName}, и он согласился.");

                return;
            }
            else
            {
                message = "Нееет уж! Я ещё не наигрался!";
                LockPeace(relationship);
            }
        }

        if (otherCountry.Leader.IsPlayer) LeaderMonoBehaviour.OpenDialogUI(message, false);
        Debug.Log($"Лидер {otherCountry.Leader.LeaderName} предложил мир Лидеру {LeaderName}, но он отказался.");
    }
}
