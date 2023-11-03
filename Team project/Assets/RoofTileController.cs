using UnityEngine;

public class RoofTileController : MonoBehaviour
{
    private MeshRenderer roofRenderer;
    private bool playerInsideHouse = false;

    private void Start()
    {
        // Get the MeshRenderer component of the roof tile
        roofRenderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the house");
            // The player has entered the house, disable the roof tile's renderer
            roofRenderer.enabled = false;
            playerInsideHouse = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left the house");
            // The player has left the house, enable the roof tile's renderer
            roofRenderer.enabled = true;
            playerInsideHouse = false;
        }
    }

    // You can add additional logic here, for example, to check if the player is still inside the house.
}
