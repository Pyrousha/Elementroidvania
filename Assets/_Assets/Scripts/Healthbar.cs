using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private RectTransform hpBar_BG;
    [SerializeField] private RectTransform hpBar_Sliding;

    [Range(0f, 1f)]
    [SerializeField] private float percent;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateHPBar(percent);
    }

    private void UpdateHPBar(float _percent)
    {
        float minHPOffset = hpBar_BG.sizeDelta.x - 75.0f;
        float maxHPOffset = 0f;
        float currOffset = Utils.RemapPercent(_percent, minHPOffset, maxHPOffset);

        hpBar_Sliding.offsetMax = new Vector2(-currOffset, hpBar_Sliding.offsetMax.y);
    }
}
