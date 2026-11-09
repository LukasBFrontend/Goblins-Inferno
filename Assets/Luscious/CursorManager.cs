using UnityEngine;
using System.Collections;
using System;

namespace Luscious
{
    public static class CursorManager
    {
        private static Coroutine _activeCoroutine;

        public static void SetCursor(CursorData cursorData)
        {
            if (_activeCoroutine != null)
            {
                Game.Player.StopCoroutine(_activeCoroutine);
            }

            InitializeCursor(cursorData);
        }

        private static void InitializeCursor(CursorData cursorData)
        {
            Texture2D firstFrame = cursorData.spriteArray[0];
            int frameCount = cursorData.spriteArray.Length;
            Vector2 offset = cursorData.offset;

            Cursor.SetCursor(firstFrame, offset, CursorMode.Auto);
            if (frameCount < 2)
            {
                _activeCoroutine = null;
                return;
            }

            _activeCoroutine = Game.Player.StartCoroutine(AnimateCursorRoutine(cursorData));
        }

        private static IEnumerator AnimateCursorRoutine(CursorData settings)
        {
            int currentFrame = 0;

            while (true)
            {
                yield return new WaitForSecondsRealtime(1f / settings.fps);

                currentFrame = (currentFrame + 1) % settings.spriteArray.Length;

                Cursor.SetCursor(
                    settings.spriteArray[currentFrame],
                    settings.offset,
                    CursorMode.Auto
                );
            }
        }
    }

    [System.Serializable]
    public struct CursorData
    {
        public Vector2 offset;
        public Texture2D[] spriteArray;
        public float fps;
    }
}
