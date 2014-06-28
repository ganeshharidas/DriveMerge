using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace DriveMerge
{
    class CWebServer
    {
        TcpListener Socket;
        volatile bool bActive;

        public void Start (int iPort)
        {
            Socket = new TcpListener (IPAddress.Any, iPort);
            Socket.Start ();
            bActive = true;
            Thread ListenThread = new Thread (this.Listen);
            ListenThread.Start ();
        }

        public void Stop ()
        {
            //
            // GHTODO: Do a get to the web server and shut it down.
            //
        }
        private void Listen ()
        {
            while (bActive)
            {
                TcpClient Req = Socket.AcceptTcpClient ();
                //
                // Thread HandleThread = new Thread (new ParameterizedThreadStart (HandleReq));
                // Using the parameterized delegate says there is no acceptable overload
                // for HandleReq. Using the lambda construct instead.
                //
                Thread HandleThread = new Thread (() => HandleReq (Req));
                HandleThread.Start ();
            }
        }

        private void HandleReq (TcpClient Req)
        {
            byte[] Buffer = new byte [1024];
            Req.GetStream ().Read (Buffer, 0, 1024);
            string Data = Convert.ToString (Buffer);
            System.Console.WriteLine (Data);
        }


    }
}
