using UnityEngine;

public class CircleFollow : MonoBehaviour
{
    public Transform player; // Reference to the player's Transform component

    void Update()
    {
        // Update the position of the circle to follow the player's feet
        transform.position = new Vector3(player.position.x, player.position.y - 1f, player.position.z);
    }
}
