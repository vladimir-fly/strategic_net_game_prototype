using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SNGPClient
{
    public class Playground : MonoBehaviour
    {
        [SerializeField] public Node Node;
        [SerializeField] public Unit Unit;

        public Action<byte> NodeSelected;
        public List<Unit> SelectedUnits { get; private set; }

        private List<Node> _nodes;
        private List<GameObject> _selectableUnits;

        public Texture ScopeTexture;
        private Vector2 _scopeOrigin;
        private Vector2 _scopeSize;
        private Rect ScopeRect;

        public void InitNodes(byte n)
        {
            _nodes = new List<Node>();
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    var position = new Vector3(i - n/2 + 0.5f, j - n/2 + 0.5f, -1);
                    var node = Instantiate(Node, position, Quaternion.identity);
                    node.transform.SetParent(transform);
                    node.Id = (byte) (i * 10 + j);
                    node.OnSelected += NodeSelected;
                    node.gameObject.SetActive(true);
                    _nodes.Add(node);
                }
            }
        }

        public void InitUnits(IEnumerable<byte> units)
        {
            if (units == null || _nodes == null || _nodes.Count == 0) return;

            foreach (var unit in units)
            {
                var postion = _nodes.First(node => node.Id == unit).transform.position;
                var newUnit = Instantiate(Unit, postion, Quaternion.identity);
                newUnit.transform.SetParent(transform);
                newUnit.Id = unit;
                newUnit.OnSelected += id => SelectedUnits = new List<Unit>{newUnit};
                newUnit.gameObject.SetActive(true);
            }
        }

        private void OnGUI()
        {
            ScopeRect = new Rect(_scopeOrigin.x, _scopeOrigin.y, _scopeSize.x, _scopeSize.y);
            GUI.color = new Color(0,0,0, .3f);
            GUI.DrawTexture(ScopeRect, ScopeTexture);

        }
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _selectableUnits = new List<GameObject>(GameObject.FindGameObjectsWithTag("Selectable"));

                var invertedY = Screen.height - Input.mousePosition.y;
                _scopeOrigin = new Vector2(Input.mousePosition.x, invertedY);
            }

            if (Input.GetMouseButtonUp(0))
            {
                SelectedUnits = new List<Unit>();
                _scopeSize = Vector2.zero;

                foreach (var unit in _selectableUnits)
                {
                    var position = Camera.main.WorldToScreenPoint(unit.transform.position);
                    var screenPoint = new Vector2(position.x, Screen.height - position.y);

                    if (!ScopeRect.Contains(screenPoint))
                        unit.GetComponent<MeshRenderer>().material.color = new Color(0.177f,0.210f, 0.184f);

                    if (!ScopeRect.Contains(screenPoint)) continue;
                    unit.GetComponent<MeshRenderer>().material.color = Color.red;
                    SelectedUnits.Add(unit.GetComponent<Unit>());
                }
            }

            if (Input.GetMouseButton(0))
            {
                var invertedY = Screen.height - Input.mousePosition.y;
                _scopeSize = new Vector2(Input.mousePosition.x - _scopeOrigin.x, (_scopeOrigin.y - invertedY) * -1);
            }

        }
    }
}