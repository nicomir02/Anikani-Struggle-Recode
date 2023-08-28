using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ObjectSpawnHelper : NetworkBehaviour
{
    [Command(requiresAuthority=false)]
    public void spawnObject(GameObject o, Transform position, string objectname) {
        o = Instantiate(o, position);
        o.name = objectname;
        NetworkServer.Spawn(o);
        //RPCspawn(o, objectname);
    }

    [ClientRpc]
    void RPCspawn(GameObject o, string objectname) {
        o.name = objectname;
    }
}
