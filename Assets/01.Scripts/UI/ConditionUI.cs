using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConditionUI : MonoBehaviour
{
    private List<Image> _hearts = new List<Image>();
    public GameObject heartPrefab;
    public Transform heartContainer;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private void Awake()
    {
		UIManager.Instance.condition = this;
    }

    public void GenerateHearts()
    {
        int maxHearts = PlayerManager.Instance.playerHealth.GetMaxHP();
        if (_hearts != null)
        {
            foreach (Image heart in _hearts)
            {
                Destroy(heart.gameObject);
            }
        }

        for (int i = 0; i < maxHearts; i++)
        {
            GameObject heartObj = Instantiate(heartPrefab, heartContainer);
            Image heartImage = heartObj.GetComponent<Image>();
            _hearts.Add(heartImage);
        }
        UpdateHeart();
    }
    public void UpdateHeart()
    {
        int maxHearts = PlayerManager.Instance.playerHealth.GetCurrentHP();
        for (int i = 0; i < maxHearts; i++)
        {
            if (i < PlayerManager.Instance.playerHealth.GetCurrentHP())
            {
                _hearts[i].sprite = fullHeart;
            }
            else
            {
                _hearts[i].sprite = emptyHeart;
            }
        }
    }
}