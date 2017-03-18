namespace SNGPClient
{
    internal enum EMessageType
    {
        PlaygroundSizeRequest = 0,
        UnitsDataRequest = 1,
        MoveDataRequest = 2,
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