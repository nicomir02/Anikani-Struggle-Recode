using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ObjectSpawnHelper : NetworkBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TilemapHover tilemapHover;

    [Command(requiresAuthority=false)]
    public void spawnBuilding(int volk, int buildingid, Transform t, string objectname) {
        GameObject prefab = Instantiate(gameManager.getVolkByID(volk).getBuildingByID(buildingid), t);
        NetworkServer.Spawn(prefab);
        RPCnamechange(prefab, objectname, t);
    }

    [ClientRpc]
    void RPCnamechange(GameObject o, string objectname, Transform parent) {

        o.name = objectname;
        o.transform.parent = parent;
    }

    [Command(requiresAuthority=false)]
    public void spawnUnit(int volk, int unitID, Transform t, string objectname, int playerCode) {
        //Debug.Log(tilemapHover.getVec3IntFromVec3(t.position));
        t.position = tilemapHover.getVec3FromVec3Int(tilemapHover.getVec3IntFromVec3(t.position));
        GameObject prefab = Instantiate(gameManager.getVolkByID(volk).getUnitByID(unitID), t);
        //prefab.transform.GetChild(3).GetComponent<SpriteRenderer>().color = gameManager.getColor(playerCode);
        //prefab.GetComponent<SpriteMerger>().Merge();
        NetworkServer.Spawn(prefab);
        RPCnamechange(prefab, objectname, t.parent);
    }
}
