using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{
    public class PlayerSoundEffecter : MonoBehaviour
    {
        AudioSource _playerAudio;

        [field: SerializeField] AudioClip[] _clip;

        void Start()
        {
            _playerAudio = GetComponent<AudioSource>();
        }

        public void PlayerFootStepSE()
        {
            _playerAudio.volume = 0.6f;
            _playerAudio.PlayOneShot(_clip[0]);
        }

        public void PlayerRunFootStepSE()
        {
            _playerAudio.volume = 0.8f;
            _playerAudio.PlayOneShot(_clip[1]);
        }
    }
}