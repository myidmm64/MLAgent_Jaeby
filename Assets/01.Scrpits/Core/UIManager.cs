using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleTon<UIManager>
{
    [SerializeField]
    private GameArea _mainGameArea;

    [SerializeField]
    private TextMeshProUGUI _scoreText = null;

    [SerializeField]
    private Transform _hpParentTrm = null;
    [SerializeField]
    private GameObject _hpImagePrefab = null;
    [SerializeField]
    private Transform _bombParentTrm = null;
    [SerializeField]
    private GameObject _bombImagePrefab = null;

    [SerializeField]
    private Image _bossHpBar = null;

    [SerializeField]
    private float _chatMinTime = 0.2f;
    [SerializeField]
    private float _chatMaxTime = 2f;

    [SerializeField]
    private ChatUI _chatUIPrefab = null;
    [SerializeField]
    private Transform _chatUIParentTrm = null;
    [SerializeField]
    private ChatUIDataSO _chatUIDataSO = null;

    private int q = 0;

    private void Awake()
    {
        if(_mainGameArea != null)
        {
            _mainGameArea.Player.OnEpisodeBeginAction += ChatDestroy;
            _mainGameArea.Player.OnEpisodeBeginAction += () => StartCoroutine(ChatCoroutine());
            _mainGameArea.Player.OnEpisodeBeginAction += ()=> { UpdatePlayerHP(_mainGameArea); UpdatePlayerBomb(_mainGameArea); };
        }
    }

    public void UpdateBossHP(GameArea area, int max, int cur)
    {
        if (_mainGameArea != area)
            return;

        _bossHpBar.fillAmount = (float)cur / max;
    }

    public void UpdatePlayerHP(GameArea area)
    {
        if (_mainGameArea != area)
            return;

        List<GameObject> childs = new List<GameObject>();
        for(int i = 0; i < _hpParentTrm.childCount; i++)
            childs.Add(_hpParentTrm.GetChild(i).gameObject);
        for(int i = 0; i < childs.Count; i++)
            Destroy(childs[i]);

        for (int i = 0; i < _mainGameArea.Player.HP; i++)
        {
            Instantiate(_hpImagePrefab, _hpParentTrm);
        }
    }

    public void UpdatePlayerBomb(GameArea area)
    {
        if (_mainGameArea != area)
            return;

        List<GameObject> childs = new List<GameObject>();
        for (int i = 0; i < _bombParentTrm.childCount; i++)
            childs.Add(_bombParentTrm.GetChild(i).gameObject);
        for (int i = 0; i < childs.Count; i++)
            Destroy(childs[i]);

        for (int i = 0; i < _mainGameArea.Player.CurBomb; i++)
        {
            Instantiate(_bombImagePrefab, _bombParentTrm);
        }
    }

    public void UpdateScore(GameArea area)
    {
        if (_mainGameArea != area)
            return;

        _scoreText.SetText(_mainGameArea.Score.ToString("00000000"));
    }

    public void ChatDestroy()
    {
        StopAllCoroutines();
        List<GameObject> childs = new List<GameObject>();
        for (int i = 0; i < _chatUIParentTrm.childCount; i++)
            childs.Add(_chatUIParentTrm.GetChild(i).gameObject);
        for (int i = 0; i < childs.Count; i++)
            Destroy(childs[i]);
    }

    private IEnumerator ChatCoroutine()
    {
        while(true)
        {
            if(_chatUIParentTrm.childCount == 7)
            {
                Destroy(_chatUIParentTrm.GetChild(_chatUIParentTrm.childCount - 1).gameObject);
            }

            ChatUI ui = Instantiate(_chatUIPrefab, _chatUIParentTrm);
            ui.ChatInit(_chatUIDataSO.GetUserName(), _chatUIDataSO.GetChat(), _chatUIDataSO.GetUserColor(), 
                _chatUIDataSO.GetIcons(UnityEngine.Random.Range(0, 4)));
            ui.name += q;
            q++;
            List<Transform> childs = new List<Transform>();
            for(int i =  0; i < _chatUIParentTrm.childCount - 1; i++)
                childs.Add(_chatUIParentTrm.GetChild(i));
            for(int i = 0; i < childs.Count;i++)
            {
                childs[i].SetParent(null);
                childs[i].SetParent(_chatUIParentTrm);
            }

            yield return new WaitForSeconds(UnityEngine.Random.Range(_chatMinTime, _chatMaxTime));
        }
    }

    private void Init()
    {
    }
}
