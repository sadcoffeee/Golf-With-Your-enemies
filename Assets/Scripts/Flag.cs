using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public GolfLevel myLevel;
    bool visible;
    GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

    }
    private void Update()
    {
        if (myLevel.playing)
        {
            if ((player.transform.position - transform.position).magnitude < 2f)
            {
                transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(true);

            }
        }
    }


    public IEnumerator moveFlag()
    {
        visible = !visible;

        if (transform.GetChild(0).childCount == 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        yield return null;
    }
}
