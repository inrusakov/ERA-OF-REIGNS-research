using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Представляет собой Район-функционал.
/// </summary>
public class District : MonoBehaviour
{
    // Ссылка на район-информацию.
    public DistrictInfo districtInfo;

    // Соседи данного Района.
    public List<District> nearbyDistricts;

    // Ссылка на главный скрипт. С его помощью можно получить камеру и всё остальное.
    public static GameManager gameManager;

    public int id;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Взять плату за обслуживание этого Района у владельца, а также начислить доход.
    /// </summary>
    public void Maintenance()
    {
        districtInfo.holder.Gold += GetGoldIncome();
    }

    /// <summary>
    /// Считает, доход (убыток) от этого Района и возвращает это значение.
    /// </summary>
    /// <returns></returns>
    public int GetGoldIncome()
    {
        int sum = 0;

        sum += gameManager.gameSession.GameRules.DistrictMaintenance;

        // Прибавим добычу золота от Бонуса Района "Золото".
        if (districtInfo.DistrictBonus == 1)
        {
            if (districtInfo.HasBonusProduction) sum += gameManager.gameSession.GameRules.GoldBonus;
        }
        // Прибавим добычу золота от улучшения "Добыча Золота".
        if (districtInfo.HasGoldProduction)
        {
            sum += gameManager.gameSession.GameRules.GoldProductionBonus;
        }

        return sum;
    }

    /// <summary>
    /// Проверяет этот Район на соседство со страной country.
    /// </summary>
    /// <param name="country">страна, на соседство с которой нужно проверить этот Район.</param>
    /// <returns>true, если соседи, false иначе.</returns>
    public bool CheckForBeingNearCountry(Country country)
    {
        for (int i = 0; i < nearbyDistricts.Count; i++)
        {
            if (nearbyDistricts[i].districtInfo.holder == country)
            {
                return true;
            }
        }

        return false;
    }

    void LostDistrict(Country newHolder)
    {
        // У старого Лидера в любом случае отнимаем ресы.
        if (districtInfo.holder != null && districtInfo.HasBonusProduction)
        {
            switch (districtInfo.DistrictBonus)
            {
                case 2:
                    districtInfo.holder.Iron -= gameManager.gameSession.GameRules.IronBonus;
                    break;
                case 3:
                    districtInfo.holder.Horses -= gameManager.gameSession.GameRules.HorsesBonus;
                    break;
            }
            gameManager.UpdateStats();
            //gameManager.districtUI.UpdateIfNeeded();
        }

        // Если покинули Район добросовестно, оштрафуем лидера, если нужно.
        if (newHolder == null)
        {
            districtInfo.holder.VictoryPoints += gameManager.gameSession.GameRules.DistrictAbandoned;
            // Если Бонус Района был "Очки Победы".
            if (districtInfo.DistrictBonus == 4)
            {
                districtInfo.holder.VictoryPoints += gameManager.gameSession.GameRules.VictoryPointsBonusLost;
            }
            // Если был построен монумент.
            if (districtInfo.HasMonument)
            {
                districtInfo.holder.VictoryPoints -= gameManager.gameSession.GameRules.MonumentBonus;
                districtInfo.HasMonument = false;
            }
        }
        // Иначе, смотрим, что произошло.
        else
        {
            // Новому лидеру в любом случае дадим ресы.
            if (districtInfo.HasBonusProduction)
            {
                switch (districtInfo.DistrictBonus)
                {
                    case 2:
                        newHolder.Iron += gameManager.gameSession.GameRules.IronBonus;
                        break;
                    case 3:
                        newHolder.Horses += gameManager.gameSession.GameRules.HorsesBonus;
                        break;
                }
                gameManager.UpdateStats();
                //gameManager.districtUI.UpdateIfNeeded();
            }

            // Если этот район был нейтральным, то дадим захватившему лидеру его награду здесь.
            if (districtInfo.holder == null)
            {
                newHolder.VictoryPoints += gameManager.gameSession.GameRules.DistrictAchieved;
                // Если Бонус Района был "Очки Победы".
                if (districtInfo.DistrictBonus == 4)
                {
                    newHolder.VictoryPoints += gameManager.gameSession.GameRules.VictoryPointsBonus;
                }
            }
            // Иначе, оштрафуем старого Лидера и дадим новому Лидеру очки.
            else
            {
                districtInfo.holder.VictoryPoints += gameManager.gameSession.GameRules.DistrictLostInCombat;
                newHolder.VictoryPoints += gameManager.gameSession.GameRules.DistrictAchievedInCombat;

                // Если Бонус Района был "Очки Победы".
                if (districtInfo.DistrictBonus == 4)
                {
                    districtInfo.holder.VictoryPoints += gameManager.gameSession.GameRules.VictoryPointsBonusLost;
                    newHolder.VictoryPoints += gameManager.gameSession.GameRules.VictoryPointsBonus;
                }
                // Если был построен монумент.
                if (districtInfo.HasMonument)
                {
                    districtInfo.holder.VictoryPoints -= gameManager.gameSession.GameRules.MonumentBonus;
                    districtInfo.HasMonument = false;
                }
            }
        }
    }

