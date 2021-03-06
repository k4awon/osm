using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
   [SerializeField] private InputMaster _controls;
   [SerializeField] private Rigidbody2D _rb;
   [SerializeField] private Transform _groundCheck;
   [SerializeField] private GameObject _TopHalfMike;
   [SerializeField] private LayerMask _groundLayer;
   [SerializeField] private LayerMask _wallLayer;
   [SerializeField] private LayerMask _killableLayer;
   private RaycastHit2D _hit;
   private RaycastHit2D[] _potentialTargets;
   
   [SerializeField] private float _speed;
   [SerializeField] private float _jumpStrength;
   private float _strikeLength = Mathf.Infinity;
   private Vector2 _movementInput;
   private bool _hasStrike;

   private void Awake()
   {
      _rb = gameObject.GetComponent<Rigidbody2D>(); 
      _controls = new InputMaster();
   }

   private void Start()
   {
      _controls.Player.Movement.performed += _ => Movement();
      _controls.Player.Jump.performed += _ => Jump();
      _controls.Player.Strike.performed += _ => Strike();
      _controls.Player.Reset.performed += _ => RestartLevel();
   }

   private void FixedUpdate()
   {
      Movement();
   }

   // System
   private void OnEnable()
   {
      _controls.Enable(); 
   }

   private void OnDisable()
   {
      _controls.Disable();
   }

   private void RestartLevel()
   {
      GameManager.Instance.RestartLevel();
   }

   // Movement
   private void Movement()
   {
      _movementInput = _controls.Player.Movement.ReadValue<Vector2>();
      _rb.velocity = new Vector2(_movementInput.x * _speed, _rb.velocity.y);
   }
   private void Jump()
   {
      if (IsGrounded())
      {
         _rb.AddForce(new Vector2(_rb.velocity.x,_jumpStrength));
      }
   }

   private bool IsGrounded()
   {
      return Physics2D.OverlapCircle(_groundCheck.position, 0.2f, _groundLayer);
   }
   
   // Abilities
   // Old Strike Method for Teleport Attack.
   /* 
   private void Strike()
   {
      if (_hasStrike) return;
      
      _hit = Physics2D.Raycast(transform.position, transform.right, _strikeLength, _wallLayer);
      _potentialTargets = Physics2D.RaycastAll(transform.position, transform.right, _strikeLength, _killableLayer); 
      if (_hit)
      {
         ClearKillPath(_potentialTargets);
         transform.position = _hit.point - new Vector2(0.7f,0);
      }
      _hasStrike = true;
   }
   */
   private void ClearKillPath(RaycastHit2D[] targets)
   {
      foreach (var t in targets)
      {
         Destroy(t.transform.gameObject);
      }
   }

   private void Strike()
   {
      _potentialTargets = Physics2D.RaycastAll(transform.position, transform.right, _strikeLength, _killableLayer); 
      while (!_hasStrike)
      {
        // TODO: Insert Animation For 1 Strike Kick Here + Await First Few Frames to Fire trigger ClearKillPath
        ClearKillPath(_potentialTargets);
        Destroy(_TopHalfMike);
         _hasStrike = true;
      }
   }
}
