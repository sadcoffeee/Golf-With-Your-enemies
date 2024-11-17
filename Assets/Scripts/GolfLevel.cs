using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GolfLevel : MonoBehaviour
{
    int shotsTaken;
    public int levelNr;
    public bool playing;
    public int par;
    public int maxShots;
    GameObject player;
    public GameObject shitsTaken;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }


    public void startPlaying() 
    {
        if (!keepTrackOfPlayer.Instance.enteredALevel) 
        {
            tutorialHandler.Instance.ShowTooltip(3);
        }
        keepTrackOfPlayer.Instance.currentRespawnPoint = transform.position;
        playing = true;
        shotsTaken = 0;
        StartCoroutine(player.GetComponent<Golfer>().DampenVelocity());
        player.transform.position = transform.position;
        player.GetComponent<Golfer>().inLevel = true;
        shitsTaken.GetComponent<TextMeshProUGUI>().text = "0 / " + maxShots;
        shitsTaken.SetActive(true);
    }
    
    public void stopPlaying()
    {
        player.GetComponent<Golfer>().inLevel = false;
        shitsTaken.SetActive(false);
        playing = false;
        if (levelNr < 3)
        {
            keepTrackOfPlayer.Instance.currentRespawnPoint = new Vector3(-17, 0.3f, -9);
        }
        else if (levelNr > 2)
        {
            keepTrackOfPlayer.Instance.currentRespawnPoint = new Vector3(-16, 0.3f, -14);
        }

    }

    public IEnumerator takeShot()
    {
        if (shotsTaken < maxShots - 1) 
        {
            shotsTaken++;
            shitsTaken.GetComponent<TextMeshProUGUI>().text = shotsTaken + " / " + maxShots;

            yield return new WaitForSeconds(4);
            if (shotsTaken >= maxShots)
            {
                // used all shots without winning, restart level
                player.GetComponent<Golfer>().resetPosition();
                startPlaying();
            }
        }
    }

    public void winLevel()
    {
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.transform.position = transform.position;
        tutorialHandler.Instance.ShowTooltip(4);

        if (keepTrackOfPlayer.Instance.scores[levelNr - 1] == 0 || keepTrackOfPlayer.Instance.scores[levelNr - 1] > shotsTaken)
        {
            keepTrackOfPlayer.Instance.scores[levelNr - 1] = shotsTaken;
        }

        if (levelNr == 2 && !keepTrackOfPlayer.Instance.unlockedCatapult)
        {
            tutorialHandler.Instance.ShowTooltip(5);
            keepTrackOfPlayer.Instance.unlockedCatapult = true;
        }
    }
}
