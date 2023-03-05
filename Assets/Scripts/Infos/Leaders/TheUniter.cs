using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Описывает поведение Лидера "Объединитель".
/// </summary>
public class TheUniter : Leader
{
    // Id Районов, которые этот Лидер считает "священными".
    private List<int> sacredDistricts;
    public List<int> SacredDistricts { get => sacredDistricts; set => sacredDistricts = value; }

    public TheUniter() : base()
    {

    }

    public TheUniter(LeaderMonoBehaviour leaderMonoBehaviour) : base(leaderMonoBehaviour)
    {
        // Назначим имя.
        leaderName = "Объединитель";

        // Назначим цвет Страны.
        Country.SetCountryColor(Color.cyan);
        Country.CountryName = "Единое Королевство";
        // Установим начальные значения для Страны этого Лидера.
        Country.Gold = 15;
        Country.Iron = 0;
        Country.Horses = 0;
        Country.Agents = 4;

        // Установим священные земли.
        SacredDistricts = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            bool mistake = false;
            District district;
            do
            {
                district = LeaderMonoBehaviour.GameManager.GetRandomDistrict(false);
                // Проверим, не выпал ли район, уже объявленный как священный ранее.
                for (int j = 0; j < SacredDistricts.Count; j++)
                {
                    if (district.id == SacredDistricts[j])
                        mistake = true;
                }
            } while (mistake);
            SacredDistricts.Add(district.id);
            Debug.Log("Район " + district + " был выбран Объединителем как священный.");
        }

        DefaultLine = "Приветствую! Моя империя - священна для меня. Не нарушайте целостность нашего государства и не трогайте земли, по праву принадлежащие нам, и мы с вами поладим.";
        WarLine = "Рано или поздно наш конфликт будет окончен.";
        PeaceAcceptedLine = "Наш конфликт отныне разрешён. Надеюсь, наши интересы не пересекутся вновь.";
        WarDeclarationFromThisLine = "Вы встали на пути единства моей империи. Вы поплатитесь за это!";
        WarDeclarationToThisLine = "Ваша империя мне не угрожает! Это война быстро закончится...";
    }

    public override void MakeMove()
    {
        Debug.Log("The Uniter делает ход. Золото: " + Country.Gold);
        // Проверяет, не захвачен ли священный Район другими Странами. Объявит войну таким Странам.
        for (int i = 0; i < SacredDistricts.Count; i++)
        {
            Country holder = LeaderMonoBehaviour.GameManager.gameSession.FindDistrictById(SacredDistricts[i]).holder;
            if (holder != null && holder != Country)
            {
                Relationship relationship = LeaderMonoBehaviour.GameManager.gameSession.FindRelationship(Country.CountryId, holder.CountryId);
                // Объявляет войну и показывает сообщение, только если ещё не показывал.
                if (!relationship.AtWar)
                {
                    relationship.AtWar = true;
                    if (holder.Leader.IsPlayer) LeaderMonoBehaviour.OpenDialogUI("Вы посягнули на целостность нашего государства и удерживаете наши священные земли. " +
                        "Мы вынуждены объявить вам войну. До того момента, как наши земли будут освобождены, мира не будет!", true);

                    Debug.Log($"Священный Район {LeaderMonoBehaviour.GameManager.gameSession.FindDistrictById(SacredDistricts[i]).DistrictName} " +
                        $"Лидера The Uniter был захвачен Лидером " +
                        $"{LeaderMonoBehaviour.GameManager.gameSession.FindDistrictById(SacredDistricts[i]).holder.Leader.LeaderName}. " +
                        $"The Uniter объявляет войну!");
                }
            }
        }

        //Debug.Log("Лидер " + LeaderName + " имеет кризис: " + CheckCrysis());

        base.MakeMove();

        if (!battleIsGoingToHappen) LeaderMonoBehaviour.GameManager.NextMove();
    }

    public override void RequestPeace(Country otherCountry)
    {
        // Проверим, освободила ли предлагающая страна священный район.
        bool isOkay = true;
        for (int i = 0; i < SacredDistricts.Count; i++)
        {
            Country holder = LeaderMonoBehaviour.GameManager.gameSession.FindDistrictById(SacredDistricts[i]).holder;
            if (holder != null && holder == otherCountry)
            {
                isOkay = false;
                break;
            }
        }

        if (isOkay)
        {
            LeaderMonoBehaviour.GameManager.gameSession.FindRelationship(otherCountry.CountryId, Country.CountryId).AtWar = false;
            if (otherCountry.Leader.IsPlayer) LeaderMonoBehaviour.OpenDialogUI(PeaceAcceptedLine, false);
            Debug.Log($"Лидер {otherCountry.Leader.LeaderName} предложил мир The Uniter, и он согласился.");
        }
        else
        {
            if (otherCountry.Leader.IsPlayer) LeaderMonoBehaviour.OpenDialogUI("Мы не будем даже и думать о заключении мира, пока вы удерживаете наши земли!", false);
            Debug.Log($"Лидер {otherCountry.Leader.LeaderName} предложил мир Лидеру {LeaderName}, но он отказался.");
        }
    }

    // Особая функция этого Лидера: Покажите, какие территории вы считаете своими.
    public override void SpecialDialogOption1()
    {
        Debug.Log("Была вызвана особая функция The Uniter.");

        for (int i = 0; i < SacredDistricts.Count; i++)
        {
            LeaderMonoBehaviour.GameManager.gameSession.FindDistrictById(SacredDistricts[i]).districtFunc.HighlightDistrict(7f);
        }

        // Теперь закроем диалог.
        LeaderMonoBehaviour.CloseDialogUI();
    }
}
