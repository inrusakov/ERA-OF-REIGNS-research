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
    // Лист всех сохранённых игр.
    static public List<GameSession> gameSessions;
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
                gameSessions = JsonConvert.DeserializeObject<List<GameSession>>(json, settings);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка загрузки игры! {e.Message}");
        }

        // Если данных нет, значит это первый запуск или произошла ошибка. В любом случае, начнём как в первый раз.
        if (gameSessions == null || gameSessions.Count == 0)
        {
            gameSessions = new List<GameSession>();
            sessionToStart = -1;
            //loadSessionButton.gameObject.SetActive(false);
            loadSessionButton.interactable = false;
            ShowHelp();
        }
    }

    public void DeleteListItem(int id)
    {
        gameSessions.RemoveAt(id);
        RefreshList();
    }

    public void StartNewSession()
    {
        // TODO: Здесь передаём новые GameRules.

        // Создадим новую сессию и запустим её.
        gameSessionToCreate = new GameSession(0);
        sessionToStart = -1;

        // Проверим, дали ли название.
        if (string.IsNullOrEmpty(nameForTheNewSession.text) || nameForTheNewSession.text.StartsWith("\u200B"))
        {
            gameSessionToCreate.SessionName = "Save - " + gameSessionToCreate.LastPlayingTime;
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
        RefreshList();
    }

    void RefreshList()
    {
        // Очистим лист.
        SavedSessionItem[] childs = GameObject.Find("Content").GetComponentsInChildren<SavedSessionItem>();
        for (int i = 0; i < childs.Length; i++)
        {
            Destroy(childs[i].gameObject);
        }

        // Создадим лист.
        for (int i = 0; i < gameSessions.Count; i++)
        {
            GameObject item = Instantiate(this.item.gameObject);
            SavedSessionItem savedSessionItem = item.GetComponent<SavedSessionItem>();
            //item.transform.parent = content.transform;
            item.transform.SetParent(content.transform);
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
            string jsonString = JsonConvert.SerializeObject(gameSessions, settings);
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
