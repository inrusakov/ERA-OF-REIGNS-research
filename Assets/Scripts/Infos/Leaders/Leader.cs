using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Характер Лидера. Данная базовая реализация, не имеет характера.
/// </summary>
public class Leader
{
    /// <summary>
    /// Имя лидера.
    /// </summary>
    public string LeaderName { get => leaderName; set => leaderName = value; }
    /// <summary>
    /// Является ли этот Лидер игроком?
    /// </summary>
    public bool IsPlayer { get => isPlayer; set => isPlayer = value; }

    public string DefaultLine { get => defaultLine; set => defaultLine = value; }
    public string WarLine { get => warLine; set => warLine = value; }
    public string PeaceAcceptedLine { get => peaceAcceptedLine; set => peaceAcceptedLine = value; }
    public string WarDeclarationFromThisLine { get => warDeclarationFromThisLine; set => warDeclarationFromThisLine = value; }
    public string WarDeclarationToThisLine { get => warDeclarationToThisLine; set => warDeclarationToThisLine = value; }

    [JsonIgnore] public Country Country { get => country; set => country = value; }
    [JsonIgnore] public LeaderMonoBehaviour LeaderMonoBehaviour { get => leaderMonoBehaviour; set => leaderMonoBehaviour = value; }
    protected string leaderName;
    // Страна, которой управляет этот Лидер.
    [JsonIgnore] private Country country;
    // Ссылка на соответсвующего лидера-функцию.
    [JsonIgnore] private LeaderMonoBehaviour leaderMonoBehaviour;
    // Является ли этот Лидер Игроком-человеком?
    private bool isPlayer;

    /// <summary>
    /// Конструктор для загрузки Лидера при дессериализации.
    /// </summary>
    public Leader()
    {

    }

    /// <summary>
    /// Конструктор для создания нового Лидера.
    /// </summary>
    /// <param name="leaderMonoBehaviour"></param>
    public Leader(LeaderMonoBehaviour leaderMonoBehaviour)
    {
        // Создадим страну.
        Country = new Country();
        Country.Leader = this;
        Country.Army = new Army();

        this.LeaderMonoBehaviour = leaderMonoBehaviour;
        isPlayer = false;
    }

    protected void MultiplyStartVariablesByDifficultyInfluence()
    {
        Country.Gold *= LeaderMonoBehaviour.GameManager.gameSession.GameRules.DifficultyInfluence;
        Country.Iron *= LeaderMonoBehaviour.GameManager.gameSession.GameRules.DifficultyInfluence;
        Country.Horses *= LeaderMonoBehaviour.GameManager.gameSession.GameRules.DifficultyInfluence;
        Country.Agents *= LeaderMonoBehaviour.GameManager.gameSession.GameRules.DifficultyInfluence;
    }

    protected bool battleIsGoingToHappen;
    /// <summary>
    /// Попросить Лидера сделать ход.
    /// </summary>
    public virtual void MakeMove()
    {
        battleIsGoingToHappen = false;
        Debug.LogWarning("Лидер делает ход с помощью виртуального метода.");
        Debug.Log("В стране Лидера " + LeaderName + " есть кризис: " + CheckCrysis());

        // Идёт проверка на кризис в стране.
        if (CheckCrysis()) HandleCrysis();

        // Принимаем решение о постройке какого-либо улучшения.
        if (!CheckCrysis() && MakeProbabilityWeightedDecision(40))
        {
            Debug.Log("Лидер " + leaderName + " решил построить какое-нибудь улучшение.");
            BuildRandomProduction();
        }

        // Принимаем решение о расширении армии.
        if (!CheckCrysis() && MakeProbabilityWeightedDecision(40))
        {
            Debug.Log("Лидер " + leaderName + " решил расширить свою армию.");
            Debug.Log("Сейчас армия Лидера состоит из " + Country.Army.Units.Count + " юнитов.");
            BuyUnits();
            Debug.Log("Теперь армия Лидера состоит из " + Country.Army.Units.Count + " юнитов.");
        }

        // Принимаем решение о расширении границ. (ещё нужно, чтобы хватило денег на постройку добычи золота).
        if (!CheckCrysis() && MakeProbabilityWeightedDecision(95) && Country.Gold > -LeaderMonoBehaviour.GameManager.gameSession.GameRules.CostOfGoldProduction)
        {
            Debug.Log("Лидер " + leaderName + " решил расширить свои границы.");
            ExpandLands();
            if (CheckCrysis()) HandleCrysis();
        }

        if (!battleIsGoingToHappen)
        {
            // LeaderMonoBehaviour.GameManager.NextMove();
        }

        Debug.Log("Ход виртуального метода закончен. Золото: " + Country.Gold);
    }

