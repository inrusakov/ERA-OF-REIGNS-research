using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс для удаления игрового объекта.
/// </summary>
public class DestroyAfter : MonoBehaviour
{
    /// <summary>
    /// Настраивает время, после которого объект самоуничтожится.
    /// </summary>
    /// <param name="destroyTime">Время в секундах, после которого нужно уничтожится. 
    /// Если оно <= 0, то автоматического уничтожения не будет.</param>
    public void SetDestructionTime(float destroyTime)
    {
        if (destroyTime > 0)
        {
            Destroy(gameObject, destroyTime);
        }
    }
}
