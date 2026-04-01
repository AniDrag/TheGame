using Gameplay.FiniteStateMachine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gameplay.Player.PlayerMovementFSM
{
    internal class PlayerIdleState : State
    {
        public PlayerIdleState(FSM owner) : base(owner) { }
    }
}