    public virtual void BattleEnded()
    {
        LeaderMonoBehaviour.GameManager.NextMove();
    }

    /// <summary>
    /// Пытается купить юнитов в армию. Проверяет возможность покупки юнитов по убыванию их дороговизны.
    /// </summary>
    /// <returns>true, если действие считать успешно выполненным.</returns>
    protected virtual bool BuyUnits()
    {
        Debug.Log("Лидер покупает юнитов в армию.");
        GameRules gameRules = LeaderMonoBehaviour.GameManager.gameSession.GameRules;
        bool isOkay = false;

        // Пробуем по максимуму закупиться юнитами, начиная с самого дорогого.
        for (int i = 3; i >= 0; i--)
        {
            while (Country.Army.CheckIfBuyngPossible(i, Country))
            {
                Country.Army.AddNewUnit(Country.Army.GetUnit(i, false), Country);
                Debug.Log("Был куплен юнит с id " + i);
                isOkay = true;
            }
        }

        return isOkay;
    }

    /// <summary>
    /// Пытается расширить границы своей страны.
    /// </summary>
    /// <returns>true, если действие считать успешно выполненным.</returns>
    protected virtual bool ExpandLands()
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

    protected bool ConquerSomeonesDistrict(List<DistrictInfo> someonesDistricts)
    {
        Debug.Log("Лидер обдумывает случай, когда есть только чьи-то районы.");

        // Атакуем любой район, объявляя войну его владельцу.
        DistrictInfo districtToConquer = someonesDistricts[Random.Range(0, someonesDistricts.Count)];
        Relationship relationship = LeaderMonoBehaviour.GameManager.gameSession.FindRelationship(country.CountryId, districtToConquer.holder.CountryId);
        relationship.AtWar = true;
        LockPeace(relationship);

        // Проверим, является ли новый враг игроком.
        if (districtToConquer.holder.Leader.isPlayer)
        {
            // Покажем ему сообщение о войне.
            LeaderMonoBehaviour.OpenDialogUI(WarDeclarationFromThisLine, true);
        }
        Debug.Log("Лидер объявляет войну Лидеру " + districtToConquer.holder.Leader.LeaderName);
        // Теперь атакуем.
        Debug.LogWarning("Бой");
        LeaderMonoBehaviour.GameManager.FightManager.StartBattle(country, districtToConquer.holder, districtToConquer);
        battleIsGoingToHappen = true;

        return true;
    }

    protected bool ConquerEnemyDistrict(List<DistrictInfo> enemyDistricts)
    {
        Debug.Log("Лидер обдумывает случай, когда нет нейтральных, но есть вражеские районы.");

        // Атакуем любой из них.
        DistrictInfo districtToConquer = enemyDistricts[Random.Range(0, enemyDistricts.Count)];
        // Теперь атакуем.
        Debug.LogWarning("Бой");
        LeaderMonoBehaviour.GameManager.FightManager.StartBattle(country, districtToConquer.holder, districtToConquer);
        battleIsGoingToHappen = true;

        return true;
    }

    protected bool ConquerNeutralDistrict(List<DistrictInfo> neutralDistricts)
    {
        Debug.Log("Лидер обдумывает случай, когда есть нейтральные и нет вражеских районов.");

        // захватываем любой нейтральный.
        DistrictInfo districtToConquer = neutralDistricts[Random.Range(0, neutralDistricts.Count)];
        Country.AddNewDistrict(districtToConquer.districtFunc, true);
        ++LeaderMonoBehaviour.GameManager.gameSession.ConqueredNeutralDistrict;
        Debug.Log("Лидер " + leaderName + " решил захватить нейтральный район " + districtToConquer.DistrictName);

        if (!districtToConquer.HasGoldProduction)
        {
            Debug.Log("И сразу построил золото");
            districtToConquer.BuildGoldProduction();
        }

        return true;
    }

