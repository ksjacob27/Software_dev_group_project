using UnityEngine;
using System.Collections.Generic;



    ///<remarks>https://medium.com/c-sharp-progarmming/making-a-rts-game-24-implementing-behaviour-trees-for-our-units-2-3-unity-c-17f14cc3c580</remarks>
    public class Timer : Node {
        private float _delay;
        private float _time;

        public delegate void   TickEnded();
        public event TickEnded onTickEnded;

        public Timer(float delay, TickEnded onTickEnded = null) : base() {
            _delay = delay;
            _time = _delay;
            this.onTickEnded = onTickEnded;
        }
        public Timer(float delay, List<Node> children, TickEnded onTickEnded = null)
            {
            _delay = delay;
            _time = _delay;
            this.onTickEnded = onTickEnded;
        }

        public override bool IsFlowNode => true;

        public override NodeState Evaluate() {
            if (!HasChildren) return NodeState.FAILURE;
            if (_time <= 0) {
                _time = _delay;
                _state = children[0].Evaluate();
                if (onTickEnded != null)
                    onTickEnded();
                _state = NodeState.SUCCESS;
            }
            else {
                _time -= Time.deltaTime;
                _state = NodeState.RUNNING;
            }
            return _state;
        }
    }

