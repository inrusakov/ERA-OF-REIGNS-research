using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  ласс, представл€ющий собой отношени€ между двум€ странами.
/// </summary>
public class Relationship
{
    int firstCountryId;
    int secondCountryId;
    bool atWar;
    int numberOfMoveToUnlockPeace;
    int numberOfMoveToUnlockWar;

    public Relationship()
    {

    }

    public Relationship(int firstCountryId, int secondCountryId)
    {
        this.firstCountryId = firstCountryId;
        this.secondCountryId = secondCountryId;
        atWar = false;
        numberOfMoveToUnlockPeace = -1;
        numberOfMoveToUnlockWar = -1;
    }

    /// <summary>
    /// Id первой страны из отношений.
    /// </summary>
    public int FirstCountryId { get => firstCountryId; set => firstCountryId = value; }
    /// <summary>
    /// Id второй страны из отношений.
    /// </summary>
    public int SecondCountryId { get => secondCountryId; set => secondCountryId = value; }
    /// <summary>
    /// Ќаход€тс€ ли эти страны в состо€нии войны?
    /// </summary>
    public bool AtWar { get => atWar; set => atWar = value; }
    /// <summary>
    /// Ќа каком ходу можно будет предложить заключить мир.
    /// </summary>
    public int NumberOfMoveToUnlockPeace { get => numberOfMoveToUnlockPeace; set => numberOfMoveToUnlockPeace = value; }
    /// <summary>
    /// Ќа каком ходу можно будет объ€вить войну.
    /// </summary>
    public int NumberOfMoveToUnlockWar { get => numberOfMoveToUnlockWar; set => numberOfMoveToUnlockWar = value; }
}
