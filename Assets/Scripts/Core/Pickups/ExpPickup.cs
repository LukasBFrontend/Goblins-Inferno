using UnityEngine;

public class ExpPickup : MonoBehaviour
{
    
    [Range(5, 200)]
    [SerializeField] int value;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<Player>(out var player))
        {
            return;
        }
        player.GainExp(value);
        Destroy(gameObject);
    }
}
