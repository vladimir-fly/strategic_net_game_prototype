﻿using System;
using UnityEngine;

namespace SNGPClient
{
    public class Unit : MonoBehaviour
    {
        public byte Id;
        public event Action<byte> OnSelected;

        private bool _isMoving;
        private Vector3 _position;
        private Animator _animator;

        void Start()
        {
            _animator = GetComponent<Animator>();
        }

        void FixedUpdate()
        {
            if (!_isMoving) return;
            var direction = _position - transform.position;
            var targetPosition = Vector3.Distance(transform.position, _position);

            if (Math.Abs(targetPosition) > 0.01f)
                transform.Translate(direction * 0.1f);
            else
            {
                _isMoving = false;
                _animator.SetBool("IsWalk", false);
                _animator.SetBool("IsStand", true);
            }
        }

        void OnMouseDown()
        {
            if (OnSelected != null) OnSelected(Id);
        }

        internal void StartMoving(Direction direction)
        {
            //Dirty hack with screen cordiante system =)
            Vector3 directionVector;
            switch (direction)
            {
                case Direction.Up:
                    directionVector = Vector3.left;
                    Id -= 10;
                    break;
                case Direction.Down:
                    directionVector = Vector3.right;
                    Id += 10;
                    break;
                case Direction.Right:
                    Id += 1;
                    directionVector = Vector3.up;
                    break;
                case Direction.Left:
                    Id -= 1;
                    directionVector = Vector3.down;
                    break;
                case Direction.None:
                    Debug.Log("No direction");
                    directionVector = Vector3.zero;
                    break;
                default:
                    Debug.Log("Unknown direction!");
                    directionVector = Vector3.zero;
                    break;
            }

            _isMoving = true;
            _animator.SetBool("IsStand", false);
            _animator.SetBool("IsWalk", true);

            _position = transform.position + directionVector;
        }
    }
}