    /// <summary>
    /// Выбирает, какой район захватить - вражеский или нейтральный.
    /// </summary>
    /// <param name="neutralDistricts"></param>
    /// <param name="enemyDistricts"></param>
    protected virtual bool ConquerNeutralOrEnemyDistrict(List<DistrictInfo> neutralDistricts, List<DistrictInfo> enemyDistricts)
    {
        Debug.Log("Лидер обдумывает случай, когда есть нейтральные и вражеские районы.");

        // Атакуем, если выпал шанс.
        if (MakeProbabilityWeightedDecision(50))
        {
            // Проверяем, есть ли в армии юниты.
            if (country.Army.Units.Count == 0 || MakeProbabilityWeightedDecision(30))
            {
                BuyUnits();
            }

            // Если после этого всего, у нас остаются деньги на постройку улучшения, продолжаем.
            if (country.Gold > -LeaderMonoBehaviour.GameManager.gameSession.GameRules.CostOfGoldProduction)
            {
                // Если появились солдаты, атакуем, иначе просто нейтральный захватываем.
                if (country.Army.Units.Count > 0)
                {
                    // Атакуем вражеский.
                    return ConquerEnemyDistrict(enemyDistricts);
                }
            }
            else
            {
                return false;
            }
        }

        // Атакуем нейтральный.
        return ConquerNeutralDistrict(neutralDistricts);
    }

    /// <summary>
    /// Добавить Район districtInfoToAdd в список Районов listToCheck, если его там ещё нет.
    /// </summary>
    protected void AddIfNotAdded(DistrictInfo districtInfoToAdd, List<DistrictInfo> listToCheck)
    {
        int idOfDistrictToAdd = districtInfoToAdd.districtFunc.id;

        for (int i = 0; i < listToCheck.Count; i++)
        {
            if (listToCheck[i].districtFunc.id == idOfDistrictToAdd)
            {
                return;
            }
        }

        listToCheck.Add(districtInfoToAdd);
    }

    /// <summary>
    /// Проверка, остануться ли деньги на экстренную постройку "Добычи Золота", если потратить costOfProductionToCheck денег.
    /// </summary>
    /// <param name="costOfProductionToCheck">Какая трата (сумма) требует проверки (передавать сюда со знаком минус)</param>
    /// <returns>true, если строить безопасно.</returns>
    public bool CheckIfBuildingIsSafe(int costOfProductionToCheck)
    {
        return Country.Gold + costOfProductionToCheck >= -LeaderMonoBehaviour.GameManager.gameSession.GameRules.CostOfGoldProduction;
        // новее: return country.Gold >= -costOfProductionToCheck + -LeaderMonoBehaviour.GameManager.gameSession.gameRules.CostOfGoldProduction;
        // изначальное: return country.Gold >= -costOfProductionToCheck + LeaderMonoBehaviour.GameManager.gameSession.gameRules.CostOfGoldProduction;
    }

