using Godot;

public class Dialogic
{
    public Node target;

    public Dialogic(Node target)
    {
        this.target = target;
    }

    public void Start(string var)
    {
        //Insert safety checks
        if (target == null)
            return;

        target.Call("start", var);
    }
}