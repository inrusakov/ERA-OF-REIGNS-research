using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Лидер "Игрок".
/// </summary>
public class ThePlayer : Leader
{
    public ThePlayer() : base()
    {

    }

    public ThePlayer(LeaderMonoBehaviour leaderMonoBehaviour) : base(leaderMonoBehaviour)
    {
        // Назначим имя.
        //leaderName = System.Environment.UserName;
        leaderName = MainMenuManager.currentProfile.profileName;

        // Назначим цвет Страны.
        Country.SetCountryColor(Color.green);
        Country.CountryName = "Империя";
        // Установим начальные значения для Страны этого Лидера.
        Country.Gold = 15;
        Country.Iron = 0;
        Country.Horses = 0;
        Country.Agents = 0;

        IsPlayer = true;
    }

    public override void RequestPeace(Country otherCountry)
    {
        // ИДЕЯ: Предложение о мире будет занесено в центр сообщений игрока, где будет кнопка принять.
    }

    public override void MakeMove()
    {
        Debug.Log($"Игрок \"{leaderName}\" делает ход. Золото: " + Country.Gold);
        Debug.Log("Лидер " + LeaderName + " имеет кризис: " + CheckCrysis());
    }

    public override void BattleEnded()
    {
        
    }
}
