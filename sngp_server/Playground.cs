using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace sngp_server
{
    public class Playground
    {
        private List<byte> _units;
        private readonly byte _size;
        private int[,] _playground;
        public Queue<Point> _pointBuffer = new Queue<Point>();

        public Playground(byte size, List<byte> units)
        {
            _size = size;
            _units = units;
        }

        public byte MoveUnit(int unitId, byte nodeId)
        {
            _playground = new int[_size, _size];

            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++)
                    Console.Write($" {i * 10 + j}");
                Console.WriteLine();
            }

            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++)
                {
                    _playground[i, j] = _units.Any(u => u == i * 10 + j && u != unitId) ? -1 : 0;
                    Console.Write(_playground[i, j] + " ");
                }
                Console.WriteLine();
            }

            Console.WriteLine($"UnitId = {unitId}, i = {unitId / 10}, j = {unitId % 10}");

            MakeWaves(new Point { x= unitId / 10, y = unitId % 10}, nodeId);

            //FindPath()

            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++)
                    Console.Write($"    {_playground[i, j]}");
                Console.WriteLine();
            }

            return 0; //direction
        }

        public struct Point
        {
            public int x;
            public int y;

            public static bool operator !=(Point a, Point b)
            {

                return a.x != b.x || a.y != b.y;
            }
            public static bool operator ==(Point a, Point b)
            {
                return a.x == b.x && a.y == b.y;
            }
        }


        private Point pop()
        {
            return _pointBuffer.Count > 0 ? _pointBuffer.Dequeue() : new Point{x = -1};
        }


        public void MakeWaves(Point start, int targetNodeId)
        {
            Console.WriteLine($"start = {_playground[start.x, start.y]}");

            if (start.x < 0 || start.x > _size || start.y < 0|| start.y > _size  /*|| _playground[x, y] == -1*/) return;

            _pointBuffer.Enqueue(start);

            Point point;
            while ((point = pop()).x != -1)
            {
                if (point.y - 1 >= 0 && _playground[point.x, point.y - 1] == 0)//&& (point.x * 10 + (point.y - 1)) != targetNodeId) //up
                {
                    var tpoint = new Point {x = point.x, y = point.y - 1};
                    if (tpoint != start)
                    {
                        //Console.WriteLine($"Setting to point {point.x},{point.y - 1} = {_playground[point.x, point.y] + 1}");
                        _playground[point.x, point.y - 1] = _playground[point.x, point.y] + 1;
                        _pointBuffer.Enqueue(tpoint);
                    }
                }

                if (point.y + 1 < _size && _playground[point.x, point.y + 1] == 0)// && (point.x * 10 + (point.y + 1)) != targetNodeId) //down
                {
                    var tpoint = new Point {x = point.x, y = point.y + 1};
                    if (tpoint != start)
                    {
                        Console.WriteLine($"Setting to point {point.x},{point.y + 1} = {_playground[point.x, point.y] + 1}");
                        _playground[point.x, point.y + 1] = _playground[point.x, point.y] + 1;
                        _pointBuffer.Enqueue(tpoint);
                    }
                }

                if (point.x + 1 < _size && _playground[point.x + 1, point.y] == 0)// && ((point.x + 1) * 10 + point.y) != targetNodeId) //right
                {
                    var tpoint = new Point {x = point.x + 1, y = point.y};
                    if (tpoint != start)
                    {
                        _playground[point.x + 1, point.y] = _playground[point.x, point.y] + 1;
                        _pointBuffer.Enqueue(tpoint);
                    }
                }

                if (point.x - 1 >= 0 && _playground[point.x - 1, point.y] == 0)// && ((point.x - 1) * 10 + point.y) != targetNodeId) //left
                {
                    var tpoint = new Point {x = point.x - 1, y = point.y};
                    if (tpoint != start)
                    {
                        _playground[point.x - 1, point.y] = _playground[point.x, point.y] + 1;
                        _pointBuffer.Enqueue(tpoint);
                    }
                }

            }

//            int node = 0;
//
//
//            while ((node = pop()) != -1)
//            {
//                if (y - 1 >= 0 && _playground[x, y - 1] == 0 && (x * 10 + (y -1)) != selectedUnit ) //up
//                {
//                    _playground[x, y - 1] = _playground[x, y] + 1;
//                    buffer.Enqueue(x * 10 + y - 1);
//                }
//
//                if (y + 1 < _size && _playground[x, y + 1] == 0 && (x * 10 + (y +1)) != selectedUnit) //down
//                {
//                    _playground[x, y + 1] = _playground[x, y] + 1;
//                    MakeWaves(x, y + 1);
//                }
//
//                if (x + 1 < _size && _playground[x + 1, y] == 0 && ((x + 1) * 10 + y) != selectedUnit) //right
//                {
//                    _playground[x + 1, y] = _playground[x, y] + 1;
//                    MakeWaves(x + 1, y);
//                }
//
//                if (x - 1 >= 0 && _playground[x - 1, y] == 0 && ((x-1) * 10 + y) != selectedUnit)  //left
//                {
//                    _playground[x - 1, y] = _playground[x, y] + 1;
//                    MakeWaves(x - 1, y);
//                }
//            }

//            if (pathLength != _playground[x, y]) return;


//            if (y - 1 >= 0 && _playground[x, y - 1] == 0 && (x * 10 + (y -1)) != targetNodeId) //up
//            {
//                _playground[x, y - 1] = _playground[x, y] + 1;
//
//                MakeWaves(x, y - 1, targetNodeId);
//            }
//
//            if (y + 1 < _size && _playground[x, y + 1] == 0 && (x * 10 + (y +1)) != targetNodeId) //down
//            {
//                _playground[x, y + 1] = _playground[x, y] + 1;
//
//                MakeWaves(x, y + 1, targetNodeId);
//            }
//
//            if (x + 1 < _size && _playground[x + 1, y] == 0 && ((x + 1) * 10 + y) != targetNodeId) //right
//            {
//                _playground[x + 1, y] = _playground[x, y] + 1;
//
//                MakeWaves(x + 1, y, targetNodeId);
//            }
//
//            if (x - 1 >= 0 && _playground[x - 1, y] == 0 && ((x-1) * 10 + y) != targetNodeId)  //left
//            {
//                _playground[x - 1, y] = _playground[x, y] + 1;
//                MakeWaves(x - 1, y, targetNodeId);
//            }
//
//            _playground[x, y] = -5;
        }
    }
}