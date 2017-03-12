using UnityEngine;

namespace SNGPClient
{
    public class Unit : MonoBehaviour
    {
        private Animator _animator;

        void Start()
        {
            _animator = GetComponent<Animator>();
        }

        void Update()
        {
            var move = Input.GetAxis("Vertical");
        }

        void OnMouseUp()
        {
            Debug.Log("unit click");
            _animator.SetBool("IsWalk", true);
        }
    }
}