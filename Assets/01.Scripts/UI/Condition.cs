using UnityEngine;

public class Condition : MonoBehaviour
{
	public int maxHealth = 5;
	public int currentHealth;
    void Start()
	{
		currentHealth = maxHealth;
	}
	
	void Update()
	{
		
	}

	public void TakeDamage(int Amount)
	{
		currentHealth = Mathf.Max(currentHealth - Amount, maxHealth);
		if (currentHealth <= 0)
		{
			Die();
        }
    }

	private void Die()
	{
		
	}
}