    /// <summary>
    /// Пытается построить какое-нибудь одно здание в районах, обходя их по мере их добавления в имперю.
    /// </summary>
    /// <returns>true, если действие считать успешно выполненным.</returns>
    protected virtual bool BuildRandomProduction()
    {
        // Делает массив построек. Каждое проверяет на возможность. Те, что можно, кладёт в отдельный массив и в нём рандомит.
        // Массив ситуации с бюджетом и потребностями.
        int[] allTypesOfProductionsEver = new int[4];
        // Проверим ситуацию с "Разработка Бонуса Района".
        if (CheckIfBuildingIsSafe(LeaderMonoBehaviour.GameManager.gameSession.GameRules.CostOfBonusProduction)) allTypesOfProductionsEver[0] = 1;
        else allTypesOfProductionsEver[0] = 0;
        // Проверим ситуацию с "Добыча Золота".
        if (Country.Gold >= -LeaderMonoBehaviour.GameManager.gameSession.GameRules.CostOfGoldProduction) allTypesOfProductionsEver[1] = 1;
        else allTypesOfProductionsEver[1] = 0;
        // Проверим ситуацию с "Бюро Безопасности".
        if (CheckIfBuildingIsSafe(LeaderMonoBehaviour.GameManager.gameSession.GameRules.CostOfAgentsProduction)) allTypesOfProductionsEver[2] = 1;
        else allTypesOfProductionsEver[2] = 0;
        // Проверим ситуацию с "Монументом".
        if (CheckIfBuildingIsSafe(LeaderMonoBehaviour.GameManager.gameSession.GameRules.CostOfMonument)) allTypesOfProductionsEver[3] = 1;
        else allTypesOfProductionsEver[3] = 0;

        // Проверим, позволяет ли бюджет построить хотя бы одно их этих зданий.
        bool atLeastOneIsAffortable = false;
        for (int i = 0; i < allTypesOfProductionsEver.Length; i++)
        {
            if (allTypesOfProductionsEver[i] != 0)
            {
                atLeastOneIsAffortable = true;
                break;
            }
        }
        if (!atLeastOneIsAffortable)
        {
            Debug.Log("Лидер " + leaderName + " Лидер не смог выбрать улучшение для постройки из-за бюджета. Золото: " + Country.Gold);
            return false;
        }

        // Если дошли до сюда, значит, можно построить по бюджету хотя бы одно здание.
        // Проходимся по всем Районам по очереди. Пробуем построить в первом возможном районе одно из отобранных по бюджету зданий.
        for (int i = 0; i < Country.DistrictsIds.Count; i++)
        {
            DistrictInfo district = LeaderMonoBehaviour.GameManager.gameSession.FindDistrictById(Country.DistrictsIds[i]);
            Debug.Log("Рассматриваем район " + district.DistrictName);

            // В каждом Районе смотрим, какое улучшение можно построить конкретно в нём.
            int[] allProductionsForThisDistrict = (int[])allTypesOfProductionsEver.Clone();

            //Debug.LogError(allProductionsForThisDistrict.Length + "   " + allProductionsForThisDistrict[0] + " " + allProductionsForThisDistrict[1] + " " + allProductionsForThisDistrict[2] + " " + allProductionsForThisDistrict[3]);

            // Проверим ситуацию с "Разработка Бонуса Района".
            if (district.DistrictBonus != 0 && district.DistrictBonus != 4 && !district.HasBonusProduction) allProductionsForThisDistrict[0] *= 1;
            else allProductionsForThisDistrict[0] = 0;
            // Проверим ситуацию с "Добыча Золота".
            if (!district.HasGoldProduction) allProductionsForThisDistrict[1] *= 1;
            else allProductionsForThisDistrict[1] = 0;
            // Проверим ситуацию с "Бюро Безопасности".
            if (!district.HasSecurityBureau) allProductionsForThisDistrict[2] *= 1;
            else allProductionsForThisDistrict[2] = 0;
            // Проверим ситуацию с "Монументом".
            if (!district.HasMonument) allProductionsForThisDistrict[3] *= 1;
            else allProductionsForThisDistrict[3] = 0;

            //Debug.LogError(allProductionsForThisDistrict.Length + "   " + allProductionsForThisDistrict[0] + " " + allProductionsForThisDistrict[1] + " " + allProductionsForThisDistrict[2] + " " + allProductionsForThisDistrict[3]);

            // Теперь сформируем лист всех возможных построек конкретно в этом Районе.
            // Лист пар готовых для постройки районов. В паре первое - индекс (номер улучшения), второе - потребность.
            List<System.Tuple<int, int>> productionsReadyToBuild = new List<System.Tuple<int, int>>();
            for (int j = 0; j < allTypesOfProductionsEver.Length; j++)
            {
                // В лист пар добавляем те, что не 0.
                // ИДЕЯ: Для других лидеров, выбираются только максимальные потребности, а не только те, что не 0.
                if (allProductionsForThisDistrict[j] != 0) productionsReadyToBuild.Add(new System.Tuple<int, int>(j, allProductionsForThisDistrict[j]));
            }
            // Если ничего нельзя построить конкретно в этом районе, смотрим следующий район.
            if (productionsReadyToBuild.Count == 0)
            {
                Debug.Log("В этом районе, ничего нельзя было построить!");
                continue;
            }

            Debug.Log("Столько улучшений можно построить в этом районе - " + productionsReadyToBuild.Count);

            // Если дошли до сюда, значит, постройка хотя бы одного улучшения по бюджету и возможности в этом районе, доступна.
            // Выьираем случайное и строим.
            int answer = productionsReadyToBuild[Random.Range(0, productionsReadyToBuild.Count)].Item1;

            //Debug.Log("Было выбрано улучшение с id - " + answer);

            switch (answer)
            {
                // Разработка Бонуса Района.
                case 0:
                    if (district.DistrictBonus != 0 && district.DistrictBonus != 4 && !district.HasBonusProduction)
                    {
                        district.BuildBonusProduction();
                        return true;
                    }
                    break;
                // Добыча Золота.
                case 1:
                    if (!district.HasGoldProduction)
                    {
                        district.BuildGoldProduction();
                        return true;
                    }
                    break;
                // Бюро Безопасности.
                case 2:
                    if (!district.HasSecurityBureau)
                    {
                        district.BuildSecurityBureau();
                        return true;
                    }
                    break;
                // Монумент.
                case 3:
                    if (!district.HasMonument)
                    {
                        district.BuildMonument();
                        return true;
                    }
                    break;
                default:
                    Debug.LogError("Какая-то ошибка.");
                    return false;
            }

            Debug.Log("Попытки постройки завершены из-за ошибки. Золото: " + Country.Gold);
            return false;
        }

        Debug.Log("Лидер " + leaderName + " Лидер не смог построить улучшение. Золото: " + Country.Gold);
        return false;
    }

