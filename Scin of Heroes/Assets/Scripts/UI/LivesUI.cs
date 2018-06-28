using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class LivesUI : MonoBehaviour
{
    public Text livesText;

    private void Start()
    {
        StartCoroutine(LivesUpdate());
    }


    IEnumerator LivesUpdate()
    {
        while (true)
        {
            livesText.text = PlayerStats.Lives.ToString() + " LIVES";
            yield return new WaitForSeconds(0.1f);
        }
	}
}
