using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
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

    private void Update()
    {
        UpdateHeart();
    }

    

    public void GenerateHearts()
    {
        bool hasPlayerObject = FindAnyObjectByType<PlayerHealth>() != null;
        int maxHearts = GameManager.Instance.Player.GetMaxHP();
        if (_hearts != null)
        {
            foreach (Image heart in _hearts)
            {
                Destroy(heart.gameObject);
            }
            _hearts.Clear();
        }

        if (hasPlayerObject)
        {
            Debug.Log("플레이어 오브젝트가 존재합니다");

            for (int i = 0; i < maxHearts; i++)
            {
                GameObject heartObj = Instantiate(heartPrefab, heartContainer);
                Image heartImage = heartObj.GetComponent<Image>();
                _hearts.Add(heartImage);
            }

            //foreach (Image heart in _hearts)
            //{
            //    heart.sprite = fullHeart;
            //}
        }
        else
        {
            Debug.LogError("플레이어 오브젝트가 존재하지 않습니다");
            return;
        }
    }

    public void UpdateHeart()
    {
        int currentHearts = GameManager.Instance.Player.GetCurrentHP();
        for (int i = 0; i < _hearts.Count; i++)
        {
            if (i < currentHearts)
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