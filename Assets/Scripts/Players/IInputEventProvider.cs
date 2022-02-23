using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{
    public interface IInputEventProvider
    {
        // 移動変数
        public Vector2 Move { get; set; }

        // 走りを有効化
        public bool Run { get; set; }

        // しゃがみ
        public bool Crouch { get; set; }

        // インタラクト
        public bool Interact { get; set; }
    }
}