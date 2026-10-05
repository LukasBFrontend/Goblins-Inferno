using UnityEngine;

public class SwordsmanController : BaseEnemyController
{
    void Awake()
    {
        Initialize();
    }

    void Update()
    {
        TrackPlayer();
    }
}
