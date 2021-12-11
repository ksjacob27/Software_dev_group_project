using System.Collections.Generic;



public enum NodeState {
    RUNNING,
    SUCCESS,
    FAILURE
}
///<remarks>https://medium.com/c-sharp-progarmming/making-a-rts-game-24-implementing-behaviour-trees-for-our-units-2-3-unity-c-17f14cc3c580</remarks>
public class Node {
    protected NodeState _state;
    public    NodeState State { get => _state; }

    protected Node                       child;
    protected List<Node>                 children = new List<Node>();
    private   System.Func<NodeState>     context;
    private   Dictionary<string, object> contextDict = new Dictionary<string, object>();

    public Node() {
        Parent = null;
    }
    
    public Node(System.Func<NodeState> context) {
        this.context = context;
    }
    
    public Node(System.Func<NodeState> context, Node child) {
        this.child = child;
        this.context = context;
    }

    public virtual NodeState Evaluate() {
        NodeState state = context.Invoke();
        if (child != null && state == NodeState.SUCCESS) {
            return child.Evaluate();
        }
        return state;
    }

    public void Attach(Node child) {
        children.Add(child);
        child.Parent = this;
    }

    public void Detach(Node child) {
        children.Remove(child);
        child.Parent = null;
    }

    public object GetData(string key) {
        object value = null;
        if (contextDict.TryGetValue(key, out value))
            return value;

        Node node = Parent;
        while (node != null) {
            value = node.GetData(key);
            if (value != null)
                return value;
            node = node.Parent;
        }
        return null;
    }
    

    public Node Parent { get; private set; }
    public List<Node> Children {
        get {
            return children;
        }
    }
    public bool HasChildren {
        get {
            return children.Count > 0;
        }
    }
    public virtual bool IsFlowNode {
        get {
            return false;
        }
    }
}
