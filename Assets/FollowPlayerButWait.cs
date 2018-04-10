using UnityEngine;

public class FollowPlayerButWait : MonoBehaviour
{
    public GameObject player;

    private void Update()
    {
        var distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance > 5)
        {
            transform.position = Vector3.Lerp(transform.position, player.transform.position - new Vector3(2, -3, 0), Time.deltaTime * 0.5f);
        }
    }
}