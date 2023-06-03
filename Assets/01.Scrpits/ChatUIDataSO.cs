using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Chat")]
public class ChatUIDataSO : ScriptableObject
{
    public List<string> userNames = new List<string>();
    public List<string> chattings = new List<string>();
    public List<Color> userColors = new List<Color>();
    public List<Sprite> icons = new List<Sprite>();

    public string GetUserName()
    {
        return userNames[Random.Range(0, userNames.Count)];
    }

    public Color GetUserColor()
    {
        return userColors[Random.Range(0, userColors.Count)];
    }

    public string GetChat()
    {
        return chattings[Random.Range(0, chattings.Count)];
    }

    public Sprite[] GetIcons(int count)
    {
        int[] randomIndexs = new int[icons.Count];
        Sprite[] result = new Sprite[count];

        for (int i = 0; i < icons.Count; i++)
            randomIndexs[i] = i;
        for (int i = 0; i < icons.Count; i++)
        {
            int indexOne = Random.Range(0, icons.Count);
            int indexTwo = Random.Range(0, icons.Count);
            int temp = randomIndexs[indexOne];
            randomIndexs[indexOne] = indexTwo;
            randomIndexs[indexTwo] = temp;
        }
        for (int i = 0; i < count; i++)
        {
            result[i] = icons[randomIndexs[i]];
        }
        return result;
    }
}
