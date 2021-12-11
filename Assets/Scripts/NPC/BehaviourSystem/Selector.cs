using System.Collections.Generic;



///<remarks>https://medium.com/c-sharp-progarmming/making-a-rts-game-24-implementing-behaviour-trees-for-our-units-2-3-unity-c-17f14cc3c580</remarks>
public class Selector : Node {
    public Node[] nodes;
    public int    currentNode = 0;

    public Selector(Node[] nodes) {
        this.nodes = nodes;
    }
    
    public void AppendNode(Node n) {
        Node[] ns = this.nodes;
        this.nodes = new Node[this.nodes.Length + 1];
        ns.CopyTo(this.nodes, 0);
        this.nodes[this.nodes.Length - 1] = n;
    }

    public override bool IsFlowNode => true;

    public override NodeState Evaluate() {
        foreach (Node node in children) {
            switch (node.Evaluate()) {
                case NodeState.FAILURE:
                    continue;
                case NodeState.SUCCESS:
                    _state = NodeState.SUCCESS;
                    return _state;
                case NodeState.RUNNING:
                    _state = NodeState.RUNNING;
                    return _state;
                default:
                    continue;
            }
        }
        _state = NodeState.FAILURE;
        return _state;
    }
}
