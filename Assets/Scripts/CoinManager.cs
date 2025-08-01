using UnityEngine;

public class CoinManager : MonoBehaviour
{
    private const string COINS_KEY = "PlayerCoins";
    private static CoinManager _instance;

    public static CoinManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CoinManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "CoinManager";
                    _instance = obj.AddComponent<CoinManager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }

    private int _coins;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCoins();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int Coins
    {
        get => _coins;
        set
        {
            _coins = value;
            SaveCoins();
        }
    }

    private void LoadCoins()
    {
        _coins = PlayerPrefs.GetInt(COINS_KEY, 0);
        Debug.Log($"Loaded coins: {_coins}");
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt(COINS_KEY, _coins);
        PlayerPrefs.Save();
        Debug.Log($"Saved coins: {_coins}");
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
    }

    public bool SpendCoins(int amount)
    {
        if (Coins >= amount)
        {
            Coins -= amount;
            return true;
        }
        return false;
    }
}