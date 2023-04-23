using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// �����, ��������������� ��� ���������� ����������� � ������� ���� ����.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    // ����� ������� ����������:
    static public SaveData saveData;
    static public Profile currentProfile;

    // ���� ���� ���������� ���.
    //static public List<GameSession> gameSessions;
    // -1 - ������ ����� ����. 0-n - ��������� ����������� ����.
    static public int sessionToStart;
    // ������� �������.
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
            // �������������� ������.
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
            Debug.LogError($"������ �������� ����! {e.Message}");
        }

        //// ���� ������ ���, ������ ��� ������ ������ ��� ��������� ������. � ����� ������, ����� ��� � ������ ���.
        //if (gameSessions == null || gameSessions.Count == 0)
        //{
        //    gameSessions = new List<GameSession>();
        //    sessionToStart = -1;
        //    //loadSessionButton.gameObject.SetActive(false);
        //    loadSessionButton.interactable = false;
        //    ShowHelp();
        //}

        // ���� ������ ���, ������ ��� ������ ������ ��� ��������� ������. � ����� ������, ����� ��� � ������ ���.
        if (saveData == null || saveData.profiles == null || saveData.profiles.Count == 0)
        {
            // ������ ����� SaveData � ������ ������ �������.
            saveData = new SaveData();
            saveData.profiles = new List<Profile>();
            saveData.profiles.Add(new Profile(System.Environment.UserName));
            saveData.lastProfile = 0;
        }

        // ������������� �������� ��� �������, ������� ���������� ���������.
        LoadProfile(saveData.lastProfile);
    }

    /// <summary>
    /// ��������� ����� �������. ��� ����, ����������� � ����������� ���������� lastProfile.
    /// </summary>
    /// <param name="id">����� ������� � ������ ��������.</param>
    public void LoadProfile(int id)
    {
        // � ����� ���������� ��������, ����� ��������� ������� �������.
        if (id >= saveData.profiles.Count || id < 0) id = 0;

        // �������� �������.
        currentProfile = saveData.profiles[id];
        // ������� ��������� ���������� �������.
        saveData.lastProfile = id;

        currentProfileUIText.SetText($"������� �������: \"{currentProfile.profileName}\"");
        Debug.Log($"�������� �������: #{saveData.lastProfile}. \"{currentProfile.profileName}\"");

        SaveSessions();

        // ���� ������ ���, ������ ��� ������ ������ ��� ��������� ������. � ����� ������, ����� ��� � ������ ���.
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
        // �������� ����� ������ � �������� �.
        //gameSessionToCreate = new GameSession(0);
        sessionToStart = -1;

        // TODO: ����� ������� ����� GameRules.
        if (difficultyDropdown.value == 0)
        {
            Debug.Log("���������� ��������� 0");
            gameSessionToCreate = new GameSession(0);
        }
        else
        {
            Debug.Log("��������� 1");
            gameSessionToCreate = new GameSession(0, new GameRules() { DifficultyInfluence = 2 });
        }

        Debug.Log("DifficultyInfluence: " + gameSessionToCreate.GameRules.DifficultyInfluence);

        // ��������, ���� �� ��������.
        if (string.IsNullOrEmpty(nameForTheNewSession.text) || nameForTheNewSession.text.StartsWith("\u200B"))
        {
            gameSessionToCreate.SessionName = "New Save" /*"Save - " + gameSessionToCreate.LastPlayingTime*/;
        }
        else
        {
            gameSessionToCreate.SessionName = nameForTheNewSession.text;
        }

        // �������� ����� ����.
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void CloseLoadSessionUI()
    {
        loadGameUI.gameObject.SetActive(false);
    }

    public void OpenLoadingSessionUI()
    {
        // ������� �������� ������, ��� ����� ������� ���� ��� ��������.
        SavedSessionItem.MainMenuManagerVar = this;
        loadGameUI.gameObject.SetActive(true);
        RefreshLoadList();
    }

    public void RefreshLoadList()
    {
        // ������� ����.
        SavedSessionItem[] childs = GameObject.Find("Content").GetComponentsInChildren<SavedSessionItem>();
        for (int i = 0; i < childs.Length; i++)
        {
            Destroy(childs[i].gameObject);
        }

        // �������� ����.
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
        // ������� ���� � ������� �������.
        ProfileItem.MainMenuManagerVar = this;
        profilesUI.gameObject.SetActive(true);
        RefreshProfileList();

    }

    public void CreateNewProfile()
    {
        // ��������, ���� �� ��������.
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
            Debug.LogWarning("������� ������� ��������� �������. ��� ������!");
            return;
        }

        saveData.profiles.RemoveAt(id);
        RefreshProfileList();
    }

    public void RefreshProfileList()
    {
        // ������� ����.
        ProfileItem[] childs = GameObject.Find("ContentPROFILES").GetComponentsInChildren<ProfileItem>();
        for (int i = 0; i < childs.Length; i++)
        {
            Destroy(childs[i].gameObject);
        }

        // �������� ����.
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
        // ������� �������.
        helpUI.Open();
    }

    public static void SaveSessions()
    {
        // Debug.Log("��������� ������� ������: " + gameSessions.Count);

        try
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            //string jsonString = JsonConvert.SerializeObject(gameSessions, settings);
            string jsonString = JsonConvert.SerializeObject(saveData, settings);
            File.WriteAllText(Application.persistentDataPath + "/savedSessions.eofs", jsonString, Encoding.UTF8);

            Debug.Log(jsonString);
            Debug.Log("���������� ���������.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"������ ���������� ����! {e.Message}");
        }
    }

    public void ExitGame()
    {
        // ����������� ������ � ������ �� ����.
        SaveSessions();
        Application.Quit();
    }
}
