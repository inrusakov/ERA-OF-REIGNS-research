using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavedSessionItem : MonoBehaviour
{
    private static MainMenuManager mainMenuManager;
    private int sessionId;
    [SerializeField] private TMP_Text nameForTheSession;
    [SerializeField] private TMP_Text lastDate;

    public static MainMenuManager MainMenuManagerVar { get => mainMenuManager; set => mainMenuManager = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateItem(int sessionId)
    {
        this.sessionId = sessionId;
        nameForTheSession.text = MainMenuManager.gameSessions[sessionId].SessionName;
        lastDate.text = "Последний запуск: " + MainMenuManager.gameSessions[sessionId].LastPlayingTime;
    }

    public void LoadSession()
    {
        MainMenuManager.sessionToStart = sessionId;
        //MainMenuManagerVar.LoadSession();
        // Запустим игру.
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void DeleteSession()
    {
        MainMenuManagerVar.DeleteListItem(sessionId);
    }
}
