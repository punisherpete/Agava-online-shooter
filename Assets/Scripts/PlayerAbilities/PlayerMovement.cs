using System;
using CharacterInput;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace PlayerAbilities
{
    [RequireComponent(typeof(CharacterController),
        typeof(GroundChecker),
        typeof(PlayerInfo))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _inputSourceBehaviour;
        [SerializeField] private float _speed,
            _jumpSpeed = 18f,
            _gravityFactor = 1f;
        
        private ICharacterInputSource _inputSource;
        private PlayerInfo _playerInfo;
        private CharacterController _character;
        private GroundChecker _groundChecker;
        private Vector3 _moveDirection;
        private float _ySpeed;

        private void OnValidate()
        {
            if (_inputSourceBehaviour 
                && !(_inputSourceBehaviour is ICharacterInputSource))
            {
                Debug.LogError(nameof(_inputSourceBehaviour) + " needs to implement " + nameof(ICharacterInputSource));
                _inputSourceBehaviour = null;
            }
        } 
        
        public void Initialize(ICharacterInputSource inputSource)
        {
            _inputSource = inputSource;
        }
        
        private void Awake()
        {
            _character = GetComponent<CharacterController>();
            _playerInfo = GetComponent<PlayerInfo>();
            _groundChecker = GetComponent<GroundChecker>();
            if (_inputSource == null)
            {
                Initialize((ICharacterInputSource)_inputSourceBehaviour);
            }
        }

        private void Update()
        {
            if (_playerInfo.IsMine == false)
                return;
            
            var inputDirection = new Vector3(_inputSource.MovementInput.x, 
                0f, _inputSource.MovementInput.y);
            var isJump = _inputSource.IsJumpInput;
            Move(inputDirection, isJump);
        }

        private void Move(Vector3 inputDirection, bool isJump)
        {
            _moveDirection = GetMoveDirection(inputDirection);
            var distance = _moveDirection * _speed;
            distance += Vector3.up * CalculateYSpeed(isJump);
            _character.Move(distance * Time.deltaTime);
            _ySpeed = _character.velocity.y;
        }
        
        private float CalculateYSpeed(bool isJump)
        {
            if (_groundChecker.IsGround && isJump)
            {
                _ySpeed = _jumpSpeed;
            }
            
            _ySpeed += _gravityFactor * Physics.gravity.y * Time.deltaTime;

            return _ySpeed;
        }

        private Vector3 GetMoveDirection(Vector3 inputDirection)
        {
            Vector3 moveDirectionForward = transform.forward * inputDirection.z;
            Vector3 moveDirectionSide = transform.right * inputDirection.x;

            return (moveDirectionForward + moveDirectionSide).normalized;
        }
    }
}
