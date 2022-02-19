using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{
    public class PlayerAnimation : MonoBehaviour
    {
        Animator _playerAnim;

        PlayerMover _playerMover;

        void Start()
        {
            _playerAnim = this.GetComponent<Animator>();
            _playerMover = this.GetComponent<PlayerMover>();
        }

        void Update()
        {
            _playerAnim.SetInteger("state", ((int)_playerMover.currentState));
        }
    }
}