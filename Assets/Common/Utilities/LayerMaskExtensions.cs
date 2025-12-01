using UnityEngine;

namespace KarenKrill.Utilities
{
    public static class LayerMaskExtensions
    {
        public static bool Contains(this LayerMask mask, int layer) => (1 << layer & mask) != 0;
    }
}