    /// <summary>
    /// Установить нового владельца для этого Района.
    /// </summary>
    /// <param name="newHolder">Новый владелец Района. null, если владельца больше не должно быть.</param>
    public void SetHolder(Country newHolder, bool gameplayChange)
    {
        // Если это произошло во время геймплея, а не загрузки, обработаем штрафы и награды.
        if (gameplayChange)
        {
            LostDistrict(newHolder);
        }

        // Установка ссылки на нового владельца.
        districtInfo.holder = newHolder;
        // districtInfo.UpdateHolder(newHolder);

        // Изменение цвета Района под нового владельца.
        if (newHolder == null)
        {
            // Белый цвет по-умолчанию.
            gameObject.GetComponent<Renderer>().material.color = Color.white;
        }
        else
        {
            // Цвет страны по-умолчанию.
            gameObject.GetComponent<Renderer>().material.color = newHolder.GetCountryColor();

            // В любом случае, проверим, не достиг ли новый лидер нового порога захваченных районов.
            if (newHolder.DistrictsIds.Count > newHolder.CounterOfDistrictsEverHelded)
            {
                newHolder.CounterOfDistrictsEverHelded = newHolder.DistrictsIds.Count;
            }
        }
    }

    /// <summary>
    /// Восстановим владельца для этого Района (при загрузке игры).
    /// </summary>
    /// <param name="districtInfo"></param>
    public void LoadHolder(DistrictInfo districtInfo)
    {
        this.districtInfo = districtInfo;

        // Изменение цвета Района под нового владельца.
        if (districtInfo.holder == null)
        {
            // Белый цвет по-умолчанию.
            gameObject.GetComponent<Renderer>().material.color = Color.white;
        }
        else
        {
            // Цвет страны по-умолчанию.
            gameObject.GetComponent<Renderer>().material.color = districtInfo.holder.GetCountryColor();
        }
    }

    /// <summary>
    /// Подсветить данный Район на time секунд.
    /// </summary>
    /// <param name="time">Время, на которое этот район нужно подсвечивать.</param>
    public void HighlightDistrict(float time)
    {
        GameObject highlightObject = Instantiate(gameManager.AttentionSign);
        highlightObject.GetComponent<DestroyAfter>().SetDestructionTime(time);
        highlightObject.transform.position = gameObject.transform.position + new Vector3(0f, 0.1f, 0f);
    }

    private void OnMouseDown()
    {
        if (gameManager.developerState)
        {
            // Запретим выбор Района, если было нажатие на кнопку над Районом.
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Нажали в режиме разработчика");
                gameManager.nearbyDistrictsCreator.DistrictAction(this);
            }
        }
        else
        {
            // Запретим выбор Района, если было нажатие на кнопку над Районом.
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (districtInfo.holder != null)
                    Debug.Log("Был нажат Район " + name + "; Бонус Района: " + districtInfo.DistrictBonus + "; Владелец: " + districtInfo.holder.Leader);
                else
                    Debug.Log("Был нажат Район " + name + "; Бонус Района: " + districtInfo.DistrictBonus);

                // Сфокусируем камеру на этом Районе.
                gameManager.CameraController.FocusCamera(gameObject);
            }
        }
    }
}
