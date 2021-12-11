using System.Collections.Generic;



///<remarks>https://medium.com/c-sharp-progarmming/making-a-rts-game-24-implementing-behaviour-trees-for-our-units-2-3-unity-c-17f14cc3c580</remarks>
public class Inverter : Node {
    public Inverter() : base() {}
    public Inverter(List<Node> children) {}

    public override bool IsFlowNode => true;

    public override NodeState Evaluate() {
        if (!HasChildren) return NodeState.FAILURE;
        switch (children[0].Evaluate()) {
            case NodeState.FAILURE:
                _state = NodeState.SUCCESS;
                return _state;
            case NodeState.SUCCESS:
                _state = NodeState.FAILURE;
                return _state;
            case NodeState.RUNNING:
                _state = NodeState.RUNNING;
                return _state;
            default:
                _state = NodeState.FAILURE;
                return _state;
        }
    }

}
