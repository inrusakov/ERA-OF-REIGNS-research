using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Класс, предназначенный для управления приложением во время игрового процесса, а также хранения главных ссылок.
/// </summary>
public class GameManager : MonoBehaviour
{
    // Текущая игровая сессия.
    public GameSession gameSession;

    #region Игровая Камера.
    // Игровая Камера.
    [SerializeField] CameraController cameraController;
    public CameraController CameraController { get => cameraController; }
    #endregion

    #region UI
    // -------------------------------------UI.
    // Диалоговое окно - сообщение Игроку.
    public DistrictUI districtUI;

    public Button buttonForNextMove;
    public TMP_Text moveUICounter;

    // 0 - золото, 1 - железо, 2 - животноводство, 3 - тайная полиция.
    public List<TMP_Text> stats;
    [SerializeField] List<TMP_Text> victoryPointsStats;

    // Меню на выход.
    [SerializeField] Canvas pauseUI;
    [SerializeField] TMP_Text sessionNamePauseUI;

    // Меню Армии.
    [SerializeField] Canvas armyUIBackground;
    [SerializeField] GameObject armyUI;

    // Меню окончания игры.
    [SerializeField] GameObject gameOverUI;
    [SerializeField] GameObject gameOverInDetailsUI;
    [SerializeField] TMP_Text winnerName;
    [SerializeField] TMP_Text gameOverMessage;

    #endregion

    #region Объекты
    // Игровая карта.
    static public List<District> districts;

    // Указательная стрелка на Район.
    public GameObject AttentionSign { get => attentionSign; }

    [SerializeField] GameObject attentionSign;
    #endregion

    public bool developerState;
    public NearbyDistrictsCreator nearbyDistrictsCreator;

    // Менеджер боя.
    [SerializeField] FightManager fightManager;
    public FightManager FightManager { get => fightManager; set => fightManager = value; }

    // Start is called before the first frame update
    void Start()
    {
        if (developerState)
        {
            LoadDistricts();
            // Далее специальный код разработчика... Например,
            // nearbyDistrictsCreator.readyToStart = true;
        }
        else
        {
            LoadDistricts();

            // Дадим всем Районам-функциям их Id.
            for (int i = 0; i < districts.Count; i++)
            {
                districts[i].id = i;
            }
            // Назначим статическое поле GameManager Лидерам-функциям, до создания Лидеров, чтобы при их создании, они могли обращаться к нему.
            LeaderMonoBehaviour.GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            districtUI.Close();

            // Узнаем, загружаем ли мы игру, или создаём новую:
            // Новая игра.
            if (MainMenuManager.gameSessions == null || MainMenuManager.sessionToStart == -1)
            {
                if (MainMenuManager.gameSessions == null)
                {
                    Debug.LogWarning("Режим Дебага! Старые сессии будут удалены после сохранения в этом режиме!");
                    MainMenuManager.gameSessions = new List<GameSession>(0);
                }
                if (MainMenuManager.gameSessionToCreate == null)
                {
                    MainMenuManager.gameSessionToCreate = new GameSession(0);
                    MainMenuManager.gameSessionToCreate.SessionName = "Debug save " + System.DateTime.Now;
                }

                gameSession = MainMenuManager.gameSessionToCreate;
                // И сразу сохраним эту игровую сессию.
                MainMenuManager.gameSessions.Add(gameSession);

                // Создаём Районы.
                for (int i = 0; i < districts.Count; i++)
                {
                    DistrictInfo districtInfo = new DistrictInfo(districts[i]);
                    gameSession.DistrictsInfos.Add(districtInfo);
                    // Установим связи между районами-информациями и районами-функциями.
                    districts[i].districtInfo = districtInfo;
                    districtInfo.districtFunc = districts[i];
                    // Сгенерируем Бонус Района.
                    districts[i].districtInfo.GenerateBonus();
                }

                // Cоздаём Лидеров и Страны.
                CreateLeadersAndCountries();

                // Создаём отношения между Странами.
                gameSession.AddUnaddedRelationships();
            }
            // Загружаем игровую сессию.
            else
            {
                gameSession = MainMenuManager.gameSessions[MainMenuManager.sessionToStart];

                // Создаём Районы.
                for (int i = 0; i < districts.Count; i++)
                {
                    // Установим связи между районами-информациями и районами-функциями просто по соответствию индекса.
                    districts[i].districtInfo = gameSession.DistrictsInfos[i];
                    gameSession.DistrictsInfos[i].districtFunc = districts[i];
                }

                // Загружаем Лидеров и Страны.
                LoadLeadersAndCountries();
                // Чтобы не пропустился ход после загрузки, отнимем один.
                --gameSession.CurrentMove;

                // Проверим, не закончилась ли игра.
                if (gameSession.GameOver)
                {
                    if (gameSession.WinnerCountryId == -1) OpenGameOverUI(null, gameSession.GameOverMessage);
                    else OpenGameOverUI(gameSession.Countries[gameSession.WinnerCountryId], gameSession.GameOverMessage);
                }

                // Ходим без вычета хода.
            }

            // После завершения всех настроек, дадим первый ход и сериализуем данные.
            gameSession.UpdateLastPlayingTime();
            MainMenuManager.SaveSessions();
            UpdateMoveCounter();
            UpdateStats();
            gameSession.Countries[gameSession.CurrentCountry].Leader.MakeMove();
        }
    }

