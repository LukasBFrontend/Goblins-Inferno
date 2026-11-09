
using UnityEngine;

namespace Luscious
{
    [CreateAssetMenu(fileName = "CursorSettings", menuName = "Luscious/CursorSettings")]
    public class SO_CursorSettings : ScriptableObject
    {
        public CursorData defaultCursor;
        public CursorData progressCursor;
        public CursorData focusCursor;
    }
}
