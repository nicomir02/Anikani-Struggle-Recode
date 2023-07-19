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
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Biom[] biome;
    [SerializeField] private Biom[] biomInBiom;

    [SerializeField] public int width = 50;
    [SerializeField] public int height = 50;
    [SerializeField] public int maxZ = 3;

    [SerializeField] private TileBase unterErde;


    [SerializeField] private float sizeMultiplier = 2;

    //Liste mit allen Vectoren
    private readonly SyncDictionary<Vector3Int, int> allVectors = new SyncDictionary<Vector3Int, int>();


    private List<Vector3Int> tempVecList = new List<Vector3Int>();



    void Start() {
        createTerrain();
    }

    int zaehler = 0;

    void Update() {
        if(zaehler%600 == 0) createTerrain();

        zaehler++;
    }

    void createTerrain() {
        for(int x=0; x<width; x++) {
            for(int y=0; y<width; y++) {
                for(int z=0;z<=maxZ+2;z++) {
                    Vector3Int v = new Vector3Int(x, y, z);
                    tilemap.SetTile(v, null);

                    if(allVectors.ContainsKey(v)) allVectors.Remove(v);
                }
            }
        }

        for(int x=0; x<width; x++) {
            for(int y=0; y<width; y++) {
                tempVecList.Add(new Vector3Int(x, y, 0));
            }
        }
        
        while(tempVecList.Count > 0) {
            createBiom();
        }
        

    }

    void createBiom() {
        Vector3Int vec = tempVecList[rand.Next(tempVecList.Count)];
        tempVecList.Remove(vec);

        (Biom, int) biomZTupel = getNextBiom(vec);

        Biom biom = biomZTupel.Item1;
    	int z = biomZTupel.Item2;
        if(z == -1) {
            z = rand.Next(Math.Min(biom.minheight, maxZ+1), Math.Min(biom.maxheight+1, maxZ+1));
        }
        vec.z = z;
        biom.getBlock().setTile(tilemap, vec);

        List<Vector3Int> nachbarn = new List<Vector3Int>();
        

        if(allVectors.ContainsKey(vec)) {
            allVectors[vec] = biom.id;
        }else {
            allVectors.Add(vec, biom.id);
        }

        if(z > 0) {
            for(int j=0; j<z; j++) {
                vec.z = j;
                tilemap.SetTile(vec, unterErde);
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
                biom.getBlock().setTile(tilemap, vec);
                if(allVectors.ContainsKey(vec)) {
                    allVectors[vec] = biom.id;
                }else {
                    allVectors.Add(vec, biom.id);
                }

                if(z > 0) {
                    for(int j=0; j<z; j++) {
                        vec.z = j;
                        tilemap.SetTile(vec, unterErde);
                    }
                }

                i++;
            }else {
                break;
            }
        }
    }

    bool insideField(Vector3Int vec) {
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
