using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("PlayerManager");
                    DontDestroyOnLoad(obj);
                    _instance = obj.AddComponent<PlayerManager>();
                }
            }
            return _instance;
        }
    }

    public PlayerHealth playerHealth;

    private void Awake()
    {
        if (_instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    
}