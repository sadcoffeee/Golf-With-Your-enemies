
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class tutorialHandler : MonoBehaviour
{
    public static tutorialHandler Instance;

    public TextMeshProUGUI tooltipText;
    public Animator tooltipAnimator;
    public List<string> tooltips = new List<string>();
    public GameObject tipCardPrefab;
    public GameObject tipPanel;

    public HashSet<int> shownTooltips = new HashSet<int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowTooltip(int index)
    {
        if (index < 0 || index >= tooltips.Count)
        {
            Debug.LogWarning("Tooltip index out of range.");
            return;
        }

        if (shownTooltips.Contains(index)) return;

        // Set tooltip text and play animation
        tooltipText.text = tooltips[index];
        tooltipAnimator.SetTrigger("ShowTooltip");

        // Mark tooltip as shown
        shownTooltips.Add(index);
    }
    public void populateTipsList()
    {
        int index = 0;
        foreach (var tooltipIndex in shownTooltips)
        {
            GameObject tipCard = Instantiate(tipCardPrefab, tipPanel.transform);

            RectTransform cardRect = tipCard.GetComponent<RectTransform>();
            cardRect.anchoredPosition = new Vector2(0, 220 - index * 120);

            TextMeshProUGUI tipCardText = tipCard.GetComponentInChildren<TextMeshProUGUI>();
            tipCardText.text = "- " + tooltips[tooltipIndex];

            index++;
        }

    }
}

