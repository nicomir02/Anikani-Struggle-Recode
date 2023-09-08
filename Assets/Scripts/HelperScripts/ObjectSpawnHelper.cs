using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ObjectSpawnHelper : NetworkBehaviour
{
    [SerializeField] private GameManager gameManager;

    [Command(requiresAuthority=false)]
    public void spawnBuilding(int volk, int buildingid, Transform position, string objectname) {
        
        GameObject prefab = Instantiate(gameManager.getVolkByID(volk).getBuildingByID(buildingid), position);
        NetworkServer.Spawn(prefab);
        RPCnamechange(prefab, objectname);
    }

    [ClientRpc]
    void RPCnamechange(GameObject o, string objectname) {
        o.name = objectname;
    }

    [Command(requiresAuthority=false)]
    public void spawnUnit(int volk, int unitID, Transform position, string objectname) {
        GameObject prefab = Instantiate(gameManager.getVolkByID(volk).getUnitByID(unitID), position);
        NetworkServer.Spawn(prefab);
        RPCnamechange(prefab, objectname);
    }

}
