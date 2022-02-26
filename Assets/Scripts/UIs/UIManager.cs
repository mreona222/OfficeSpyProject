using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Players;

namespace UIManagers
{
    public class UIManager : MonoBehaviour
    {
        PlayerMover _playerMover;

        void Start()
        {
            _playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        }

        void Update()
        {
            switch (_playerMover.currentState)
            {
                case PlayerMover.State.Arrested:
                    {
                        GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(true);
                    }
                    break;
                case PlayerMover.State.GameClear:
                    {
                        GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(true);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}