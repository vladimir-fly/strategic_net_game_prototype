using System;
using System.Collections.Generic;
using System.Linq;

namespace sngp_server
{
    public class Playground
    {
        private List<byte> _units;
        private readonly byte _size;
        private int[,] _playground;
        public Queue<Point> _pointBuffer = new Queue<Point>();

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
                {
                    _playground[i, j] = _units.Any(u => u == i * 10 + j && u != unitId) ? -1 : 0;
                    Console.Write(_playground[i, j] + " ");
                }
                Console.WriteLine();
            }

            Console.WriteLine($"UnitId = {unitId}, i = {unitId / 10}, j = {unitId % 10}");

            MakeWaves(new Point { x= unitId / 10, y = unitId % 10}, nodeId);

            var path = FindPath(
                target: new Point{ x = nodeId / 10, y = nodeId % 10 },
                start: new Point { x= unitId / 10, y = unitId % 10 });

            Console.WriteLine($"Path length {path.Count}");

            var direction = GetNextDirection(path);

            Console.WriteLine($"Next direction = {direction}");

            ShowPlayground();

            var index = _units.FindIndex(u => unitId == u);

            Console.WriteLine($"Moving unit. Id = {unitId}, index = {index} ");
            _units[index] = (byte) (path.Peek().x * 10 + path.Peek().y);

            ShowPlayground();

            return direction;
        }


        private Stack<Point> FindPath(Point target, Point start)
        {
            var result = new Stack<Point>();
            result.Push(target);
            var point = target;

            while (point != start)
            {
                if (point.y - 1 >= 0 && _playground[point.x, point.y - 1] != -1 &&
                    _playground[point.x, point.y] - _playground[point.x, point.y - 1] == 1) //up
                {
                    point = new Point {x = point.x, y = point.y - 1};
                    result.Push(point);
                    continue;
                }

                if (point.y + 1 < _size && _playground[point.x, point.y + 1] != -1 &&
                    _playground[point.x, point.y] - _playground[point.x, point.y + 1] == 1) //down
                {
                    point = new Point {x = point.x, y = point.y + 1};
                    result.Push(point);
                    continue;
                }

                if (point.x + 1 < _size && _playground[point.x + 1, point.y] != -1 &&
                    _playground[point.x, point.y] - _playground[point.x + 1, point.y] == 1) //right
                {
                    point = new Point {x = point.x + 1, y = point.y};
                    result.Push(point); continue;
                }

                if (point.x - 1 >= 0 && _playground[point.x - 1, point.y] != -1 &&
                    _playground[point.x, point.y] - _playground[point.x - 1, point.y] == 1) //left
                {
                    point = new Point {x = point.x - 1, y = point.y};
                    result.Push(point);
                }

            }
            return result;
        }

        private byte GetNextDirection(Stack<Point> path)
        {
            if (path.Count == 0) return 0;
            var currentPoint = path.Pop();

            if (path.Peek().x - currentPoint.x == -1 && path.Peek().y == currentPoint.y) return 1; //up
            if (path.Peek().x - currentPoint.x == 1 && path.Peek().y == currentPoint.y) return 2; //down
            if (path.Peek().x == currentPoint.x && path.Peek().y - currentPoint.y == 1) return 3; //right
            if (path.Peek().x == currentPoint.x && path.Peek().y - currentPoint.y == -1) return 4; //left

            return 0;
        }

        private Point SafeDequeue()
        {
            return _pointBuffer.Count > 0 ? _pointBuffer.Dequeue() : new Point{x = -1, y = -1};
        }

        public void MakeWaves(Point start, int targetNodeId)
        {

            if (start.x < 0 || start.x > _size || start.y < 0|| start.y > _size  /*|| _playground[x, y] == -1*/) return;

            _pointBuffer.Enqueue(start);

            Point point;
            while ((point = SafeDequeue()).x != -1 && point.y != -1)
            {
                if (point.y - 1 >= 0 && _playground[point.x, point.y - 1] == 0)//&& (point.x * 10 + (point.y - 1)) != targetNodeId) //up
                {
                    var tpoint = new Point {x = point.x, y = point.y - 1};
                    if (tpoint != start)
                    {
                        _playground[point.x, point.y - 1] = _playground[point.x, point.y] + 1;
                        _pointBuffer.Enqueue(tpoint);
                    }
                }

                if (point.y + 1 < _size && _playground[point.x, point.y + 1] == 0)// && (point.x * 10 + (point.y + 1)) != targetNodeId) //down
                {
                    var tpoint = new Point {x = point.x, y = point.y + 1};
                    if (tpoint != start)
                    {
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
        }

        private void ShowPlayground()
        {
            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++)
                {
                    var result = _units.Any(u => u == i * 10 + j) ? i * 10 + j : 0;
                    Console.Write($" {result} ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}