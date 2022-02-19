using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{
    public interface IInputEventProvider
    {
        // ˆÚ“®•Ï”
        public Vector2 Move { get; set; }

        // ‘–‚è‚ğ—LŒø‰»
        public bool Run { get; set; }

        // ‚µ‚á‚ª‚İ
        public bool Crouch { get; set; }
    }
}