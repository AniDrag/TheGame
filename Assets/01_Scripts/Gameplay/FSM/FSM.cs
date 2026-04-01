using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.FiniteStateMachine
{
    public class FSM : MonoBehaviour
    {
        [Header("Movement / Ranges")]
        protected NavMeshAgent Agent;
        protected Blackboard Blackboard;

        [Header("Animation")]
        [SerializeField]
        public Animator Animator;

        [Header("Debug / UI")]
        [SerializeField]
        protected TextMeshPro StateText;
        [Tooltip("Enable console debug logs for FSM activity")]
        public bool EnableDebug = true;
        [Tooltip("Draw gizmos for ranges and target")]
        public bool DrawGizmos = false;

        // The current state
        [SerializeReference]
        protected State _currentState;

        List<Transform> _targets = new List<Transform>();
        Transform _currentTarget = null;
        public Transform CurrentTarget => _currentTarget;


        public virtual void Start()
        {

        }

        public virtual void Update()
        {
            if (_currentState != null)
            {
                _currentState.Step();
                if (_currentState.NextState() != null)
                {
                    //Cache the next state, because after currentState.Exit, calling
                    //currentState.NextState again might return null because of change
                    //of context.
                    State nextState = _currentState.NextState();
                    _currentState.Exit();
                    _currentState = nextState;
                    _currentState.Enter();

                    if (EnableDebug)
                    {
                        DebugInfo();
                        Debug.Log($"[FSM] Transition: {_currentState.StateName} -> {nextState.StateName}");
                    }
                }
            }
        }

        void DebugInfo()
        {
            // Build diagnostics for all transitions belonging to the current state
            var sb = new StringBuilder();
            sb.AppendLine($"State: {_currentState.StateName} IsFinised: {_currentState.IsFinished}");
            //sb.AppendLine($"Target: {_currentTarget?.name ?? "none"} Dist: {DistanceToPrimary():F2}");

            // If there's a Health component, show HP for context
            //var health = GetComponent<Health>();
            //if (health != null)
            //{
            //    sb.AppendLine($"HP: {health.GetCurrentHp()}/{health.GetMaxHP()}");
            //}

            for (int i = 0; i < _currentState.Transitions.Count; i++)
            {
                var tr = _currentState.Transitions[i];
                string label = !string.IsNullOrEmpty(tr.Label) ? tr.Label : (tr._nextState != null ? tr._nextState.StateName : $"Transition{i}");
                string diag;
                bool value = false;

                // Evaluate
                try
                {
                    value = tr._condition?.Invoke() ?? false;
                    diag = value ? "TRUE" : "false";
                }
                catch (Exception ex)
                {
                    diag = $"ERROR: {ex.Message}";
                    value = false;
                }

                sb.AppendLine($" - {label}: {diag}");
            }

            // Write diagnostics to blackboard (so you can inspect it in the inspectors debug text)
            var bb = GetComponent<Blackboard>();
            if (bb != null)
            {
                bb.DebugNotes = sb.ToString();
            }

            // Optionally print to an on-screen text (assign in inspector)
            if (StateText != null)
            {
                StateText.text = sb.ToString();
            }

            // Console logging
            if (EnableDebug)
            {
                Debug.Log(sb.ToString());
            }
        }
    }
}
