using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Класс, предназначенный для управления приложением в главном меню игры.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    // Новая система сохранений:
    static public SaveData saveData;
    static public Profile currentProfile;

    // Лист всех сохранённых игр.
    //static public List<GameSession> gameSessions;
    // -1 - Начать новую игру. 0-n - Загрузить определённую игру.
    static public int sessionToStart;
    // Игровые правила.
    static public GameSession gameSessionToCreate;

    [SerializeField] Button loadSessionButton;

    [SerializeField] Canvas newGameUI;
    [SerializeField] TMP_Text nameForTheNewSession;

    [SerializeField] Canvas loadGameUI;
    [SerializeField] GameObject content;
    List<SavedSessionItem> savedSessionItems;
    [SerializeField] SavedSessionItem item = null;

    [SerializeField] HelpUI helpUI;

    [SerializeField] TMP_Text currentProfileUIText;
    [SerializeField] Canvas profilesUI;
    [SerializeField] ProfileItem profileItem = null;
    [SerializeField] TMP_Text newProfileNameText;

    [SerializeField] TMP_Dropdown difficultyDropdown;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            // Дессериализуем данные.
            if (File.Exists(Application.persistentDataPath + "/savedSessions.eofs"))
            {
                string json = File.ReadAllText(Application.persistentDataPath + "/savedSessions.eofs", Encoding.UTF8);
                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                //gameSessions = JsonConvert.DeserializeObject<List<GameSession>>(json, settings);
                saveData = JsonConvert.DeserializeObject<SaveData>(json, settings);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка загрузки игры! {e.Message}");
        }

        //// Если данных нет, значит это первый запуск или произошла ошибка. В любом случае, начнём как в первый раз.
        //if (gameSessions == null || gameSessions.Count == 0)
        //{
        //    gameSessions = new List<GameSession>();
        //    sessionToStart = -1;
        //    //loadSessionButton.gameObject.SetActive(false);
        //    loadSessionButton.interactable = false;
        //    ShowHelp();
        //}

        // Если данных нет, значит это первый запуск или произошла ошибка. В любом случае, начнём как в первый раз.
        if (saveData == null || saveData.profiles == null || saveData.profiles.Count == 0)
        {
            // Создаём новую SaveData и создаём первый профиль.
            saveData = new SaveData();
            saveData.profiles = new List<Profile>();
            saveData.profiles.Add(new Profile(System.Environment.UserName));
            saveData.lastProfile = 0;
        }

        // Автоматически загрузим тот профиль, который запускался последним.
        LoadProfile(saveData.lastProfile);
    }

    /// <summary>
    /// Загрузить новый профиль. При этом, обновляется и сохраняется переменная lastProfile.
    /// </summary>
    /// <param name="id">номер профиля в списке профилей.</param>
    public void LoadProfile(int id)
    {
        // В любой непонятной ситуации, будем загружать нулевой профиль.
        if (id >= saveData.profiles.Count || id < 0) id = 0;

        // Загрузим профиль.
        currentProfile = saveData.profiles[id];
        // Обновим последний запущенный профиль.
        saveData.lastProfile = id;

        currentProfileUIText.SetText($"Текущий Профиль: \"{currentProfile.profileName}\"");
        Debug.Log($"Загружен профиль: #{saveData.lastProfile}. \"{currentProfile.profileName}\"");

        SaveSessions();

        // Если данных нет, значит это первый запуск или произошла ошибка. В любом случае, начнём как в первый раз.
        if (currentProfile.gameSessions == null || currentProfile.gameSessions.Count == 0)
        {
            currentProfile.gameSessions = new List<GameSession>();
            sessionToStart = -1;
            //loadSessionButton.gameObject.SetActive(false);
            loadSessionButton.interactable = false;
            ShowHelp();
        }
        else loadSessionButton.interactable = true;

        profilesUI.gameObject.SetActive(false);
    }

    public void DeleteListItem(int id)
    {
        currentProfile.gameSessions.RemoveAt(id);
        RefreshLoadList();
    }

    public void StartNewSession()
    {
        // Создадим новую сессию и запустим её.
        //gameSessionToCreate = new GameSession(0);
        sessionToStart = -1;

        // TODO: Здесь передаём новые GameRules.
        if (difficultyDropdown.value == 0)
        {
            Debug.Log("Нормальная сложность 0");
            gameSessionToCreate = new GameSession(0);
        }
        else
        {
            Debug.Log("Сложность 1");
            gameSessionToCreate = new GameSession(0, new GameRules() { DifficultyInfluence = 2 });
        }

        Debug.Log("DifficultyInfluence: " + gameSessionToCreate.GameRules.DifficultyInfluence);

        // Проверим, дали ли название.
        if (string.IsNullOrEmpty(nameForTheNewSession.text) || nameForTheNewSession.text.StartsWith("\u200B"))
        {
            gameSessionToCreate.SessionName = "New Save" /*"Save - " + gameSessionToCreate.LastPlayingTime*/;
        }
        else
        {
            gameSessionToCreate.SessionName = nameForTheNewSession.text;
        }

        // Запустим новую игру.
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void CloseLoadSessionUI()
    {
        loadGameUI.gameObject.SetActive(false);
    }

    public void OpenLoadingSessionUI()
    {
        // Откроем менеджер сессий, где игрок выберет игру для загрузки.
        SavedSessionItem.MainMenuManagerVar = this;
        loadGameUI.gameObject.SetActive(true);
        RefreshLoadList();
    }

    public void RefreshLoadList()
    {
        // Очистим лист.
        SavedSessionItem[] childs = GameObject.Find("Content").GetComponentsInChildren<SavedSessionItem>();
        for (int i = 0; i < childs.Length; i++)
        {
            Destroy(childs[i].gameObject);
        }

        // Создадим лист.
        for (int i = 0; i < currentProfile.gameSessions.Count; i++)
        {
            GameObject item = Instantiate(this.item.gameObject);
            SavedSessionItem savedSessionItem = item.GetComponent<SavedSessionItem>();
            //item.transform.parent = content.transform;
            item.transform.SetParent(content.transform);
            savedSessionItem.CreateItem(i);
        }
    }

    public void OpenProfilesUI()
    {
        // Откроем окно с выбором профиля.
        ProfileItem.MainMenuManagerVar = this;
        profilesUI.gameObject.SetActive(true);
        RefreshProfileList();

    }

    public void CreateNewProfile()
    {
        // Проверим, дали ли название.
        if (string.IsNullOrEmpty(newProfileNameText.text) || newProfileNameText.text.StartsWith("\u200B"))
        {
            return;
        }

        saveData.profiles.Add(new Profile(newProfileNameText.text));

        newProfileNameText.text = "";
        RefreshProfileList();
    }

    public void DeleteProfileListItem(int id)
    {
        if (saveData.profiles.Count == 1)
        {
            Debug.LogWarning("Попытка удалить последний профиль. Так нельзя!");
            return;
        }

        saveData.profiles.RemoveAt(id);
        RefreshProfileList();
    }

    public void RefreshProfileList()
    {
        // Очистим лист.
        ProfileItem[] childs = GameObject.Find("ContentPROFILES").GetComponentsInChildren<ProfileItem>();
        for (int i = 0; i < childs.Length; i++)
        {
            Destroy(childs[i].gameObject);
        }

        // Создадим лист.
        for (int i = 0; i < saveData.profiles.Count; i++)
        {
            GameObject item = Instantiate(profileItem.gameObject);
            ProfileItem savedSessionItem = item.GetComponent<ProfileItem>();
            item.transform.SetParent(GameObject.Find("ContentPROFILES").transform);
            savedSessionItem.CreateItem(i);
        }
    }

    public void ShowHelp()
    {
        // Откроем Справку.
        helpUI.Open();
    }

    public static void SaveSessions()
    {
        // Debug.Log("Сохранено столько сессий: " + gameSessions.Count);

        try
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            //string jsonString = JsonConvert.SerializeObject(gameSessions, settings);
            string jsonString = JsonConvert.SerializeObject(saveData, settings);
            File.WriteAllText(Application.persistentDataPath + "/savedSessions.eofs", jsonString, Encoding.UTF8);

            Debug.Log(jsonString);
            Debug.Log("Сохранение завершено.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка сохранения игры! {e.Message}");
        }
    }

    public void ExitGame()
    {
        // Сериализуем данные и выйдим из игры.
        SaveSessions();
        Application.Quit();
    }
}
