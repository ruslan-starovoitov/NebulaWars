﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Code.Common;
using Plugins.submodules.SharedCode;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.BattleScene.Udp.Connection
{
    public class UdpClientWrapper:IDisposable
    {
        private readonly UdpClient udpClient;
        private CancellationTokenSource cancellationTokenSource;
        private readonly ILog log = LogManager.CreateLogger(typeof(UdpClientWrapper));

        public UdpClientWrapper(UdpClient udpClient)
        {
            this.udpClient = udpClient;
        }
        
        public void StartReceiveThread(IByteArrayHandler byteArrayHandler)
        {
            cancellationTokenSource = new CancellationTokenSource();
            ThreadPool.QueueUserWorkItem(async o =>
            {
                await Listen(byteArrayHandler);
            });
        }
        
        private async Task Listen(IByteArrayHandler byteArrayHandler)
        {
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    UdpReceiveResult result = await udpClient.ReceiveAsync();
                    byte[] receiveBytes = result.Buffer;
                    byteArrayHandler.HandleBytes(receiveBytes);
                }
                catch (SocketException e)
                {
                    // 10004 thrown when socket is closed
                    if (e.ErrorCode != 10004)
                    {
                        log.Error("Socket exception while receiving data from udp client: " + e.FullMessage());
                    }
                }
                catch (ObjectDisposedException e)
                {
                    log.Warn("UdpClient was disposed. All ok.");
                }
                catch (Exception e)
                {
                    log.Error("Error receiving data from udp client: " + e.FullMessage());
                }
            }
        }

        public void Send(byte[] data)
        {
            try
            {
                udpClient.SendAsync(data, data.Length);
            }
            catch (Exception e)
            {
                log.Error(e.FullMessage());
            }
        }
        
        public void Stop()
        {
            cancellationTokenSource.Cancel();
            udpClient.Close();
        }

        public void Dispose()
        {
            udpClient?.Dispose();
            cancellationTokenSource?.Dispose();
        }
    }
}