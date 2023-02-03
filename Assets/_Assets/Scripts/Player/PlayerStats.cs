using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Singleton<PlayerStats>
{
    [SerializeField] private int maxHp;
    private float currHp;

    // Start is called before the first frame update
    void Start()
    {
        Healthbar.Instance.UpdateMaxHP(maxHp);
        currHp = maxHp;
    }

    public void TakeDamage(int _dmg)
    {
        currHp = Mathf.Max(0, currHp - _dmg);

        Healthbar.Instance.UpdateHPBar((float)currHp / maxHp);

        if (currHp <= 0)
            Die();
    }

    private void Die()
    {

    }

    /// <summary>
    /// Attemps to heal the player the specified amount.
    /// </summary>
    /// <param name="_heal"> amount of health to heal </param>
    /// <returns> true if the healing was successful, false if the player is already at full hp.</returns>
    public bool Heal(int _heal)
    {
        if (currHp == maxHp)
            return false;

        currHp = Mathf.Min(maxHp, currHp + _heal);

        Healthbar.Instance.UpdateHPBar((float)currHp / maxHp);

        return true;
    }

    public void MaxHpUp()
    {
        maxHp++;
        currHp++;

        Healthbar.Instance.UpdateMaxHP(maxHp);
        Healthbar.Instance.UpdateHPBar((float)currHp / maxHp);
    }
}
