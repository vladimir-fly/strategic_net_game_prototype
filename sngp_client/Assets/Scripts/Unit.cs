using System;

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

            Debug.Log("target position = " + targetPosition);

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
            //StartMoving(Direction.Right);

            if (OnSelected != null)
                OnSelected(Id);

            Debug.Log("unit selected id : " + Id);
        }

        internal void StartMoving(Direction direction)
        {
            Debug.Log("Unit: " + Id + "; start moving in direction = " + direction);

            Vector3 directionVector;
            switch (direction)
            {
                case Direction.Up:
                    directionVector = Vector3.up;
                    break;
                case Direction.Down:
                    directionVector = Vector3.down;
                    break;
                case Direction.Right:
                    directionVector = Vector3.right;
                    break;
                case Direction.Left:
                    directionVector = Vector3.left;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }

            _isMoving = true;
            _animator.SetBool("IsStand", false);
            _animator.SetBool("IsWalk", true);

            _position = transform.position + directionVector;
        }
    }
}