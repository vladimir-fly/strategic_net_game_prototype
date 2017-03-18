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

        public Playground(byte size, List<byte> units)
        {
            _size = size;
            _units = units;
        }

        public byte MoveUnit(int unitId, byte nodeId)
        {
            _playground = new int[_size, _size];
            var unitPoint = new Point {x = unitId / 10, y = unitId % 10};
            var targetNodePoint = new Point {x = nodeId / 10, y = nodeId % 10};

            InitOtherUnits(unitId);
            MakeWaves(unitPoint);

            ShowPlayground();

            var path = FindPath(targetNodePoint, unitPoint);
            var direction = GetNextDirection(path);
            var index = _units.FindIndex(u => unitId == u);

            if (path.Any() && index >= 0)
                _units[index] = (byte) (path.Peek().x * 10 + path.Peek().y);

            ShowPlayground();

            return direction;
        }

        private void InitOtherUnits(int unitId)
        {
            for (var i = 0; i < _size; i++)
            for (var j = 0; j < _size; j++)
                _playground[i, j] = _units.Any(u => u == i * 10 + j && u != unitId) ? -1 : 0;
        }

        private Stack<Point> FindPath(Point target, Point start)
        {
            var result = new Stack<Point>();
            var point = target;

            if (_playground[point.x, point.y] == -1)
                point = GetMinNearbyPoint(point);

            result.Push(point);

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

        private Point GetMinNearbyPoint(Point target)
        {
            var mooreNeighborhood = new List<Point>();

            if (target.y - 1 > 0 && _playground[target.x, target.y - 1] != -1)
                mooreNeighborhood.Add(new Point{x = target.x, y = target.y - 1}); //North

            if (target.y + 1 < _size && _playground[target.x, target.y + 1] != -1)
                mooreNeighborhood.Add(new Point{x = target.x, y = target.y + 1}); //South

            if (target.x + 1 < _size && _playground[target.x + 1, target.y] != -1)
                mooreNeighborhood.Add(new Point{x = target.x + 1, y = target.y}); //East

            if (target.x - 1 > 0 && _playground[target.x - 1, target.y] != -1)
                mooreNeighborhood.Add(new Point{x = target.x - 1, y = target.y}); //West

            if (target.x - 1 > 0 && target.y - 1 > 0 && _playground[target.x - 1, target.y - 1] != -1)
                mooreNeighborhood.Add(new Point{x = target.x - 1, y = target.y - 1}); //NorthWest

            if (target.x + 1 < _size && target.y - 1 > 0 && _playground[target.x + 1, target.y - 1] != -1)
                mooreNeighborhood.Add(new Point{x = target.x + 1, y = target.y - 1}); //NorthEast

            if (target.x - 1 > 0 && target.y + 1 < _size && _playground[target.x - 1, target.y + 1] != -1)
                mooreNeighborhood.Add(new Point{x = target.x - 1, y = target.y + 1}); //SouthWest

            if (target.x + 1 < _size && target.y + 1 < _size && _playground[target.x + 1, target.y + 1] != -1)
                mooreNeighborhood.Add(new Point{x = target.x + 1, y = target.y + 1}); //SouthEast

            return mooreNeighborhood.OrderByDescending(p => _playground[p.x, p.y]).FirstOrDefault();
        }

        private byte GetNextDirection(Stack<Point> path)
        {
            if (path.Count < 2) return 0;
            var currentPoint = path.Pop();
            var nextPoint = path.Peek();

            if (nextPoint.x - currentPoint.x == -1 && nextPoint.y == currentPoint.y) return 1; //up
            if (nextPoint.x - currentPoint.x == 1 && nextPoint.y == currentPoint.y) return 2; //down
            if (nextPoint.x == currentPoint.x && nextPoint.y - currentPoint.y == 1) return 3; //right
            if (nextPoint.x == currentPoint.x && nextPoint.y - currentPoint.y == -1) return 4; //left

            return 0;
        }

        private Point SafeDequeue()
        {
            return _pointBuffer.Count > 0 ? _pointBuffer.Dequeue() : new Point{x = -1, y = -1};
        }

        public void MakeWaves(Point start)
        {
            if (start.x < 0 || start.x > _size || start.y < 0|| start.y > _size) return;

            _pointBuffer.Enqueue(start);

            Point point;
            while ((point = SafeDequeue()).x != -1 && point.y != -1)
            {
                if (point.y - 1 >= 0 && _playground[point.x, point.y - 1] == 0) //up
                {
                    var tmp = new Point {x = point.x, y = point.y - 1};
                    if (tmp != start)
                    {
                        _playground[point.x, point.y - 1] = _playground[point.x, point.y] + 1;
                        _pointBuffer.Enqueue(tmp);
                    }
                }

                if (point.y + 1 < _size && _playground[point.x, point.y + 1] == 0) //down
                {
                    var tmp = new Point {x = point.x, y = point.y + 1};
                    if (tmp != start)
                    {
                        _playground[point.x, point.y + 1] = _playground[point.x, point.y] + 1;
                        _pointBuffer.Enqueue(tmp);
                    }
                }

                if (point.x + 1 < _size && _playground[point.x + 1, point.y] == 0) //right
                {
                    var tmp = new Point {x = point.x + 1, y = point.y};
                    if (tmp != start)
                    {
                        _playground[point.x + 1, point.y] = _playground[point.x, point.y] + 1;
                        _pointBuffer.Enqueue(tmp);
                    }
                }

                if (point.x - 1 >= 0 && _playground[point.x - 1, point.y] == 0) //left
                {
                    var tmp = new Point {x = point.x - 1, y = point.y};
                    if (tmp != start)
                    {
                        _playground[point.x - 1, point.y] = _playground[point.x, point.y] + 1;
                        _pointBuffer.Enqueue(tmp);
                    }
                }

            }
        }

        private void ShowPlayground()
        {
            for (var i = 0; i < _size; i++)
                for (var j = 0; j < _size; j++)
                    Console.Write($" {_playground[i, j]} ");
                Console.WriteLine();
            Console.WriteLine();
        }
    }
}