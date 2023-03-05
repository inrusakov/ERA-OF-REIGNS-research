using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Представляет страну-информацию (страны-функции нет).
/// </summary>
public class Country
{
    // Название страны.
    private string countryName = "";
    // Цвет Страны.
    private List<float> colorAsRGBList;
    // Лидер-информация, который управляет данной страной.
    private Leader leader;
    // Id районов, которые принадлежат этой стране.
    private List<int> districtsIds = new List<int>();
    // Id этой страны.
    private int countryId;
    // Кол-во Районов, которыми владела эта страна за всё время.
    private int counterOfDistrictsEverHelded = 0;
    // Армия этой Страны.
    private Army army;

    // Очки Победы.
    private int victoryPoints = 0;
    // Золото.
    private int gold = 0;
    // Железо.
    private int iron = 0;
    // Животноводство.
    private int horses = 0;
    // Тайная полиция.
    private int agents = 0;

    /// <summary>
    /// Вызывает метод Maintenance() для всех Районов этой Страны.
    /// </summary>
    public void MainteneAllDistricts()
    {
        for (int i = 0; i < DistrictsIds.Count; i++)
        {
            LeaderMonoBehaviour.GameManager.gameSession.FindDistrictById(DistrictsIds[i]).districtFunc.Maintenance();
        }
    }

    /// <summary>
    /// Добавить Район в Страну.
    /// </summary>
    /// <param name="districtToAdd">Район, который нужно добавить.</param>
    /// <param name="gameplayChange">Произошло ли добавление Района во время геймплея? (Нужно ли обрабатывать бонусы за захват?)</param>
    public void AddNewDistrict(District districtToAdd, bool gameplayChange)
    {
        DistrictsIds.Add(districtToAdd.id);
        districtToAdd.SetHolder(this, gameplayChange);
    }

    /// <summary>
    /// Удалить Район из Страны.
    /// </summary>
    /// <param name="districtToDelete">Район, который нужно удалить.</param>
    /// <param name="gameplayChange">Произошло ли добавление Района во время геймплея? (Нужно ли обрабатывать штрафы за потерю?)</param>
    /// <param name="newHolder">Новый владелец Района (допускается null).</param>
    public void RemoveDistrict(District districtToDelete, bool gameplayChange, Country newHolder)
    {
        DistrictsIds.Remove(districtToDelete.id);
        if (newHolder != null)
        {
            newHolder.DistrictsIds.Add(districtToDelete.id);
        }
        districtToDelete.SetHolder(newHolder, gameplayChange);
    }

    /// <summary>
    /// Получить цвет, сохранённый как список (rgb).
    /// </summary>
    public Color GetCountryColor()
    {
        return new Color(ColorAsRGBList[0], ColorAsRGBList[1], ColorAsRGBList[2]);
    }

    /// <summary>
    /// Сохранить цвет (rgb) в список.
    /// </summary>
    public void SetCountryColor(Color color)
    {
        ColorAsRGBList = new List<float>();
        ColorAsRGBList.Add(color.r);
        ColorAsRGBList.Add(color.g);
        ColorAsRGBList.Add(color.b);
    }


    /// <summary>
    /// Очки Победы.
    /// </summary>
    public int VictoryPoints { get => victoryPoints; set => victoryPoints = value; }
    /// <summary>
    /// Золото.
    /// </summary>
    public int Gold { get => gold; set { gold = value; if (gold < 0) gold = 0; } }
    /// <summary>
    /// Железо.
    /// </summary>
    public int Iron { get => iron; set => iron = value; }
    /// <summary>
    /// Животноводство.
    /// </summary>
    public int Horses { get => horses; set => horses = value; }
    /// <summary>
    /// Тайная полиция.
    /// </summary>
    public int Agents { get => agents; set => agents = value; }

    /// <summary>
    /// Кол-во Районов, которыми владела эта страна за всё время.
    /// </summary>
    public int CounterOfDistrictsEverHelded { get => counterOfDistrictsEverHelded; set => counterOfDistrictsEverHelded = value; }
    /// <summary>
    /// Название страны.
    /// </summary>
    public string CountryName { get => countryName; set => countryName = value; }
    /// <summary>
    /// Армия Страны.
    /// </summary>
    public Army Army { get => army; set => army = value; }
    /// <summary>
    /// Id этой страны.
    /// </summary>
    public int CountryId { get => countryId; set => countryId = value; }
    /// <summary>
    /// Id районов, которые принадлежат этой стране.
    /// </summary>
    public List<int> DistrictsIds { get => districtsIds; set => districtsIds = value; }
    /// <summary>
    /// Лидер-информация, который управляет данной страной.
    /// </summary>
    public Leader Leader { get => leader; set => leader = value; }
    /// <summary>
    /// Цвет Страны.
    /// </summary>
    public List<float> ColorAsRGBList { get => colorAsRGBList; set => colorAsRGBList = value; }
}
