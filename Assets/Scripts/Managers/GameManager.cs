using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private float _gold = 0, _goldGeneration = 123.4f;
    private int _minesNumber = 0;
    
    private float _food = 0, _foodGeneration = 12.3f;
    private int _farmsNumber = 0;

    public enum Resource { Gold, Food };
    
    
    // event functions
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        _gold += _goldGeneration;
        _food += _foodGeneration;
    }
    
    
    
    // regular functions
    public float GetResourceValue(Resource type)
    {
        switch (type)
        {
            case Resource.Gold: return _gold;
            case Resource.Food: return _food;
            default: return -1;
        }
    }
    
    public string GetResourceDetails(Resource type)
    {
        switch (type)
        {
            case Resource.Gold: return $"Sources: {_minesNumber} Gold Mines\nGeneration: +{_goldGeneration} / round";
            case Resource.Food: return $"Sources: {_farmsNumber} Farms\nGeneration: +{_foodGeneration} / round";
            default: return "";
        }
    }
}
