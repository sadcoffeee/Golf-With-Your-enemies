using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keepTrackOfPlayer : MonoBehaviour
{
    public static keepTrackOfPlayer Instance;

    public int[] scores;
    public bool unlockedCatapult;
    public bool enteredALevel;
    public Vector3 currentRespawnPoint;
    public GameObject[] scoreObjects;
    public GameObject tipPanel;
    public GameObject scorePanel;

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
        unlockedCatapult = false;
        enteredALevel = false;
        currentRespawnPoint = new Vector3(-17, 0.3f, -9);
        scores = new int[4];

        StartCoroutine(playTutorialTips());
    }

    IEnumerator playTutorialTips()
    {
        tutorialHandler.Instance.ShowTooltip(0);
        yield return new WaitForSeconds(5);
        tutorialHandler.Instance.ShowTooltip(1);
        yield return new WaitForSeconds(5);
        tutorialHandler.Instance.ShowTooltip(2);
    }

    void fillInScores()
    {
        // Ensure scoreObjects array matches the number of scores
        for (int i = 0; i < scores.Length && i < scoreObjects.Length; i++)
        {
            // Find a Text component on each score object and set it to the corresponding score
            var scoreText = scoreObjects[i].GetComponent<TMPro.TextMeshProUGUI>();
            scoreText.text = scores[i].ToString();

        }
    }

    public void toggleScoreOverlay()
    {
        // Check if we need to update tipPanel based on shown tooltips
        if (tutorialHandler.Instance.shownTooltips.Count > tipPanel.transform.childCount)
        {
            // Clear existing tip panel entries and populate updated list
            foreach (Transform child in tipPanel.transform)
            {
                Destroy(child.gameObject);
            }
            tutorialHandler.Instance.populateTipsList();
        }

        // Run fillInScores to update score panel with the latest scores
        fillInScores();

        // Toggle the activation state of the score and tip panels
        bool isActive = scorePanel.activeSelf;
        scorePanel.SetActive(!isActive);
        tipPanel.SetActive(!isActive);
    }

}
