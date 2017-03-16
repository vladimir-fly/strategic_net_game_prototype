using System;
using UnityEngine;

namespace SNGPClient
{
    public class Node : MonoBehaviour
    {
        public byte Id;
        public event Action<byte> OnSelected;

        void OnMouseUp()
        {
            if (OnSelected != null)
                OnSelected(Id);

            Debug.Log("Node " + Id + "selected!");
        }
    }
}