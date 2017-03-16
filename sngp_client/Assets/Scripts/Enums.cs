using System;

namespace SNGPClient
{
    internal enum EMessageType
    {
        PlaygroundSizeRequest = 0, //1 byte
        UnitsDataRequest = 1, // >= 5 bytes
        MoveDataRequest = 2, // 2 bytes
    }

    internal enum Direction : byte
    {
        Up = 0,
        Down = 1,
        Right = 2,
        Left = 3
    }
}