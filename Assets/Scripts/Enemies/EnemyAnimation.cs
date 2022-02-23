using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class EnemyAnimation : MonoBehaviour
    {
        Animator _enemyAnim;

        EnemyMover _enemyMover;

        void Start()
        {
            _enemyAnim = GetComponent<Animator>();
            _enemyMover = GetComponent<EnemyMover>();
        }

        void Update()
        {
            _enemyAnim.SetInteger("state", (int)_enemyMover.currentState);

            _enemyAnim.SetInteger("lookaround", _enemyMover._enemyLookAroundAnimation);
        }
    }
}