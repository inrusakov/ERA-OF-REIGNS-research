using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Profile
{
    // ��� �������.
    public string profileName;
    // ���� ���� ���������� ���.
    public List<GameSession> gameSessions;

    public Profile(string profileName)
    {
        this.profileName = profileName;
        gameSessions = new List<GameSession>();
    }

    public Profile()
    {

    }
}
