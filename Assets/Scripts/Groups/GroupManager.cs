using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupManager : MonoBehaviour
{
    public static GroupManager Instance { get; private set; }
    List<GroupData> groups = new List<GroupData>();

    public GroupManager()
    {
        Instance = this;
    }

    public void FormGroup(SpriteStateManager sprite)
    {
        SpriteStateManager chosenPartner = LinkLine.Instance.SelectedSprite;
        GroupData currentGroup = groups.FirstOrDefault(group => group.Members.Any(member => member == chosenPartner));
        if (currentGroup == null)
        {
            chosenPartner.isInGroup = true;
            currentGroup = new GroupData(chosenPartner, sprite);
            groups.Add(currentGroup);
        }
        else
        {
            currentGroup.Members.Add(sprite);
        }
        sprite.isInGroup = true;
        foreach(SpriteStateManager member in currentGroup.Members)
        {
            member.SwitchState(member.roamingState);
        }
    }

    public void RemoveFromGroup(SpriteStateManager sprite)
    {
        GroupData currentGroup = groups.FirstOrDefault(group => group.Members.Any(member => member == sprite));
        currentGroup.Members.Remove(sprite);
        if (currentGroup.Members.Count == 1)
        {
            currentGroup.Members[0].isInGroup = false;
            groups.Remove(currentGroup);
        }
        sprite.isInGroup = false;
    }

    public float GetGroupIdleDeadLine(SpriteStateManager sprite, float minIdleTime, float maxIdleTime)
    {
        GroupData currentgroup = groups.First(group => group.Members.Any(member => member == sprite));

        if (Time.time >= currentgroup.CurrentIdleTime)
        {
            currentgroup.CurrentIdleTime = Time.time + Random.Range(minIdleTime, maxIdleTime);
        }
        
        return currentgroup.CurrentIdleTime;
    }

    public Vector3 GetCurrentGroupDestination(SpriteStateManager sprite, float roamingRadius)
    {
        GroupData currentgroup = groups.First(group => group.Members.Any(member => member == sprite));

        currentgroup.timesAskedForDestination++;
        if (currentgroup.timesAskedForDestination > currentgroup.Members.Count)
        {
            currentgroup.CurrentDestination = Random.insideUnitSphere * roamingRadius;
        }
        return currentgroup.CurrentDestination;
    }

    public GroupData GetCurrentGroup(SpriteStateManager sprite)
    {
        return groups.First(group => group.Members.Any(member => member == sprite));
    }
}
