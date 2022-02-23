using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using UnityEngine.UI;

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

        // LookAroundアニメーション用
        public int _enemyLookAroundAnimation = 0;

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
        RaycastHit[] _playerHit = new RaycastHit[6];
        RaycastHit[] _somethingNoticeHit = new RaycastHit[6];

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
                        if (_waitAFewSecounds < 0.6f)
                        {
                            this.transform.Rotate(90.0f * Time.deltaTime * new Vector3(0, 1.0f, 0));
                            _enemyLookAroundAnimation = 1;
                            _waitAFewSecounds += Time.deltaTime;
                        }
                        else if (_waitAFewSecounds < 1.2f)
                        {
                            _enemyLookAroundAnimation = 0;
                            _waitAFewSecounds += Time.deltaTime;
                        }
                        else if (_waitAFewSecounds < 2.4f)
                        {
                            _enemyLookAroundAnimation = -1;
                            this.transform.Rotate(90.0f * Time.deltaTime * new Vector3(0, -1.0f, 0));
                            _waitAFewSecounds += Time.deltaTime;
                        }
                        else if (_waitAFewSecounds < 3.0f)
                        {
                            _enemyLookAroundAnimation = 0;
                            _waitAFewSecounds += Time.deltaTime;
                        }
                        // 何も見つけられなければ歩き出す
                        else
                        {
                            _enemyLookAroundAnimation = 0;
                            Walk();
                        }
                        // 何か気になれば警戒状態
                        if (SomethingNotice())
                        {
                            _enemyLookAroundAnimation = 0;
                            _enemyNavMeshAgent.speed = 0;
                            Warn();
                        }
                        // プレイヤーを見つける
                        if (PlayerFinder())
                        {
                            _enemyLookAroundAnimation = 0;
                            _enemyNavMeshAgent.speed = 0;
                            CatchOut();
                        }
                    }
                    break;
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
                    }
                    break;
                case State.Warning:
                    {
                        // 気になる場所を捜索する
                        if (_waitAFewSecounds < 1.0f)
                        {
                            if (_waitAFewSecounds == 0f)
                            {
                                _enemyNavMeshAgent.speed = 0f;
                            }
                            _waitAFewSecounds += Time.deltaTime;
                        }
                        else
                        {
                            _enemyNavMeshAgent.speed = 3.0f;
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
                    }
                    break;
                case State.CatchOut:
                    {
                        // 走り始める
                        if (_waitAFewSecounds < 0.5f)
                        {
                            if (_waitAFewSecounds == 0f)
                            {
                                _enemyNavMeshAgent.destination = _playerTransform.position;
                            }
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
                        _enemyNavMeshAgent.speed = 4.5f;
                        _enemyNavMeshAgent.destination = _playerTransform.position;
                        _enemyNavMeshAgent.stoppingDistance = 1.5f;
                        // プレイヤーを捕まえる
                        if (_enemyNavMeshAgent.remainingDistance <= _enemyNavMeshAgent.stoppingDistance)
                        {
                            Arrest();
                        }
                    }
                    break;
                case State.Arresting:
                    {
                        _playerTransform.gameObject.GetComponent<Players.PlayerMover>().currentState = Players.PlayerMover.State.Arrested;
                    }
                    break;
                default:
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
            _enemyNavMeshAgent.destination = _targetTransform.GetChild((_nextTarget) % _targetTransform.childCount).position;

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
            // 1本目
            if (Physics.SphereCast(this.transform.position + Vector3.up * 0.7f, 0.3f, this.transform.forward, out _playerHit[0], _catchOutDistance))
            {
                if (_playerHit[0].transform.gameObject.CompareTag("Player"))
                {
                    return true;
                }
            }
            // 2本目
            if (Physics.SphereCast(this.transform.position + Vector3.up * 0.7f, 0.3f, Quaternion.AngleAxis(10.0f, Vector3.up) * this.transform.forward, out _playerHit[1], _catchOutDistance))
            {
                if (_playerHit[1].transform.gameObject.CompareTag("Player"))
                {
                    return true;
                }
            }
            // 3本目
            if (Physics.SphereCast(this.transform.position + Vector3.up * 0.7f, 0.3f, Quaternion.AngleAxis(-10.0f, Vector3.up) * this.transform.forward, out _playerHit[2], _catchOutDistance))
            {
                if (_playerHit[2].transform.gameObject.CompareTag("Player"))
                {
                    return true;
                }
            }
            // 4本目
            if (Physics.SphereCast(this.transform.position + Vector3.up * 1.7f, 0.3f, this.transform.forward, out _playerHit[3], _catchOutDistance))
            {
                if (_playerHit[3].transform.gameObject.CompareTag("Player"))
                {
                    return true;
                }
            }
            // 5本目
            if (Physics.SphereCast(this.transform.position + Vector3.up * 1.7f, 0.3f, Quaternion.AngleAxis(10.0f, Vector3.up) * this.transform.forward, out _playerHit[4], _catchOutDistance))
            {
                if (_playerHit[4].transform.gameObject.CompareTag("Player"))
                {
                    return true;
                }
            }
            // 6本目
            if (Physics.SphereCast(this.transform.position + Vector3.up * 1.7f, 0.3f, Quaternion.AngleAxis(-10.0f, Vector3.up) * this.transform.forward, out _playerHit[5], _catchOutDistance))
            {
                if (_playerHit[5].transform.gameObject.CompareTag("Player"))
                {
                    return true;
                }
            }

            return false;
        }

        private bool SomethingNotice()
        {
            // 1本目
            if (Physics.SphereCast(this.transform.position + Vector3.up * 0.7f, 0.3f, this.transform.forward, out _somethingNoticeHit[0], _warnDistance))
            {
                if (_somethingNoticeHit[0].transform.gameObject.CompareTag("Player"))
                {
                    _enemyNavMeshAgent.destination = _somethingNoticeHit[0].point;
                    return true;
                }
            }
            // 2本目
            if (Physics.SphereCast(this.transform.position + Vector3.up * 0.7f, 0.3f, Quaternion.AngleAxis(10.0f, Vector3.up) * this.transform.forward, out _somethingNoticeHit[1], _warnDistance))
            {
                if (_somethingNoticeHit[1].transform.gameObject.CompareTag("Player"))
                {
                    _enemyNavMeshAgent.destination = _somethingNoticeHit[1].point;
                    return true;
                }
            }
            // 3本目
            if (Physics.SphereCast(this.transform.position + Vector3.up * 0.7f, 0.3f, Quaternion.AngleAxis(-10.0f, Vector3.up) * this.transform.forward, out _somethingNoticeHit[2], _warnDistance))
            {
                if (_somethingNoticeHit[2].transform.gameObject.CompareTag("Player"))
                {
                    _enemyNavMeshAgent.destination = _somethingNoticeHit[2].point;
                    return true;
                }
            }
            // 4本目
            if (Physics.SphereCast(this.transform.position + Vector3.up * 1.7f, 0.3f, this.transform.forward, out _somethingNoticeHit[3], _warnDistance))
            {
                if (_somethingNoticeHit[3].transform.gameObject.CompareTag("Player"))
                {
                    _enemyNavMeshAgent.destination = _somethingNoticeHit[3].point;
                    return true;
                }
            }
            // 5本目
            if (Physics.SphereCast(this.transform.position + Vector3.up * 1.7f, 0.3f, Quaternion.AngleAxis(10.0f, Vector3.up) * this.transform.forward, out _somethingNoticeHit[4], _warnDistance))
            {
                if (_somethingNoticeHit[4].transform.gameObject.CompareTag("Player"))
                {
                    _enemyNavMeshAgent.destination = _somethingNoticeHit[4].point;
                    return true;
                }
            }
            // 6本目
            if (Physics.SphereCast(this.transform.position + Vector3.up * 1.7f, 0.3f, Quaternion.AngleAxis(-10.0f, Vector3.up) * this.transform.forward, out _somethingNoticeHit[5], _warnDistance))
            {
                if (_somethingNoticeHit[5].transform.gameObject.CompareTag("Player"))
                {
                    _enemyNavMeshAgent.destination = _somethingNoticeHit[5].point;
                    return true;
                }
            }

            return false;
        }

        // ギズモ--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawRay(this.transform.position + Vector3.up * 0.7f, this.transform.forward * _catchOutDistance);
            Gizmos.DrawRay(this.transform.position + Vector3.up * 0.7f, Quaternion.AngleAxis(10.0f, Vector3.up) * this.transform.forward * _catchOutDistance);
            Gizmos.DrawRay(this.transform.position + Vector3.up * 0.7f, Quaternion.AngleAxis(-10.0f, Vector3.up) * this.transform.forward * _catchOutDistance);

            Gizmos.DrawRay(this.transform.position + Vector3.up * 1.7f, this.transform.forward * _catchOutDistance);
            Gizmos.DrawRay(this.transform.position + Vector3.up * 1.7f, Quaternion.AngleAxis(10.0f, Vector3.up) * this.transform.forward * _catchOutDistance);
            Gizmos.DrawRay(this.transform.position + Vector3.up * 1.7f, Quaternion.AngleAxis(-10.0f, Vector3.up) * this.transform.forward * _catchOutDistance);
        }
#endif
    }
}