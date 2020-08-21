using System;
using System.Collections.Generic;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;

namespace Code.Prediction
{
    public class GameState
    {
        public readonly int tickNumber;
        public readonly float tickMatchTimeSec;
        public Dictionary<ushort, ViewTransform> transforms = new Dictionary<ushort, ViewTransform>();

        public GameState()
        {
        }
        
        public GameState(int tickNumber)
        {
            this.tickNumber = tickNumber;
        }
        
        public GameState(int tickNumber, float tickMatchTimeSec)
        {
            this.tickMatchTimeSec = tickMatchTimeSec;
            this.tickNumber = tickNumber;
        }

        public void Copy(GameState serverState)
        {
            transforms = new Dictionary<ushort, ViewTransform>(serverState.transforms);
        }

        public void Modify(Dictionary<ushort, ViewTransform> transformsArg)
        {
            transforms = transformsArg;
        }
    }
}