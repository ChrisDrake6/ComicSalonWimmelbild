using System.Collections.Generic;
using UnityEngine;

public class GroupData
{
    public float CurrentIdleTime { get; set; }
    public Vector3 CurrentDestination { get; set; }
    public int timesAskedForDestination { get; set; }
    public List<SpriteStateManager> Members { get; set; }

    public GroupData(params SpriteStateManager[] members)
    {
        Members = new List<SpriteStateManager>();
        Members.AddRange(members);
    }
}
