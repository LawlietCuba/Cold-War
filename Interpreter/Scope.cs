using System.Collections.Generic;

public class Scope
{
    public Scope Parent;

    public List<string> political_currents;

    public Scope()
    {
        political_currents = new List<string>();   
    }

    public Scope CreateChild()
    {
        Scope child = new Scope();
        child.Parent = this;
            
        return child;
    }

}