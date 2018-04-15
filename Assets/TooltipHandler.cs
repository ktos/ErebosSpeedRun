using System.Collections;
using TMPro;
using UnityEngine;

public class TooltipHandler : MonoBehaviour
{
    public void ShowTooltip(string text, int secondsToClose)
    {
        GetComponent<Animator>().SetBool("IsTooltipOpened", true);
        GetComponent<TextMeshProUGUI>().text = text;

        StartCoroutine(CloseAfter(secondsToClose));
    }

    private IEnumerator CloseAfter(int seconds)
    {
        yield return new WaitForSeconds(seconds);

        GetComponent<Animator>().SetBool("IsTooltipOpened", false);
    }
}