using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Automaterria.Code.Definitations.Factories
{
    public enum PipeConnectorMode
    {
        None,
        Input,
        Output,
        Both,
    }
    public class PipeConnector
    {
        public Chest chest;
        public PipeConnectorMode mode;
        public Vector2Int chestPos;
        public Vector2Int pipePos;

        public bool isInput { get { return mode == PipeConnectorMode.Input || mode == PipeConnectorMode.Both; } }
        public bool isOutput { get { return mode == PipeConnectorMode.Output || mode == PipeConnectorMode.Both; } }

        public PipeConnector(Chest _chest, PipeConnectorMode _mode, Vector2Int _pipePos, Vector2Int _chestPos)
        {
            chest = _chest;
            mode = _mode;
            chestPos = _chestPos;
            pipePos = _pipePos;
        }
    }
}
