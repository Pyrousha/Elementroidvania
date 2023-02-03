using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private Pickup_Enum pickupType;

    [System.Serializable]
    private enum Pickup_Enum
    {
        Healing,
        MaxHp
    }

    void OnTriggerEnter2D(Collider2D _col)
    {
        switch (pickupType)
        {
            case Pickup_Enum.Healing:
                {
                    if (PlayerStats.Instance.Heal(1))
                        Destroy(gameObject);
                    break;
                }
            case Pickup_Enum.MaxHp:
                {
                    PlayerStats.Instance.MaxHpUp();

                    Destroy(gameObject);
                    break;
                }
        }
    }
}
