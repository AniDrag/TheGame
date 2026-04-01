using Gameplay.FiniteStateMachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Player.PlayerMovementFSM
{
    public class PlayerFSM : FSM
    {
        public PlayerInput Input;

        State _jumpState;
        State _dashState;
        State _phazeState;
        State _runState;
        State _walkState;
        State _idleState;

        public override void Start()
        {
            Blackboard = GetComponent<PlayerBlackboard>();
            if (Blackboard == null)
            {
                Blackboard = gameObject.AddComponent<PlayerBlackboard>();
                if (EnableDebug) Debug.Log("[FSM] No Blackboard found on boss; added default BossBlackboard.");
            }
            _dashState = new PlayerDashState(this);
            _jumpState = new PlayerJumpState(this);
            _phazeState = new PlayerPhazeState(this);
            _runState = new PlayerRunState(this);
            _walkState = new PlayerWalkState(this);

            
            // TODO: Add phasing
            Vector2 moving = Input.actions["Move"].ReadValue<Vector2>();
            Vector2 look = Input.actions["Look"].ReadValue<Vector2>();
            bool pressedAttack = Input.actions["Attack"].WasPressedThisFrame();
            bool heldAttack = Input.actions["Attack"].IsPressed();
            bool pressedSprint = Input.actions["Sprint"].WasPressedThisFrame();
            bool heldSprint = Input.actions["Sprint"].IsPressed();
            bool pressedCrouch = Input.actions["Crocuh"].WasPressedThisFrame();
            bool heldCrouch = Input.actions["Crocuh"].IsPressed();
            bool pressedJump = Input.actions["Jump"].WasPressedThisFrame();
            bool heldJump = Input.actions["Jump"].IsPressed();



            _idleState.Transitions.Add(new Transition(() => moving.magnitude > 0, _walkState, "idle_to_walking"));
            _idleState.Transitions.Add(new Transition(() => heldSprint, _dashState, "idle_to_dash"));
            _idleState.Transitions.Add(new Transition(() => heldJump, _jumpState, "idle_to_jump"));
            //_idleState.Transitions.Add(new Transition(() => pressedPhazing, _phazeState, "idle_to_phazing"));



            base.Start();
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
        }
    }
}