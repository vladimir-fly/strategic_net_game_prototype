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
        None = 0,
        Up = 1,
        Down = 2,
        Right = 3,
        Left = 4
    }
}