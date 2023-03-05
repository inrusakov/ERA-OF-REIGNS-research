using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Представляет собой игровую сессию (для сохранения, загрузки и хранения игровых данных).
/// Если нужно будет обновить класс сессии, то можно будет просто создать наследника.
/// </summary>
public class GameSession
{
    // Имя этой сессии.
    private string sessionName;
    // Дата последнего запуска этой сессии.
    private string lastPlayingTime;
    // Карта этой сессии.
    private int mapId;

    // Страны-информации этой сессии.
    private List<Country> countries;
    // Список районов-информаций данной карты этой сессии.
    private List<DistrictInfo> districtsInfos;
    // Список отношений между странами.
    private List<Relationship> relationships;

    // Текущий ход.
    private int currentMove;
    // Номер Лидера, который совершает текущий ход.
    private int currentCountry = 0;
    // Сколько нейтральных Районов захватил Лидер, чей ход сейчас, за этот ход.
    private int conqueredNeutralDistrict;

    // Игровые правила этой сессии.
    private GameRules gameRules;

    // Настал ли у этой сессии конец игры?
    private bool gameOver;
    // Сообщение о конце игры.
    private string gameOverMessage;
    // Победившая страна.
    private int winnerCountryId;

    public string SessionName { get => sessionName; set => sessionName = value; }
    public string LastPlayingTime { get => lastPlayingTime; set => lastPlayingTime = value; }
    public int MapId { get => mapId; set => mapId = value; }
    public List<Country> Countries { get => countries; set => countries = value; }
    public List<DistrictInfo> DistrictsInfos { get => districtsInfos; set => districtsInfos = value; }
    public List<Relationship> Relationships { get => relationships; set => relationships = value; }
    public int CurrentMove { get => currentMove; set => currentMove = value; }
    public int CurrentCountry { get => currentCountry; set => currentCountry = value; }
    public int ConqueredNeutralDistrict { get => conqueredNeutralDistrict; set => conqueredNeutralDistrict = value; }
    public GameRules GameRules { get => gameRules; set => gameRules = value; }
    public bool GameOver { get => gameOver; set => gameOver = value; }
    public string GameOverMessage { get => gameOverMessage; set => gameOverMessage = value; }
    public int WinnerCountryId { get => winnerCountryId; set => winnerCountryId = value; }

    public GameSession()
    {

    }

    public GameSession(int mapId)
    {
        this.MapId = mapId;

        LastPlayingTime = DateTime.Now.ToString();
        SessionName = "Save " + LastPlayingTime;
        CurrentMove = 0;

        Countries = new List<Country>();
        DistrictsInfos = new List<DistrictInfo>();
        Relationships = new List<Relationship>();

        GameRules = new GameRules();
    }

    public GameSession(int mapId, GameRules gameRules) : this(mapId)
    {
        this.GameRules = gameRules;
    }

    public void EndGame(string gameOverMessage, int winnerCountryId)
    {
        gameOver = true;
        this.gameOverMessage = gameOverMessage;
        this.winnerCountryId = winnerCountryId;
    }

    public DistrictInfo FindDistrictById(int id)
    {
        if (id < 0 || id > DistrictsInfos.Count)
        {
            Debug.LogError("Был запрошен Район с id " + id + ", но такого Района нет!");
            return null;
        }

        return DistrictsInfos[id];
    }

    /// <summary>
    /// Ищет отношения между странами с данными id.
    /// </summary>
    /// <param name="firstId">Страна 1(2)</param>
    /// <param name="secondId">Страна 2(1)</param>
    /// <returns>Отношение между этими Странами. Null, если отношений между этими странами нет.</returns>
    public Relationship FindRelationship(int firstId, int secondId)
    {
        for (int i = 0; i < Relationships.Count; i++)
        {
            if (Relationships[i].FirstCountryId == firstId && Relationships[i].SecondCountryId == secondId ||
                Relationships[i].FirstCountryId == secondId && Relationships[i].SecondCountryId == firstId)
            {
                return Relationships[i];
            }
        }

        return null;
    }
    
    /// <summary>
    /// Проверяет для каких из стран в списке не существует отношений и создаёт их.
    /// </summary>
    public void AddUnaddedRelationships()
    {
        for (int i = 0; i < Countries.Count; i++)
        {
            for (int j = 0; j < Countries.Count; j++)
            {
                // У страны с самой с собой нет отношений.
                if (i == j)
                    continue;
                // Если отношения уже есть, пропускаем.
                if (FindRelationship(Countries[i].CountryId, Countries[j].CountryId) != null)
                {
                    continue;
                }
                // Иначе - создаём.
                else
                {
                    Relationships.Add(new Relationship(Countries[i].CountryId, Countries[j].CountryId));
                }
            }
        }
    }

    public void UpdateLastPlayingTime()
    {
        LastPlayingTime = DateTime.Now.ToString();
    }
}
