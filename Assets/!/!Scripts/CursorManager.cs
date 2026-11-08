using UnityEngine;

public class CursorManager : MonoBehaviour
{

    [SerializeField] CursorSettings defaultCursor;

    private int _currentFrame;
    private float _frameTimer;

    void Start()
    {
        Cursor.SetCursor(defaultCursor.textureArray[0], defaultCursor.offset, CursorMode.Auto);
    }


    void Update()
    {
        _frameTimer -= Time.deltaTime;

        if (_frameTimer <= 0f)
        {
            _frameTimer += defaultCursor.frameRate;
            _currentFrame = (_currentFrame + 1) % defaultCursor.frameCount;
            Cursor.SetCursor(defaultCursor.textureArray[_currentFrame], defaultCursor.offset, CursorMode.Auto);
        }
    }

    [System.Serializable]
    struct CursorSettings
    {
        public int frameCount;
        public float frameRate;
        public Vector2 offset;
        public Texture2D[] textureArray;

    }
}
