using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyCompany.RogueSmash.Prototype
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
    public class CharacterControls : MonoBehaviour
    {
        // CONSTANTES
        private const float GRAVITY = -9.81f;

        // COMPONENTS
        private CharacterController _controller;
        private PlayerInput _playerInput;
        [SerializeField]
        private WeaponController _weaponController;
        [SerializeField]
        private WeaponController.WeaponControllerData _weaponData;
        // MOVEMNTS
        [SerializeField] private float playerSpeed = 12.0f;
        [SerializeField] private float runSpeed = 4.0f;
        [SerializeField] private float jumpHeight = 2.45f;
        private Vector3 _playerVelocity;
        private Vector3 _currentVelocity;
        private bool _isGrounded;
        private bool _isSprinting;
        //ACCIONS
        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _fireAction;
        private InputAction _sprintAction;


        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();

            _weaponController = new WeaponController(_weaponData);

        }

        private void Start()
        {
            _moveAction = _playerInput.actions["Move"];
            _jumpAction = _playerInput.actions["Jump"];
            _fireAction = _playerInput.actions["Attack"];
            _sprintAction = _playerInput.actions["Sprint"];

            _fireAction.performed += context => Shoot();
        }

        private void Update()
        {
            HandleMovement();
            HandleJump();            
        }

        private void HandleMovement()
        {
            _isGrounded = _controller.isGrounded;
            Vector2 moveInput = _moveAction.ReadValue<Vector2>();
            Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

            //toggling speed / walking
            float speed = playerSpeed;
            if (_isSprinting)
            {
                speed *= runSpeed;
                if (moveInput.y < 0)
                {
                    speed = speed / 2;
                }
            }

            _currentVelocity = (move * (Time.deltaTime * playerSpeed)) * speed;
            _controller.Move(_currentVelocity);

            if (move != Vector3.zero)
            {
                transform.forward = move;
            }

            //MOUSE TO 3D
            HandleMouseRotation();
        }

        private void HandleMouseRotation()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float distanceHit))
            {
                Vector3 mousePosition3D = ray.GetPoint(distanceHit);
                Vector3 direction = mousePosition3D - transform.position;
                direction.y = 0;

                if (direction.sqrMagnitude > 0.01f)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }
            }
        }

        private void HandleJump()
        {
            if (_isGrounded && _playerVelocity.y < 0)
            {
                _playerVelocity.y = 0f;
            }

            if (_jumpAction.IsPressed() && _isGrounded)
            {
                _playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * GRAVITY);
            }

            _playerVelocity.y += GRAVITY * Time.deltaTime;
            _controller.Move(_playerVelocity * Time.deltaTime);
        }

        private void Shoot()
        {
            _weaponController?.Use();
        }

        private void OnDestroy()
        {
            // Desuscribirse de eventos para evitar memory leaks
            _fireAction.performed -= _ => Shoot();
        }
    }
}
