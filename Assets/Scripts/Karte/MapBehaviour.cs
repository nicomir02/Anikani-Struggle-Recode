using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;

public class MapBehaviour : NetworkBehaviour
{
    //Für Random Zahl
    private static Random rand = new Random();

    //Einstellungen:
    [SerializeField] private GameObject prefabTilemap;
    [SerializeField] private Biom[] biome;
    [SerializeField] private Ressource[] ressourcen;
    //[SerializeField] private Biom[] biomInBiom;

    [SerializeField] public int width = 50;
    [SerializeField] public int height = 50;
    [SerializeField] public int maxZ = 3;

    [SerializeField] private TileBase unterErde;

    [SerializeField] private TilemapHover tilemapHover;
    


    [SerializeField] private float sizeMultiplier = 2;
    [SerializeField, Range(0, 10)] private float ressourceMultiplier = 2;

    [SerializeField] private GameObject loading;

    //Liste mit allen Vectoren
    private readonly SyncDictionary<Vector3Int, int> allVectors = new SyncDictionary<Vector3Int, int>();
    public readonly SyncDictionary<Vector3Int, int> allRessourcen = new SyncDictionary<Vector3Int, int>();


    private List<Vector3Int> tempVecList = new List<Vector3Int>();

    /*
    void createTerrain(); -> Erstellt ein neues Terrain
    void createBiom(); -> Erstellt Biom, nur in createTerrain() benutzen
    bool insideField(Vector3Int vec); -> Schaut ob Vektor im Feld ist
    */

    public int getZ(Vector3Int vec) {
        for(int z=0; z<=maxZ; z++) {
            vec.z = z;
            if(allVectors.ContainsKey(vec)) return z;
        }
        return -1;
    }

    public Biom getBiom(Vector3Int vec) {
        if(!allVectors.ContainsKey(vec)) return null;
        foreach(Biom b in biome) {
            if(b.id == allVectors[vec]) return b;
        }
        return null;
    }

    public void startGame() {
        createTerrain();
    }

    [Command(requiresAuthority=false)]
    public void createTerrain() {
        Debug.Log("[MapBehaviour] Terrain wird geprüft");
        if(allVectors.Count > 0) generateTerrain();
        Debug.Log("[MapBehaviour] Terrain wird erstellt!");
        /*for(int x=0; x<width; x++) {
            for(int y=0; y<width; y++) {
                for(int z=0;z<=maxZ+2;z++) {
                    Vector3Int v = new Vector3Int(x, y, z);
                    tilemap.SetTile(v, null);

                    if(allVectors.ContainsKey(v)) allVectors.Remove(v);
                }
            }
        }*/

        for(int x=0; x<width; x++) {
            for(int y=0; y<width; y++) {
                tempVecList.Add(new Vector3Int(x, y, 0));
            }
        }
        float test = maxZ*0.4f;
        for(int z=maxZ; z>=0; z--) {
            GameObject instance = Instantiate(prefabTilemap, new Vector3(0, 0, 0), Quaternion.identity);
            instance.name = "Height" + z;
            Vector3 setvec = instance.transform.position;
            setvec.z = z;
            setvec.y = test;
            instance.transform.position = setvec;
            NetworkServer.Spawn(instance);
            test -= 0.4f;
        }
        
        while(tempVecList.Count > 0) {
            createBiom();
        }

        foreach(KeyValuePair<Vector3Int, int>kvp in allVectors) {
            if(rand.Next(10) <= ressourceMultiplier) {
                float ressFloat = 0;
                foreach(Ressource ress in ressourcen) {
                    ressFloat += ress.getProbality(getBiom(kvp.Key));
                }
                float value = rand.NextFloat(0, ressFloat);
                ressFloat = 0;
                int i=0;
                foreach(Ressource ress in ressourcen) {
                    float before = ressFloat;
                    ressFloat += ress.getProbality(getBiom(kvp.Key));
                    if(value > before && value <=ressFloat) allRessourcen.Add(kvp.Key, i);
                    i++;
                }
            }
        }
        Debug.Log("[MapBehaviour] Terrain wurde erstellt!");

        generateTerrain();
        
    }

