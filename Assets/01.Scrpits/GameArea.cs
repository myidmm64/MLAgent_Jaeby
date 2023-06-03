using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameArea : MonoBehaviour
{
    [SerializeField]
    private PlayerAgent _player = null;
    public PlayerAgent Player => _player;

    private int score = 0;
    public int Score => score;

    private void Start()
    {
        StartCoroutine(ScoreCoroutine());
    }

    private IEnumerator ScoreCoroutine()
    {
        while(true)
        {
            score += 13;
            UIManager.Instance.UpdateScore(this);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