    private void LoadDistricts()
    {
        // Получим список всех Районов-функционалов на Игровой Карте.
        District[] childs = GameObject.Find("World_map").GetComponentsInChildren<District>();
        districts = childs.Cast<District>().ToList();
    }

    public void Save()
    {
        MainMenuManager.SaveSessions();
    }

    /// <summary>
    /// Проверка, умер ли игрок.
    /// </summary>
    /// <returns>true, если да.</returns>
    private bool CheckIfPlayerIsDead()
    {
        Country playerCountry = null;
        for (int i = 0; i < gameSession.Countries.Count; i++)
        {
            if (gameSession.Countries[i].Leader.IsPlayer)
            {
                playerCountry = gameSession.Countries[i];
                break;
            }
        }

        if (playerCountry.DistrictsIds.Count == 0)
        {
            gameSession.EndGame($"Игрок потерпел поражение - у него не осталось Районов.", -1);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Проверка, есть ли военная победа.
    /// </summary>
    /// <returns>true, если есть.</returns>
    private bool CheckForWarVictory()
    {
        int countriesThatNotDead = 0;
        int idOfTheCountry = -1;

        for (int i = 0; i < gameSession.Countries.Count; i++)
        {
            if (gameSession.Countries[i].DistrictsIds.Count != 0)
            {
                ++countriesThatNotDead;
                idOfTheCountry = i;
            }

            if (countriesThatNotDead > 1)
            {
                return false;
            }
        }

        gameSession.EndGame($"Военная Победа у страны, которая захватила весь мир!", idOfTheCountry);
        return true;
    }

    /// <summary>
    /// Проверка, закончились ли ходы.
    /// </summary>
    /// <returns>true, если да.</returns>
    private bool CheckForEndAfterMoves()
    {
        GameRules gameRules = gameSession.GameRules;

        if (gameRules.EndGameAfterMoves)
        {
            if (gameSession.CurrentMove >= gameRules.EndGameAfterNumberOfMoves)
            {
                int maxNumberOfVictoryPoints = gameSession.Countries[0].VictoryPoints;
                List<int> countriesWithMaxVictoryPoints = new List<int>();

                for (int i = 0; i < gameSession.Countries.Count; i++)
                {
                    // Если это новый максимум, то обнулим список стран с прошлым максимумом.
                    if (gameSession.Countries[i].VictoryPoints > maxNumberOfVictoryPoints)
                    {
                        maxNumberOfVictoryPoints = gameSession.Countries[i].VictoryPoints;
                        countriesWithMaxVictoryPoints = new List<int>();
                        countriesWithMaxVictoryPoints.Add(i);
                    }
                    // Если у какой-то страны столько же очков, сколько у максимума, то добавим в список стран с максимумом.
                    else if (gameSession.Countries[i].VictoryPoints == maxNumberOfVictoryPoints)
                    {
                        countriesWithMaxVictoryPoints.Add(i);
                    }
                }

                // Если победитель один - выигрывает он.
                if (countriesWithMaxVictoryPoints.Count == 1)
                {
                    gameSession.EndGame($"Победа по достижению {gameRules.EndGameAfterNumberOfMoves} ходов " +
                        $"у страны с наибольшим количеством Очков Победы ({maxNumberOfVictoryPoints})!", countriesWithMaxVictoryPoints[0]);
                    return true;
                }
                // Если победителей несколько...
                else
                {
                    // Выберем игрока, если он есть.
                    for (int i = 0; i < countriesWithMaxVictoryPoints.Count; i++)
                    {
                        if (gameSession.Countries[countriesWithMaxVictoryPoints[i]].Leader.IsPlayer)
                        {
                            gameSession.EndGame($"Победа по достижению {gameRules.EndGameAfterNumberOfMoves} ходов " +
                                $"у страны с наибольшим количеством Очков Победы ({maxNumberOfVictoryPoints})!", countriesWithMaxVictoryPoints[i]);
                            return true;
                        }
                    }

                    // Если дошли до сюда, то игрока нет - выбираем случайного победителя.
                    gameSession.EndGame($"Победа по достижению {gameRules.EndGameAfterNumberOfMoves} ходов " +
                                $"у случайной из стран с наибольшим количеством Очков Победы ({maxNumberOfVictoryPoints})!",
                                countriesWithMaxVictoryPoints[Random.Range(0, countriesWithMaxVictoryPoints.Count)]);
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    ///  Проверка, есть ли победа по Очкам Победы.
    /// </summary>
    /// <returns>true, если есть.</returns>
    private bool CheckForVictoryPointsVictory()
    {
        GameRules gameRules = gameSession.GameRules;

        if (gameRules.EndGameAfterVictoryPoints)
        {
            List<int> countriesIdsWithVictoryPointsToWin = new List<int>();
            for (int i = 0; i < gameSession.Countries.Count; i++)
            {
                if (gameSession.Countries[i].VictoryPoints >= gameRules.EndGameAfterNumberOfVictoryPoints)
                {
                    // Если это игрок, то побеждает он.
                    if (gameSession.Countries[i].Leader.IsPlayer)
                    {
                        gameSession.EndGame($"Победа по достижению {gameRules.EndGameAfterNumberOfVictoryPoints} Очков Победы!", i);
                        return true;
                    }
                    else
                    {
                        countriesIdsWithVictoryPointsToWin.Add(i);
                    }
                }
            }

            if (countriesIdsWithVictoryPointsToWin.Count == 0)
            {
                // Ещё никто не победил.
            }
            else
            {
                // Здесь процесс выбора одного из них. На данный момент побеждает первый в списке.
                gameSession.EndGame($"Победа по достижению {gameRules.EndGameAfterNumberOfVictoryPoints} Очков Победы!", countriesIdsWithVictoryPointsToWin[0]);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Проверяет, закончилась ли игра. Если закончилась - заносим данные о победителе в сессию.
    /// </summary>
    private void CheckIfGameIsOver()
    {
        if (CheckIfPlayerIsDead()) return;
        if (CheckForWarVictory()) return;
        if (CheckForEndAfterMoves()) return;
        if (CheckForVictoryPointsVictory()) return;
    }

    public void OpenGameOverUI(Country winnerCountry, string message)
    {
        gameOverUI.SetActive(true);
        gameOverInDetailsUI.SetActive(true);
        cameraController.LockCamera(true);

        if (winnerCountry == null)
        {
            winnerName.text = "Отсутсвует";
            winnerName.color = Color.black;
        }
        else
        {
            winnerName.text = winnerCountry.Leader.LeaderName;
            winnerName.color = winnerCountry.GetCountryColor();
        }

        gameOverMessage.text = message;
    }

    public void OpenGameOverInDetailsUI()
    {
        gameOverInDetailsUI.SetActive(true);
        cameraController.LockCamera(true);
    }

    public void CloseGameOverInDetailsUI()
    {
        gameOverInDetailsUI.SetActive(false);
        cameraController.LockCamera(false);
    }

    public void OpenPauseUI()
    {
        Save();
        sessionNamePauseUI.text = $"имя сессии:\n\"{gameSession.SessionName}\"";
        pauseUI.gameObject.SetActive(true);
        cameraController.LockCamera(true);
    }

    public void ClosePauseUI()
    {
        pauseUI.gameObject.SetActive(false);
        cameraController.LockCamera(false);
    }

    public void OpenArmyMenu()
    {
        armyUIBackground.gameObject.SetActive(true);
        cameraController.LockCamera(true);
        armyUI.GetComponent<ArmyUI>().Open(gameSession.Countries[gameSession.CurrentCountry], 0);
    }

    public void CloseArmyMenu()
    {
        armyUIBackground.gameObject.SetActive(false);
        cameraController.LockCamera(false);
        armyUI.GetComponent<ArmyUI>().Close();
    }

    public void ExitSession()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void ShowHelp()
    {
        //MainMenuManager.
    }

    /// <summary>
    /// Увеличивает счётчик ходов на 1 и отображает информацию о ходе в moveUICounter.
    /// При этом учитывает, нужно ли показывать, из скольких ходов игровая сессия.
    /// </summary>
    void UpdateMoveCounter()
    {
        if (gameSession.GameRules.EndGameAfterMoves)
            moveUICounter.text = $"Ход {++gameSession.CurrentMove}/{gameSession.GameRules.EndGameAfterNumberOfMoves}";
        else
            moveUICounter.text = $"Ход {++gameSession.CurrentMove}";
    }

    public void UpdateStats()
    {
        // Обновим показатели страны.
        stats[0].text = gameSession.Countries[gameSession.CurrentCountry].Gold.ToString();
        stats[1].text = gameSession.Countries[gameSession.CurrentCountry].Iron.ToString();
        stats[2].text = gameSession.Countries[gameSession.CurrentCountry].Horses.ToString();
        stats[3].text = gameSession.Countries[gameSession.CurrentCountry].Agents.ToString();

        // Обновим Очки Победы у кнопок вызова функции.
        for (int i = 0; i < victoryPointsStats.Count; i++)
        {
            victoryPointsStats[i].text = gameSession.Countries[i].VictoryPoints.ToString();
        }

        // Залочим погибших.
        for (int i = 0; i < gameSession.Countries.Count; i++)
        {
            if (gameSession.Countries[i].DistrictsIds.Count == 0) buttonsToOpenDialogsWithLeaders[i].interactable = false;
        }
    }

    public void NextMove()
    {
        gameSession.ConqueredNeutralDistrict = 0;
        // Проведём обслуживание Районов походившего Лидера.
        gameSession.Countries[gameSession.CurrentCountry].MainteneAllDistricts();

        // Проверим, закончилась ли игра.
        if (!gameSession.GameOver)
        {
            CheckIfGameIsOver();
            // Если игра теперь окончена, отобразим экран конца.
            if (gameSession.GameOver)
            {
                if (gameSession.WinnerCountryId == -1) OpenGameOverUI(null, gameSession.GameOverMessage);
                else OpenGameOverUI(gameSession.Countries[gameSession.WinnerCountryId], gameSession.GameOverMessage);
                // OpenGameOverUI(gameSession.Countries[gameSession.WinnerCountryId], gameSession.GameOverMessage);
            }
        }

        // Узнаем, какая страна сейчас будет ходить.
        if (gameSession.CurrentCountry + 1 == gameSession.Countries.Count)
        {
            // Если ходивший Лидер - последний в списке, то начинаем назначать ходы с начала списка.
            gameSession.CurrentCountry = 0;
            // Увеличиваем счётчик ходов, когда прошли круг.
            UpdateMoveCounter();
        }
        else
        {
            ++gameSession.CurrentCountry;

            //// Проверим условия конца игры, если конца игры ещё не было.
            //if (!gameSession.GameOver &&
            //    gameSession.GameRules.EndGameAfterMoves && gameSession.GameRules.EndGameAfterNumberOfMoves < gameSession.CurrentMove ||
            //    gameSession.GameRules.EndGameAfterVictoryPoints && gameSession.GameRules.EndGameAfterNumberOfVictoryPoints == MaxOfCountriesVictoryPoints())
            //{
            //    // Конец Игры. Покажем результаты на экране.
            //    Debug.LogWarning("Конец Игры.");
            //}
        }

        // Узнаем, начал ли ходить ИИ-Лидер. Заблокируем кнопку след. хода, если ходит не игрок.
        if (gameSession.Countries[gameSession.CurrentCountry].Leader.LeaderMonoBehaviour.gameObject.tag != "Player-Leader")
        {
            buttonForNextMove.interactable = false;
        }
        else
        {
            buttonForNextMove.interactable = true;
        }

        // Просим следующую Страну походить.
        UpdateStats();
        gameSession.Countries[gameSession.CurrentCountry].Leader.MakeMove();
        Save();
    }

    // Update is called once per frame
    void Update()
    {

    }

    int MaxOfCountriesVictoryPoints()
    {
        return 0;
    }

    public void NextMoveRequestedByPlayerViaButton()
    {
        //leaders[currentCountryToMakeTheMove].MoveIsFinished = true;
        cameraController.FreeCamera();
        districtUI.Close();
        NextMove();
    }

    void LoadLeadersAndCountries()
    {
        for (int i = 0; i < gameSession.Countries.Count; i++)
        {
            GameObject spawnedLeader;
            // Инстанциируем префаб объекта Лидера.
            spawnedLeader = Instantiate(leadersPrefabs[i]);
            // Назначим ему родителя-путышку, чтобы было легче искать в будущем.
            spawnedLeader.transform.parent = leadersParentObject.transform;
            // Получим скрипт-абстракцию этого Лидера.
            LeaderMonoBehaviour leaderMonoBehaviour = spawnedLeader.GetComponent<LeaderMonoBehaviour>();

            // Назначим скрипт-характер для этого Лидера. Просто по индексу ищем соответствия.
            leaderMonoBehaviour.leader = gameSession.Countries[i].Leader;
            // Создадим обратную ссылку для скрипта-характера на скрипт-абстракцию.
            leaderMonoBehaviour.SetLinkToLeaderMonoBehaviourForLeader();
            // Создадим обратную ссылку для этого лидера на страну этого лидера.
            leaderMonoBehaviour.leader.Country = gameSession.Countries[i];

            // Добавим соответствующей кнопке, вызов диалога с Лидером.
            buttonsToOpenDialogsWithLeaders[i].onClick.AddListener(leaderMonoBehaviour.OpenDialogUI);

            // Для каждого Района этой страны назначим обратную ссылку на эту Страну.
            for (int districtIndex = 0; districtIndex < gameSession.Countries[i].DistrictsIds.Count; districtIndex++)
            {
                gameSession.FindDistrictById(gameSession.Countries[i].DistrictsIds[districtIndex]).holder = gameSession.Countries[i];
            }
        }

        // Теперь установим соответствие между Районами-информациями и Районами-функциями.
        for (int i = 0; i < districts.Count; i++)
        {
            // И обновим цвета Районов под их владельцев.
            districts[i].LoadHolder(gameSession.DistrictsInfos[i]);
        }
    }

    public GameObject leadersParentObject;
    public List<GameObject> leadersPrefabs;
    public List<Button> buttonsToOpenDialogsWithLeaders;
    /// <summary>
    /// Создать Лидеров для Новой игры.
    /// </summary>
    void CreateLeadersAndCountries()
    {
        GameObject spawnedLeader;
        for (int i = 0; i < leadersPrefabs.Count; ++i)
        {
            // Инстанциируем префаб объекта Лидера.
            spawnedLeader = Instantiate(leadersPrefabs[i]);
            // Назначим ему родителя-путышку, чтобы было легче искать в будущем.
            spawnedLeader.transform.parent = leadersParentObject.transform;
            // Получим скрипт-абстракцию этого Лидера.
            LeaderMonoBehaviour leaderMonoBehaviour = spawnedLeader.GetComponent<LeaderMonoBehaviour>();

            // Создадим и назначим скрипт-характер для этого Лидера.
            switch (i)
            {
                // Игрок.
                case 0:
                    leaderMonoBehaviour.leader = new ThePlayer(leaderMonoBehaviour);
                    break;
                // The Uniter.
                case 1:
                    leaderMonoBehaviour.leader = new TheUniter(leaderMonoBehaviour);
                    break;
                // The Conqueror.
                case 2:
                    leaderMonoBehaviour.leader = new TheConqueror(leaderMonoBehaviour);
                    break;
                // The Сollector.
                case 3:
                    leaderMonoBehaviour.leader = new TheСollector(leaderMonoBehaviour);
                    break;

                // Лидер без характера.
                default:
                    leaderMonoBehaviour.leader = new Leader(leaderMonoBehaviour);
                    break;
            }
            // Созданной внутри конструктора Лидера Стране, назначим её id.
            leaderMonoBehaviour.leader.Country.CountryId = i;
            // Теперь эту Страну, добавим в gameSession.
            gameSession.Countries.Add(leaderMonoBehaviour.leader.Country);

            // Добавим соответствующей кнопке, вызов диалога с Лидером.
            buttonsToOpenDialogsWithLeaders[i].onClick.AddListener(leaderMonoBehaviour.OpenDialogUI);

            // Теперь добавим первый Район Стране Лидера. Это начало игры, поэтому не рассматриваем null случай.
            District districtToAdd = GetRandomDistrict(true);
            leaderMonoBehaviour.leader.Country.AddNewDistrict(districtToAdd, false);
        }
    }

    /// <summary>
    /// Возвращает случайный Район Игровой Карты.
    /// </summary>
    /// <param name="withoutHolder">Возвратить только Район без владельца?</param>
    /// <returns>Случайный Район на Игровой Карте. Если нужны Районы без владельца и таких нет, то null.</returns>
    public District GetRandomDistrict(bool withoutHolder)
    {
        District district;
        // Если нужно найти Район без владельца, ...
        if (withoutHolder)
        {
            // ... то проверим сначала, есть ли такие.
            bool isOkay = false;
            for (int i = 0; i < districts.Count; ++i)
            {
                if (districts[i].districtInfo.holder == null)
                {
                    isOkay = true;
                    break;
                }
            }

            // Если есть, то ищем, пока не найдём.
            if (isOkay)
            {
                do
                {
                    district = districts[Random.Range(0, districts.Count)];
                } while (district.districtInfo.holder != null);
            }
            // Иначе, возвращаем null.
            else
            {
                Debug.LogWarning("Искался случайный Район без владельца, но таких нет!");
                return null;
            }
        }
        // Иначе, если подойдёт любой Район, то берём первый сгенерированный.
        else
        {
            district = districts[Random.Range(0, districts.Count)];
        }

        return district;
    }
}
