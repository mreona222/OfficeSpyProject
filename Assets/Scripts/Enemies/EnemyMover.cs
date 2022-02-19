using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class EnemyMover : MonoBehaviour
    {
        public enum State
        {
            LookAround,
            Walking,
            Warning,
            CatchOut,
            Running,
            Arresting
        }

        // 状態遷移変数-----------------------------------------------------------------------------------------------------------------------------------------------------------------
        public State _currentState = State.LookAround;
        public State currentState
        {
            get => _currentState;
            set
            {
                if (isArresting)
                {
                    return;
                }
                _currentState = value;
            }
        }
        public bool isArresting => currentState == State.Arresting;

        // NavMeshAgent
        NavMeshAgent _enemyNavMeshAgent;

        // プレイヤーTransform
        [field: SerializeField] Transform _playerTransform;

        // target
        [field: SerializeField] Transform _targetTransform;
        private int _nextTarget = 0;

        // 数秒待つ
        private float _waitAFewSecounds = 0f;

        // プレイヤーの捜索
        RaycastHit[] _playerHit;
        RaycastHit[] _somethingNoticeHit;
        int _playerMask = 1 << 6;
        float _catchOutDistance = 3.0f;
        float _warnDistance = 5.0f;


        void Start()
        {
            _enemyNavMeshAgent = this.GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            switch (currentState)
            {
                case State.LookAround:
                    {
                        // 辺りを見渡す
                        if (_waitAFewSecounds < 3.0f)
                        {
                            _waitAFewSecounds += Time.deltaTime;
                        }
                        // 何も見つけられなければ歩き出す
                        else
                        {
                            Walk();
                        }
                        // 何か気になれば警戒状態
                        if (SomethingNotice())
                        {
                            _enemyNavMeshAgent.speed = 0;
                            Warn();
                        }
                        // プレイヤーを見つける
                        if (PlayerFinder())
                        {
                            _enemyNavMeshAgent.speed = 0;
                            CatchOut();
                        }

                    }break;
                case State.Walking:
                    {
                        // 目標地点に到達したら辺りを見回す
                        if (_enemyNavMeshAgent.remainingDistance <= _enemyNavMeshAgent.stoppingDistance)
                        {
                            LookAround();
                        }
                        // 何かに反応して警戒状態になる
                        if (SomethingNotice())
                        {
                            _enemyNavMeshAgent.speed = 0;
                            Warn();
                        }
                        // プレイヤーを見つける
                        if (PlayerFinder())
                        {
                            _enemyNavMeshAgent.speed = 0;
                            CatchOut();
                        }
                    }break;
                case State.Warning:
                    {
                        // 気になる場所を捜索する
                        if (_waitAFewSecounds < 0.5f)
                        {
                            if (_waitAFewSecounds == 0f) {
                                _enemyNavMeshAgent.speed = 0f;
                                _enemyNavMeshAgent.destination = _somethingNoticeHit[0].point;
                            }
                            _waitAFewSecounds += Time.deltaTime;
                        }
                        else
                        {
                            _enemyNavMeshAgent.speed = 2.0f;
                        }
                        // 辺りを見回す
                        if (_enemyNavMeshAgent.remainingDistance <= _enemyNavMeshAgent.stoppingDistance)
                        {
                            LookAround();
                        }
                        // プレイヤーを見つける
                        if (PlayerFinder())
                        {
                            _enemyNavMeshAgent.speed = 0;
                            CatchOut();
                        }

                    }break;
                case State.CatchOut:
                    {
                        // 走り始める
                        if (_waitAFewSecounds < 0.5f)
                        {
                            _waitAFewSecounds += Time.deltaTime;
                        }
                        else
                        {
                            Run();
                        }
                    }
                    break;
                case State.Running:
                    {
                        // 走る
                        _enemyNavMeshAgent.speed = 6.0f;
                        _enemyNavMeshAgent.stoppingDistance = 1.0f;
                        _enemyNavMeshAgent.destination = _playerTransform.position;
                        // プレイヤーを捕まえる
                        if (_enemyNavMeshAgent.remainingDistance <= _enemyNavMeshAgent.stoppingDistance)
                        {
                            Arrest();
                        }
                    }break;
                case State.Arresting:
                    {
                        _playerTransform.gameObject.GetComponent<Players.PlayerMover>().currentState = Players.PlayerMover.State.Arrested;
                    }
                    break;
            }
        }

        // 状態遷移--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 辺りを見渡す
        /// </summary>
        private void LookAround()
        {
            currentState = State.LookAround;
            _waitAFewSecounds = 0f;
        }

        /// <summary>
        /// 歩く
        /// </summary>
        private void Walk()
        {
            // 次のターゲットへ向けて歩く
            _nextTarget++;
            _enemyNavMeshAgent.SetDestination(_targetTransform.GetChild((_nextTarget) % _targetTransform.childCount).position);

            currentState = State.Walking;
            _waitAFewSecounds = 0f;
        }

        /// <summary>
        /// 警戒する
        /// </summary>
        private void Warn()
        {
            currentState = State.Warning;
            _waitAFewSecounds = 0f;
        }

        /// <summary>
        /// 見つける
        /// </summary>
        private void CatchOut()
        {
            currentState = State.CatchOut;
            _waitAFewSecounds = 0f;
        }

        /// <summary>
        /// 走る
        /// </summary>
        private void Run()
        {
            currentState = State.Running;
            _waitAFewSecounds = 0f;
        }

        /// <summary>
        /// 捕まえる
        /// </summary>
        private void Arrest()
        {
            currentState = State.Arresting;
            _waitAFewSecounds = 0f;
        }

        // メソッド------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// プレイヤーを見つける
        /// </summary>
        /// <returns></returns>
        private bool PlayerFinder()
        {
            _playerHit = Physics.SphereCastAll(this.transform.position + Vector3.up * 1.0f, 0.5f, this.transform.forward, _catchOutDistance);

            if (_playerHit.Length == 1)
            {
                if (_playerHit[0].transform.gameObject.CompareTag("Player"))
                {
                    return true;
                }
            }

            return false;
        }

        private bool SomethingNotice()
        {
            _somethingNoticeHit = Physics.SphereCastAll(this.transform.position + Vector3.up * 1.0f, 0.5f, this.transform.forward, _warnDistance);

            if (_somethingNoticeHit.Length == 1 && _somethingNoticeHit[0].distance > _catchOutDistance)
            {
                if (_somethingNoticeHit[0].transform.gameObject.CompareTag("Player"))
                {
                    return true;
                }
            }

            return false;
        }

        // ギズモ--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            //if (_playerHit.Length >= 1)
            //{
            //    Gizmos.color = Color.blue;
            //    Gizmos.DrawRay(this.transform.position + Vector3.up * 1.0f, this.transform.forward * _playerHit[0].distance);
            //    Gizmos.DrawWireSphere(this.transform.position + Vector3.up * 1.0f + this.transform.forward * _playerHit[0].distance, 0.5f);
            //}
        }
#endif
    }
}