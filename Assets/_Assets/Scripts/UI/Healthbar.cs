using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : Singleton<Healthbar>
{
    //150 units per health segment, 75 units starting offset

    [SerializeField] private RectTransform hpBar_BG;
    [SerializeField] private RectTransform hpBar_Sliding;

    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void UpdateHPBar(float _percent)
    {
        float minHPOffset = hpBar_BG.sizeDelta.x - 75.0f;
        float maxHPOffset = 0f;
        float currOffset = Utils.RemapPercent(_percent, minHPOffset, maxHPOffset);

        hpBar_Sliding.offsetMax = new Vector2(-currOffset, hpBar_Sliding.offsetMax.y);
    }

    public void UpdateMaxHP(int _maxHp)
    {
        rectTransform.sizeDelta = new Vector2(75 + 150 * _maxHp, rectTransform.sizeDelta.y);
    }
}
