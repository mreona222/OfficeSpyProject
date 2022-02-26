using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{
    public class PlayerMover : MonoBehaviour
    {
        // 状態Enum
        public enum State
        {
            Idling,
            Walking,
            Running,
            Crouching,
            CrouchWalking,
            Arrested,
            GameClear,
        }

        // 状態遷移変数-----------------------------------------------------------------------------------------------------------------------------------------------------------------
        public State _currentState = State.Idling;
        public State currentState
        {
            get => _currentState;
            set
            {
                if (isArrested)
                {
                    return;
                }
                _currentState = value;
            }
        }
        public bool isArrested => currentState == State.Arrested;


        // 入力
        IInputEventProvider _playerInput;
        // Rigidbody
        Rigidbody _playerRigidbody;

        // 速度変数
        [field: SerializeField] float _maxSpeed = 0f;
        private float _currentMaxSpeed = 0f;

        void Start()
        {
            _playerInput = this.GetComponent<IInputEventProvider>();
            _playerRigidbody = this.GetComponent<Rigidbody>();
        }

        // Update-----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        void Update()
        {
            switch (currentState)
            {
                case State.Idling:
                    {
                        // 走り入力
                        if (_playerInput.Run)
                        {
                            // 何もしない
                        }
                        else
                        {
                            // 何もしない
                        }
                        // しゃがみ入力
                        if (_playerInput.Crouch)
                        {
                            Crouch();
                        }
                        else
                        {
                            // 何もしない
                        }
                        if (GameObject.Find("tablet") == null)
                        {
                            GameClear();
                        }
                    }
                    break;
                case State.Walking:
                    {
                        // 走り入力
                        if (_playerInput.Run)
                        {
                            Run();
                        }
                        else
                        {
                            // 何もしない
                        }
                        // しゃがみ入力
                        if (_playerInput.Crouch)
                        {
                            CrouchWalk();
                        }
                        else
                        {
                            // 何もしない
                        }
                    }
                    break;
                case State.Running:
                    {
                        // 走り入力
                        if (_playerInput.Run)
                        {
                            // 何もしない
                        }
                        else
                        {
                            Walk();
                        }
                        // しゃがみ入力
                        if (_playerInput.Crouch)
                        {
                            // 何もしない
                        }
                        else
                        {
                            // 何もしない
                        }
                    }
                    break;
                case State.Crouching:
                    {
                        // 走り入力
                        if (_playerInput.Run)
                        {
                            // 何もしない
                        }
                        else
                        {
                            // 何もしない
                        }
                        // しゃがみ入力
                        if (_playerInput.Crouch)
                        {
                            // 何もしない
                        }
                        else
                        {
                            Idle();
                        }
                    }
                    break;
                case State.CrouchWalking:
                    {
                        // 走り入力
                        if (_playerInput.Run)
                        {
                            // 何もしない
                        }
                        else
                        {
                            // 何もしない
                        }
                        // しゃがみ入力
                        if (_playerInput.Crouch)
                        {
                            // 何もしない
                        }
                        else
                        {
                            Walk();
                        }
                    }
                    break;
                case State.Arrested:
                    {
                        if (UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame)
                        {
                            UnityEngine.SceneManagement.SceneManager.LoadScene("Test01");
                        }
                    }
                    break;
                case State.GameClear:
                    {
                        if (UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame)
                        {
                            UnityEngine.SceneManagement.SceneManager.LoadScene("Test01");
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        // FixedUpdate-----------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void FixedUpdate()
        {
            switch (currentState)
            {
                case State.Idling:
                    {
                        if (_playerInput.Move != Vector2.zero)
                        {
                            Walk();
                        }
                        else
                        {
                            // 何もしない
                        }
                    }
                    break;
                case State.Walking:
                    {
                        if (_playerInput.Move != Vector2.zero)
                        {
                            ChangeDirection(_playerInput.Move);
                            MoveForward(_playerInput.Move);
                        }
                        else
                        {
                            Idle();
                        }
                    }
                    break;
                case State.Running:
                    {
                        if (_playerInput.Move != Vector2.zero)
                        {
                            ChangeDirection(_playerInput.Move);
                            MoveForward(_playerInput.Move);
                        }
                        else
                        {
                            Idle();
                        }
                    }
                    break;
                case State.Crouching:
                    {
                        if (_playerInput.Move != Vector2.zero)
                        {
                            CrouchWalk();
                        }
                        else
                        {
                            // 何もしない
                        }
                    }
                    break;
                case State.CrouchWalking:
                    {
                        if (_playerInput.Move != Vector2.zero)
                        {
                            ChangeDirection(_playerInput.Move);
                            MoveForward(_playerInput.Move);
                        }
                        else
                        {
                            Crouch();
                        }
                    }
                    break;
                case State.Arrested:
                    {

                    }
                    break;
                case State.GameClear:
                    {
                        
                    }
                    break;
            }
        }


        // 状態遷移--------------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// アイドリング
        /// </summary>
        private void Idle()
        {
            currentState = State.Idling;
        }

        /// <summary>
        /// 歩き
        /// </summary>
        private void Walk()
        {
            SpeedChange(_maxSpeed / 2.0f);
            currentState = State.Walking;
        }

        /// <summary>
        /// 走り
        /// </summary>
        private void Run()
        {
            SpeedChange(_maxSpeed / 1.0f);
            currentState = State.Running;
        }

        /// <summary>
        /// しゃがみ
        /// </summary>
        private void Crouch()
        {
            currentState = State.Crouching;
        }

        /// <summary>
        /// しゃがみ歩き
        /// </summary>
        private void CrouchWalk()
        {
            SpeedChange(_maxSpeed / 3.0f);
            currentState = State.CrouchWalking;
        }

        /// <summary>
        /// 捕まる
        /// </summary>
        private void Arrested()
        {
            currentState = State.Arrested;
        }

        /// <summary>
        /// ゲームクリア
        /// </summary>
        private void GameClear()
        {
            currentState = State.GameClear;
        }

        // メソッド--------------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 方向転換
        /// </summary>
        void ChangeDirection(Vector2 _direction)
        {
            // ゆっくり回転
            this.transform.rotation = 
                Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(new Vector3(_direction.x, 0, _direction.y), this.transform.up), 0.15f * Time.fixedDeltaTime * 50.0f);
        }

        /// <summary>
        /// 速度変更
        /// </summary>
        void SpeedChange(float speed)
        {
            _currentMaxSpeed = speed;
        }

        /// <summary>
        /// 直進
        /// </summary>
        void MoveForward(Vector2 _direction)
        {
            // 目標の速度
            Vector3 _targetSpeed = new Vector3(_direction.x, 0, _direction.y) * _currentMaxSpeed;

            // 目標の速度と現在の速度の差
            Vector3 applyforce = _targetSpeed - _playerRigidbody.velocity;

            // 上下方向の速度は無視する
            applyforce.y = 0f;

            // 加える力が大きすぎる場合
            if (applyforce.magnitude > _currentMaxSpeed)
            {
                applyforce = applyforce.normalized * _currentMaxSpeed;
            }

            // 力を加える
            _playerRigidbody.AddForce(applyforce, ForceMode.VelocityChange);
        }
    }

}