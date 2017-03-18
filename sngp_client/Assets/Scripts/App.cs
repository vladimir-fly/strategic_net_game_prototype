using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

using Debug = UnityEngine.Debug;

namespace SNGPClient
{
    public class App : MonoBehaviour
    {
        private const int port = 8000;
        public Playground Playground;

        void Start()
        {
            Playground.NodeSelected =
                nodeId =>
                {
                    if (Playground == null || Playground.SelectedUnits == null ||
                        Playground.SelectedUnits.Count == 0) return;

                    var selectedUnitsQueue = new Queue<Unit>(Playground.SelectedUnits);

                    Unit unit;
                    while (selectedUnitsQueue.Count > 0 && (unit = selectedUnitsQueue.Peek()) != null)
                    //foreach (var unit in Playground.SelectedUnits)
                    {
                        Debug.Log("UnitID = " + unit.Id + "; NodeId = " + nodeId);

                        var direction = MakeRequest(EMessageType.MoveDataRequest, new List<byte> {unit.Id, nodeId})
                            .FirstOrDefault();

                        Debug.Log("Direction is " + ((Direction) direction).ToString());


                        if (direction == 0)
                        {
                         selectedUnitsQueue.Dequeue();
                         continue;

                        }


                        unit.StartMoving((Direction) direction);

                           Thread.Sleep(1000);
                    };
                };

            Playground.InitNodes(PlaygroundSizeRequest());
            Playground.InitUnits(UnitsDataRequest());
        }

        private byte PlaygroundSizeRequest()
        {
            var result = MakeRequest(EMessageType.PlaygroundSizeRequest, new byte[0]).FirstOrDefault();
            return result;
        }

        private IEnumerable<byte> UnitsDataRequest()
        {
            var result = MakeRequest(EMessageType.UnitsDataRequest, new byte[0]);
            return result;
        }

        private byte[] MakeRequest(EMessageType type, IEnumerable<byte> data)
        {
            var result = new byte[6];
            try
            {
                using (var client = new TcpClient())
                {
                    client.Connect(IPAddress.Loopback, port);
                    var requestData = new List<byte> {(byte) type}.Concat(data).ToList();

                    using (var stream = client.GetStream())
                    {
                        stream.Write(requestData.ToArray(), 0, requestData.Count);
                        do stream.Read(result, 0, result.Length);
                        while (stream.DataAvailable);
                    }
                }
            }
            catch (Exception e) { Debug.LogError("Connection error: " + e); }
            return result;
        }
    }
}