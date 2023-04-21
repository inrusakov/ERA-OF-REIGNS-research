using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileItem : MonoBehaviour
{
    private static MainMenuManager mainMenuManager;
    private int profileId;

    [SerializeField] private TMP_Text profileName;
    [SerializeField] private TMP_Text renameForTheSession;
    [SerializeField] private Button deleteButton;

    public static MainMenuManager MainMenuManagerVar { get => mainMenuManager; set => mainMenuManager = value; }

    public void CreateItem(int profileId)
    {
        this.profileId = profileId;

        // Если это нулевой профиль, то он основной: удалять его нельзя.
        if (profileId == 0) deleteButton.interactable = false;
        else deleteButton.interactable = true;

        profileName.text = MainMenuManager.saveData.profiles[profileId].profileName;

        renameForTheSession.text = profileName.text;
    }
    public void LoadProfile()
    {
        MainMenuManagerVar.LoadProfile(profileId);
    }

    public void DeleteProfile()
    {
        MainMenuManagerVar.DeleteProfileListItem(profileId);
    }

    public void RenameProfile()
    {
        // Проверим, дали ли название.
        if (string.IsNullOrEmpty(renameForTheSession.text) || renameForTheSession.text.StartsWith("\u200B"))
        {
            // Ничего не делаем.
            return;
        }

        MainMenuManager.saveData.profiles[profileId].profileName = renameForTheSession.text;
        MainMenuManagerVar.RefreshProfileList();
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
