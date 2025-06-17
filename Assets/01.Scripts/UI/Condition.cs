using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
	public int maxHeart = 5;
	public int currentHeart;

	bool isLive = true;

    private void Awake()
    {
		UIManager.Instance.condition = this;
        currentHeart = maxHeart;
    }

    void Start()
	{
		UIManager.Instance.GenerateHearts();
    }
	
	void Update()
	{
		
	}

	public void TakeDamage(int Amount = 1)
	{
		currentHeart = Mathf.Max(currentHeart - Amount, 0);
		UIManager.Instance.UpdateHeart();
		Die();
    }

	private void Die()
	{
        if (currentHeart <= 0)
        {
            isLive = false;
        }
    }
}