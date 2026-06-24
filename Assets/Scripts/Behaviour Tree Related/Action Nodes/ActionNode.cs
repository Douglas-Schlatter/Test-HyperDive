using UnityEngine;

/// <summary>
/// Leaf node of the tree, in essence, doesn't have any children to execute/delegate
/// <br/>
/// Since this systens takes into account sequence of behaviours through interconection
///  <br/>
/// in the out of the nodes, most of the action nodes  that usally are here (like wait and debug)
///  <br/>
/// where moved to decorator section
/// </summary>
public abstract class ActionNode: Node
{
    
}
