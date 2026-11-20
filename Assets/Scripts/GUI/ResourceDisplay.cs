using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ResourceDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Resource Settings")]
    public GameManager.Resource resourceType;

    [Header("UI References")]
    public TextMeshProUGUI valueText;
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    
    

    // event functions
    void Start()
    {
        if (tooltipPanel != null) { tooltipPanel.SetActive(false); }
    }

    void Update()
    {
        if (GameManager.Instance != null)
        {
            float amount = GameManager.Instance.GetResourceValue(resourceType);
            valueText.text = FormatNumber(amount);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipPanel != null && GameManager.Instance != null)
        {
            tooltipText.text = GameManager.Instance.GetResourceDetails(resourceType);
            tooltipPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    private string FormatNumber(float num)
    {
        if (num >= 1000000) return (num / 1000000f).ToString("0.#") + "M";
        return num.ToString("#,0");
    }
}