    /// <summary>
    /// Пытается избавиться от убытка.
    /// Пытается построить Добычу Золота в Районе с Бонусом Золото. Если таких нет, то пытается построить улучшение "Добыча Золота".
    /// </summary>
    /// <returns>true, если проблема решена, иначе false.</returns>
    protected virtual bool HandleCrysis()
    {
        // Если хватит денег...
        if (Country.Gold >= -LeaderMonoBehaviour.GameManager.gameSession.GameRules.CostOfBonusProduction)
        {
            // Пытается построить улучшения "Разработка Бонуса Района" в Районах с Бонусом Золото.
            for (int i = 0; i < Country.DistrictsIds.Count; i++)
            {
                DistrictInfo district = LeaderMonoBehaviour.GameManager.gameSession.FindDistrictById(Country.DistrictsIds[i]);
                // Если "Разработка Бонуса Района" принесёт золото и улучшение ещё не построено, то сторим.
                if (district.DistrictBonus == 1 && !district.HasBonusProduction)
                {
                    district.BuildBonusProduction();

                    // Если проблема решена, прекращаем...
                    if (!CheckCrysis())
                    {
                        return true;
                    }
                    // иначе проверяем, хватает ли денег на продолжние.
                    else
                    {
                        if (Country.Gold < -LeaderMonoBehaviour.GameManager.gameSession.GameRules.CostOfBonusProduction)
                        {
                            Debug.Log("Лидер " + leaderName + " не смог избавиться от кризиса постройкой улучшения \"Разработка Бонуса Района\" и прекращает попытки!!");
                            break;
                        }
                    }
                    Debug.Log("Лидер " + leaderName + " не смог избавиться от кризиса постройкой улучшения \"Разработка Бонуса Района\", но продолжает попытки.");
                }
            }
        }

        // Если проблема ещё не решена, и если хватит денег...
        if (Country.Gold >= LeaderMonoBehaviour.GameManager.gameSession.GameRules.CostOfGoldProduction)
        {
            // Пытается пострить улучшения "Добыча Золота"
            for (int i = 0; i < Country.DistrictsIds.Count; i++)
            {
                DistrictInfo district = LeaderMonoBehaviour.GameManager.gameSession.FindDistrictById(Country.DistrictsIds[i]);
                // Если улучшение ещё не построено, то строим.
                if (!district.HasGoldProduction)
                {
                    district.BuildGoldProduction();

                    // Если проблема решена, прекращаем...
                    if (!CheckCrysis())
                    {
                        return true;
                    }
                    // иначе проверяем, хватает ли денег на продолжние.
                    else
                    {
                        if (Country.Gold < -LeaderMonoBehaviour.GameManager.gameSession.GameRules.CostOfGoldProduction)
                        {
                            Debug.Log("Лидер " + leaderName + " не смог избавиться от кризиса постройкой улучшения \"Добыча Золота\" и прекращает попытки!");
                            break;
                        }
                    }
                    Debug.Log("Лидер " + leaderName + " не смог избавиться от кризиса постройкой улучшения \"Добыча Золота\", но продолжает попытки.");
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Проверяет, теряет ли деньги Страна или зарабатывает.
    /// </summary>
    /// <returns>true, если убыток <= 0, false, если доход > 0.</returns>
    protected virtual bool CheckCrysis()
    {
        int sum = 0;
        for (int i = 0; i < Country.DistrictsIds.Count; i++)
        {
            sum += LeaderMonoBehaviour.GameManager.gameSession.FindDistrictById(Country.DistrictsIds[i]).districtFunc.GetGoldIncome();
        }

        if (sum <= 0) return true;
        else return false;
    }

    /// <summary>
    /// Запросить мир у этой страны со страной otherCountry.
    /// Базовая реализация принимает мир с 50% вероятностью.
    /// </summary>
    public virtual void RequestPeace(Country otherCountry)
    {
        Relationship relationship = LeaderMonoBehaviour.GameManager.gameSession.FindRelationship(otherCountry.CountryId, Country.CountryId);
        // Принимает мир с 50% вероятностью.
        if (MakeProbabilityWeightedDecision(50))
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
            Debug.Log($"Лидер {otherCountry.Leader.LeaderName} предложил мир Лидеру {leaderName}, и он согласился.");

            if (otherCountry.Leader.IsPlayer) LeaderMonoBehaviour.OpenDialogUI("Мы пока не готовы к этому.", false);
        }
    }

    public virtual void ConsiderToImproveArmyBeforeFight(int enemysArmyPower)
    {
        if (MakeProbabilityWeightedDecision(100))
        {
            Debug.Log("Лидер решил расширить армию перед боем.");
            BuyUnits();
        }
    }

    /// <summary>
    /// Выбирает юнита из армии, каким напасть.
    /// </summary>
    /// <returns></returns>
    public virtual Unit ChooseUnitToFight()
    {
        if (country.Army.Units.Count == 0)
        {
            Debug.Log("Армия Лидера " + leaderName + " была пуста и не получилось выбрать юнит для атаки!");
            return null;
        }
        else
        {
            int number = Random.Range(0, country.Army.Units.Count);
            Debug.Log("Лидер " + leaderName + " выбрал юнит из своей армии под номером " + number + $"({country.Army.Units[number].UnitName})");
            return country.Army.Units[number];
        }
    }

    /// <summary>
    /// Помагает принять случайное решение.
    /// </summary>
    /// <param name="probabilityOfTrue">какой процент (от 1 до probabilityOfTrue) будет считаться "да".</param>
    /// <returns>true, если "да", иначе false.</returns>
    public static bool MakeProbabilityWeightedDecision(int probabilityOfTrue)
    {
        int probability = Random.Range(1, 101);
        bool answer = probability <= probabilityOfTrue;
        Debug.Log($"Было принято случайное решение. Выпала вероятность решения: {probability}...");
        return answer;
    }

    /// <summary>
    /// Запретить заключение мира на столько ходов, сколько описано в Игровых Правилах.
    /// </summary>
    /// <param name="relationship">В отношениях каких стран нужна задержка</param>
    public void LockPeace(Relationship relationship)
    {
        relationship.NumberOfMoveToUnlockPeace = LeaderMonoBehaviour.GameManager.gameSession.CurrentMove
            + LeaderMonoBehaviour.GameManager.gameSession.GameRules.PeaceNegotiationsDelayAfterPeaceSuggestion;
    }

    /// <summary>
    /// Запретить заключение мира на delay ходов.
    /// </summary>
    /// <param name="relationship">В отношениях каких стран нужна задержка</param>
    public void LockPeace(Relationship relationship, int delay)
    {
        relationship.NumberOfMoveToUnlockPeace = LeaderMonoBehaviour.GameManager.gameSession.CurrentMove + delay;
    }

    private string defaultLine = "Приветсвую вас!";
    private string warLine = "Хотите заключить мир?";
    private string peaceAcceptedLine = "Мир принят.";
    private string warDeclarationFromThisLine = "Мы объявляем вам войну!";
    private string warDeclarationToThisLine = "Мы дадим отпор!";

    #region Специальные функции Лидера для Экрана Общения.
    // Для кнопки 1 специальной функции этого Лидера на Экране Общения.
    public virtual void SpecialDialogOption1()
    {
        Debug.Log("Была вызвана уникальная функция 1 у Базового Лидера.");
    }

    // (На будущее) Для кнопки 2 специальной функции этого Лидера на Экране Общения.
    public virtual void SpecialDialogOption2()
    {
        Debug.Log("Была вызвана уникальная функция 2 у Базового Лидера.");
    }
    #endregion
}
