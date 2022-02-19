using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cameras
{
    public class MainCameraController : MonoBehaviour
    {
        // プレイヤーオブジェクト
        [field: SerializeField] private GameObject _player = null;
        // カメラのオフセット
        [field: SerializeField] private Vector3 _offset = Vector3.zero;

        void Start()
        {
            
        }

        void LateUpdate()
        {
            this.transform.position = _player.transform.position + this._offset;
        }
    }
}