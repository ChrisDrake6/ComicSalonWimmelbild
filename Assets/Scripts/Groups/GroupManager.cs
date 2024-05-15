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
        if (chosenPartner != null)
        {
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
            currentGroup.timesAskedForDestination = 0;
            foreach (SpriteStateManager member in currentGroup.Members)
            {
                member.SwitchState(member.roamingState);
            }
        }
    }

    public void RemoveFromGroup(SpriteStateManager sprite)
    {
        GroupData currentGroup = groups.FirstOrDefault(group => group.Members.Any(member => member == sprite));
        if (currentGroup != null)
        {
            currentGroup.Members.Remove(sprite);
            if (currentGroup.Members.Count == 1)
            {
                currentGroup.Members[0].isInGroup = false;
                groups.Remove(currentGroup);
            }
        }
        sprite.isInGroup = false;
    }

    public float GetGroupIdleDeadLine(SpriteStateManager sprite, float minIdleTime, float maxIdleTime)
    {
        GroupData currentGroup = groups.FirstOrDefault(group => group.Members.Any(member => member == sprite));
        if (currentGroup == null)
        {
            sprite.isInGroup = false;
            return currentGroup.CurrentIdleTime = Time.time + Random.Range(minIdleTime, maxIdleTime);
        }
        if (Time.time >= currentGroup.CurrentIdleTime)
        {
            currentGroup.CurrentIdleTime = Time.time + Random.Range(minIdleTime, maxIdleTime);
        }

        return currentGroup.CurrentIdleTime;
    }

    public Vector3 GetCurrentGroupDestination(SpriteStateManager sprite)
    {
        GroupData currentGroup = groups.FirstOrDefault(group => group.Members.Any(member => member == sprite));
        if (currentGroup == null)
        {
            sprite.isInGroup = false;
            return Vector3.zero;
        }
        if (currentGroup.timesAskedForDestination >= currentGroup.Members.Count || currentGroup.timesAskedForDestination == 0)
        {
            currentGroup.CurrentDestination = NavigationManager.Instance.GetRandomShortDistanceDestination(sprite.transform);
            currentGroup.timesAskedForDestination = 0;
        }
        currentGroup.timesAskedForDestination++;
        return currentGroup.CurrentDestination;
    }

    public GroupData GetCurrentGroup(SpriteStateManager sprite)
    {
        GroupData currentGroup = groups.FirstOrDefault(group => group.Members.Any(member => member == sprite));
        if (currentGroup == null)
        {
            sprite.isInGroup = false;
        }
        return currentGroup;
    }
}
