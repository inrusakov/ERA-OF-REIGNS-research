using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс, представляющий собой юнита в армии.
/// </summary>
public class Unit
{
    /// <summary>
    /// Имя юнита.
    /// </summary>
    public string UnitName { get => unitName; set => unitName = value; }
    /// <summary>
    /// Тип юнита.
    /// 0 - воин; 1 - усиленный воин; 2 - всадник; 3 - тайная полиция.
    /// </summary>
    public int Type { get => type; set => type = value; }
    /// <summary>
    /// Является ли юнит временным?
    /// </summary>
    public bool IsTemporary { get => isTemporary; set => isTemporary = value; }
    /// <summary>
    /// Урон, который наносит этот юнит.
    /// </summary>
    public int Damage { get => damage; set => damage = value; }
    /// <summary>
    /// Здоровье, которое есть у этого юнита.
    /// </summary>
    public int Health { get => health; set => health = value; }
    /// <summary>
    /// Максимальный урон, который этот юнит может нанесить.
    /// </summary>
    public int MaxDamage { get => maxDamage; set => maxDamage = value; }
    /// <summary>
    /// Максимум здоровья, которые может быть у этого юнита.
    /// </summary>
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }

    private string unitName;
    private int type;
    private bool isTemporary;

    private int damage;
    private int maxDamage;

    private int health;
    private int maxHealth;

    public Unit() { }

    public Unit(string unitName, int type, bool isTemporary, int maxDamage, int maxHealth)
    {
        this.unitName = unitName;
        this.type = type;
        this.isTemporary = isTemporary;
        if (isTemporary) this.unitName += " (Врем.)";

        this.MaxDamage = maxDamage;
        damage = maxDamage;
        this.MaxHealth = maxHealth;
        health = maxHealth;
    }

    /// <summary>
    /// Восстанавливает все характеристики юниту.
    /// </summary>
    public void Heal()
    {
        damage = MaxDamage;
        health = MaxHealth;
    }

    /// <summary>
    /// Принять урон.
    /// </summary>
    /// <param name="damageToDealWith">Урон, который наноситься этому юниту (значение должно быть со знаком плюс).</param>
    /// <returns>true, если юнит погибает (здоровье стало меньше или равно 0), иначе false.</returns>
    public bool TakeDamage(int damageToDealWith)
    {
        health -= damageToDealWith;

        return health <= 0;
    }
}
