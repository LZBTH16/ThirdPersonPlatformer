using UnityEngine;

public class CoinController : MonoBehaviour
{

    [SerializeField] private float rotationSpeed = 90f; // degrees per second

    private void Update()
    {
        // Rotate the coin continuously around the Y-axis.
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the trigger.
        if (other.CompareTag("Player"))
        {
            // Destroy the coin once it's collected.
            Destroy(gameObject);
        }
    }
}
