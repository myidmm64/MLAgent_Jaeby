using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _nameText = null;
    [SerializeField]
    private TextMeshProUGUI _chatText = null;
    [SerializeField]
    private Transform _iconImageParentTrm = null;
    [SerializeField]
    private GameObject _iconImagePrefab = null;

    private List<GameObject> _iconList = new List<GameObject>();

    public void ChatInit(string userName, string chat, Color userColor, Sprite[] icons)
    {
        _nameText.SetText(userName);
        _nameText.color = userColor;
        _chatText.SetText(chat);
        for(int i = 0; i< icons.Length; i++) 
        {
            Image image = Instantiate(_iconImagePrefab, _iconImageParentTrm).GetComponent<Image>();
            image.sprite = icons[i];
        }
    }
}