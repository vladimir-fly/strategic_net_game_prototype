using System.Collections.Generic;
using UnityEngine;

namespace SNGPClient
{
    public class Playground : MonoBehaviour
    {
        public byte[] PlaygroundData;

        public List<Unit> Units;
        public List<MonoBehaviour> Nodes;

        [SerializeField] public Node Node;

        public int N = 12;

        void Start()
        {
            for (var i = -N/2 ; i < N/2 ; i++)
            {
                for (var j = -N/2 ; j < N/2; j++)
                {
                    Debug.Log("i = " + i + "; j = " + j);
                    var position = new Vector3(i + 0.5f, j + 0.5f, -1);
                    var node = Instantiate(Node, position, Quaternion.identity);
                    node.transform.SetParent(transform);
                }
            }
        }


        public void Reload()
        {

        }

        public void Show()
        {

        }



        void OnMouseDown()
        {

        }

        public Texture MarqueeGraphics;

        private Vector2 _marqueeOrigin;
        private Vector2 _marqueeSize;


        public Rect BackupRect;
        public Rect MarqueeRect;

        public List<GameObject> SelectableUnits;

        private void OnGUI()
        {
            MarqueeRect = new Rect(_marqueeOrigin.x, _marqueeOrigin.y, _marqueeSize.x, _marqueeSize.y);
            GUI.color = new Color(0,0,0, .3f);
            GUI.DrawTexture(MarqueeRect, MarqueeGraphics);

        }
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Poppulate the selectableUnits array with all the selectable units that exist
                SelectableUnits = new List<GameObject>(GameObject.FindGameObjectsWithTag("Selectable"));

                var invertedY = Screen.height - Input.mousePosition.y;
                _marqueeOrigin = new Vector2(Input.mousePosition.x, invertedY);
            }

            if (Input.GetMouseButtonUp(0))
            {
                _marqueeSize = Vector2.zero;

                foreach (var unit in SelectableUnits)
                {

                    //Convert the world position of the unit to a screen position and then to a GUI point
                    var screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);
                    var screenPoint = new Vector2(screenPos.x, Screen.height - screenPos.y);
                    //Ensure that any units not within the marquee are currently unselected
                    if (!MarqueeRect.Contains(screenPoint) || !BackupRect.Contains(screenPoint))
                    {
                        unit.GetComponent<MeshRenderer>().material.color = Color.green;
                    }
                    if (MarqueeRect.Contains(screenPoint) || BackupRect.Contains(screenPoint))
                    {
                        unit.GetComponent<MeshRenderer>().material.color = Color.red;
                    }

                }
            }

            if (Input.GetMouseButton(0))
            {
                var invertedY = Screen.height - Input.mousePosition.y;
                _marqueeSize = new Vector2(Input.mousePosition.x - _marqueeOrigin.x, (_marqueeOrigin.y - invertedY) * -1);
            }

        }
    }
}