    [Command(requiresAuthority=false)]
    void createBiom() {
        Vector3Int vec = tempVecList[rand.Next(tempVecList.Count)];
        tempVecList.Remove(vec);

        (Biom, int) biomZTupel = getNextBiom(vec);

        Biom biom = biomZTupel.Item1;
    	int z = biomZTupel.Item2;
        if(z == -1) {
            z = rand.Next(Math.Min(biom.minheight, maxZ+1), Math.Min(biom.maxheight+1, maxZ+1));
        }
        
        vec.z = 0;
        //biom.getBlock().setTile(tilemap, vec);

        List<Vector3Int> nachbarn = new List<Vector3Int>();
        

        if(allVectors.ContainsKey(vec)) {
            allVectors[vec] = biom.id;
        }else {
            allVectors.Add(vec, biom.id);
        }

        if(z > 0) {
            for(int j=0; j<z; j++) {
                vec.z = j;
                //tilemap.SetTile(vec, unterErde);
            }
        }

        int i = 0;
        while(i<biom.size*sizeMultiplier) {
            Vector3Int[] nachbarliste = getNachbarn(vec);
            for(int j=0; j<4; j++) {
                if(insideField(nachbarliste[j]) && tempVecList.Contains(nachbarliste[j])) nachbarn.Add(nachbarliste[j]);
            }

            if(nachbarn.Count > 0) {
                vec = nachbarn[rand.Next(0, nachbarn.Count)];
                nachbarn.Remove(vec);
                tempVecList.Remove(vec);
                vec.z = z;
                //biom.getBlock().setTile(tilemap, vec);
                if(allVectors.ContainsKey(vec)) {
                    allVectors[vec] = biom.id;
                }else {
                    allVectors.Add(vec, biom.id);
                }

                if(z > 0) {
                    for(int j=0; j<z; j++) {
                        vec.z = j;
                        //tilemap.SetTile(vec, unterErde);
                    }
                }

                i++;
            }else {
                break;
            }
        }
    }

    [ClientRpc]
    void generateTerrain() {
        Debug.Log("[MapBehaviour] Terrain wird generiert");
        foreach(KeyValuePair<Vector3Int, int> kvp in allVectors) {
            Biom b = null;
            Vector3Int vec = kvp.Key;
            foreach(Biom biom in biome) {
                if(biom.id == kvp.Value) {
                    b = biom;
                    break;
                }
            }
            int z = vec.z;
            Tilemap tilemap = GameObject.Find("Height" + vec.z).transform.GetChild(0).GetComponent<Tilemap>();
            vec.z = 0;
            b.getBlock().setTile(tilemap, vec);
            if(z > 0) {
                for(int j=0; j<z; j++) {
                    tilemap = GameObject.Find("Height" + j).transform.GetChild(0).GetComponent<Tilemap>();
                    tilemap.SetTile(vec, unterErde);
                }
            }
        }

        foreach(KeyValuePair<Vector3Int, int> kvp in allRessourcen) {
            Tilemap tilemap = GameObject.Find("Height" + kvp.Key.z).transform.GetChild(0).GetComponent<Tilemap>();
            ressourcen[kvp.Value].setTile(tilemap, new Vector3Int(kvp.Key.x, kvp.Key.y, 0));
        }
        Debug.Log("[MapBehaviour] Finished");
        loading.SetActive(false);
        tilemapHover.afterGeneration();
    }

    public bool insideField(Vector3Int vec) {
        return vec.x >= 0 && vec.x <= width && vec.y >= 0 && vec.y <= height;
    }

    Vector3Int[] getNachbarn(Vector3Int vec) {
        Vector3Int[] nachbarliste = {vec+new Vector3Int(0, 1, 0), vec+new Vector3Int(0, -1, 0), 
                                    vec+new Vector3Int(1, 0, 0), vec+new Vector3Int(-1, 0, 0)};
        for(int i=0; i<nachbarliste.Length; i++) {
            nachbarliste[i].z = 0;
        }
        return nachbarliste;
    }

    (Biom, int) getNextBiom(Vector3Int vec) {
        Dictionary<int, int> isBiomNear = new Dictionary<int, int>();
        Dictionary<int, int> zCounter = new Dictionary<int, int>();
        foreach(Vector3Int v in getNachbarn(vec)) {
            for(int z = 0; z<maxZ+1; z++) {
                Vector3Int ve = new Vector3Int(v.x, v.y, z);
                if(allVectors.ContainsKey(ve)) {
                    if(zCounter.ContainsKey(ve.z)) {
                        zCounter[ve.z]+=1;
                    }else {
                        zCounter.Add(ve.z, 1);
                    }
                    if(isBiomNear.ContainsKey(allVectors[ve])) {
                        isBiomNear[allVectors[ve]] += 1; 
                        continue;
                    }

                    isBiomNear.Add(allVectors[ve], 1);
                }
            }
        }
        if(isBiomNear.Count > 0) {
            int cur = 0;
            int maxvalue = 0;

            foreach(KeyValuePair<int, int> kvp in isBiomNear) {
                if(kvp.Value > maxvalue) cur = kvp.Key;
            }

            int curZ = 0;
            int maxZvalue = 0;
            foreach(KeyValuePair<int, int> kvp in isBiomNear) {
                if(kvp.Value > maxZvalue) curZ = kvp.Key;
            }

            foreach(Biom b in biome) {
                if(b.id == cur) return (b, curZ);
            }
        }

        float max = 0;
        for(int i=0; i<biome.Length; i++) {
            max += biome[i].probability;
        }
        float wert = rand.NextFloat(0, max);
        max = 0;
        for(int i=0; i<biome.Length; i++) {
            max+=biome[i].probability;
            if(wert <= max) {
                return (biome[i], -1);
            }
        }

        return (biome[0], -1);
    }
}


