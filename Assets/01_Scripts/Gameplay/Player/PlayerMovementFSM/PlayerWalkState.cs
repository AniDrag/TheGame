using Gameplay.FiniteStateMachine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gameplay.Player.PlayerMovementFSM
{
    internal class PlayerWalkState : State
    {
        public PlayerWalkState(FSM owner) : base (owner) { }
    }
}
