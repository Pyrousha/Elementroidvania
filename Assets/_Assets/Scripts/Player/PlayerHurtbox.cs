using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHurtbox : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D _col)
    {
        PlayerStats.Instance.TakeDamage(_col.GetComponent<Hitbox>().Damage);
    }
}
