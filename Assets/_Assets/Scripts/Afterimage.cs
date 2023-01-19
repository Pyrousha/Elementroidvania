using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Afterimage : MonoBehaviour
{
    [SerializeField] private float duration;
    private float timeLeft;
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    [SerializeField] private SpriteRenderer sprRend;

    void Awake()
    {
        timeLeft = duration;
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0)
            Destroy(gameObject);

        sprRend.color = Color.Lerp(startColor, endColor, 1 - timeLeft / duration);
    }

    public static void SpawnAfterimage(GameObject _afterimagePrefab, Vector3 _playerPosition, SpriteRenderer _playerSprite)
    {
        Afterimage newAfterimage = Instantiate(_afterimagePrefab).GetComponent<Afterimage>();
        newAfterimage.transform.position = _playerPosition;
        newAfterimage.sprRend.sprite = _playerSprite.sprite;
        newAfterimage.sprRend.flipX = _playerSprite.flipX;
    }
}
