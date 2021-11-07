using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosina.Components
{
    public static class CosinaMathf
    {
        public enum Direction
        {
            Left, Right,
            /// <summary>
            /// may be up, top
            /// </summary>
            Front,
            /// <summary>
            /// may be down, bottom
            /// </summary>
            Back
        }
        
        public static readonly Vector3 VEC_ZERO = Vector3.zero;
        public const float ZERO_BUT_GREATER = 0.001f;
        public const float ZERO_BUT_LESSER = -0.001f;
        
    }

}
