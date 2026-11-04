using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Image healthBar;
    private Destroyable destroyable;

    void Awake()
    {
        destroyable = GetComponent<Destroyable>();
    }

    void Start()
    {
        Invoke(nameof(selfDamage), 5f);
    }

    void OnEnable()
    {
        destroyable.HealthChanged += UpdateHealthbar;
    }

    void OnDisable()
    {
        destroyable.HealthChanged -= UpdateHealthbar;
    }

    void Update()
    {
        transform.GetChild(0).transform.LookAt(Camera.main.transform);
    }

    void UpdateHealthbar(float health, float maxHealth)
    {
        float percentage = health / maxHealth;
        healthBar.fillAmount = percentage;
    }

    void selfDamage()
    {
        destroyable.TakeDamage(10f);
        Invoke(nameof(selfDamage), 5f);
    }

}
