using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс для хранения данных файла сохранения.
/// </summary>
public class SaveData
{
    // Последний открытый профиль.
    public int lastProfile;
    // Лист всех профилей.
    public List<Profile> profiles;

    public SaveData()
    {

    }
}
