using UnityEngine;

public class ExpPickup : BasePickup
{

    [Range(5, 200)]
    [SerializeField] int value;

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Player>(out var player))
        {
            return;
        }
        player.GainExp(value);
        Destroy(gameObject);
    }

    void Update()
    {

    }
}
