using UnityEngine;

public static class BehaviorUtils
{
    public static bool DistanceToPlayer(GameObject agent, GameObject player, float zoneDetec)
    {
        if (player == null) return false;
        if(agent == null) return false;
        float distance = Vector3.Distance(agent.transform.position, player.transform.position);
        return distance <= zoneDetec;
    }
    public static bool InRangeAttack(GameObject agent, GameObject player, float zoneDetec)
    {
        if (player == null) return false;
        if (agent == null) return false;
        float distance = Vector3.Distance(agent.transform.position, player.transform.position);
        return distance <= zoneDetec;
    }

    public static float Distance(GameObject agent, GameObject player)
    {
        return Vector3.Distance(agent.transform.position, player.transform.position);
    }
}
