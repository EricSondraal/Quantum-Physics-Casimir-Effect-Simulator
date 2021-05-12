using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrControl : MonoBehaviour
{
    public GameObject quantumParticleGO;
    public GameObject solidParticleGO;


    private int maxSize=600;
    private int particleCount;

    private int bestCount;

    private const int xLength = 60;
    private const int yLength = 60;
    private const int zLength = 60;
    private const float RNGChange = 0.65f;


    private GameObject[,,] solidGrid;
    private bool[,,] triedToCreate;
    private Vector3Int[,,] touchingOffsets;
    private List<GameObject> solidParticleList;


    private const int maxTime = 100;
    private int time;
    private int particleLiveTime = 60;


    //for recording the best structure
    private bool[,,] bestStructure;
    private float bestStructureScore;
    private bool showBest;

    private bool virtualStop;

    // Start is called before the first frame update
    void Start()
    {
        virtualStop = false;
        showBest = false;
        bestStructureScore = 0f;
        bestCount = 0;
        bestStructure = new bool[xLength, yLength, zLength];
        Create();
    }


    void FixedUpdate()
    {
        for(int i=0;i<5;i++)
            TryToSpawnRandomQuantumParticle();



        time++;
        if(time== maxTime)
        {
            EndSimulation();
        } else if(time == maxTime+1)
        {
            Create();
        }
    }



    private void EndSimulation()
    {
        if (!showBest)
        {
            float thisScore = calulateScore();

            if (thisScore > bestStructureScore)
            {
                for (int x = 0; x < xLength; x++)
                {
                    for (int y = 0; y < yLength; y++)
                    {
                        for (int z = 0; z < zLength; z++)
                        {
                            bestStructure[x, y, z] = solidGrid[x, y, z] != null;
                        }
                    }
                }

                bestStructureScore = thisScore;
                bestCount++;
                print("New Best! Number " + bestCount);
            }
        }


        DestroyAllParticles();


    }


    private void DestroyAllParticles()
    {
        GameObject[] deleteParticles = GameObject.FindGameObjectsWithTag("particle");
        for (int i = 0; i < deleteParticles.Length; i++)
            Destroy(deleteParticles[i]);
    }

    private float calulateScore()
    {
        float score = 0f;
        float count = 0f;
        foreach(GameObject currentParticle in solidParticleList)
        {
            score+=Mathf.Sqrt(currentParticle.GetComponent<Rigidbody>().velocity.magnitude);
            count += 1f;
        }

        return score;
        //return score / count;
    }



    private void TryToSpawnRandomQuantumParticle()
    {
        if (!virtualStop)
        {
            if (time < maxTime - 3)
            {
                const float xScale = 1.6329931618554520654648560498039f;
                const float yScale = 1.333333333333333333333333333333f;
                const float zScale = 1.4142135623730950488016887242097f;

                float spawnX = ((Random.value * 0.5f + 0.25f) * xLength) * xScale;
                float spawnY = ((Random.value * 0.5f + 0.25f) * yLength) * yScale;
                float spawnZ = ((Random.value * 0.5f + 0.25f) * zLength) * zScale;
                Vector3 attemptedSpawnPoint = new Vector3(spawnX, spawnY, spawnZ);

                bool isSpotFree = true;
                foreach (GameObject currentParticle in solidParticleList)
                {
                    float distanceBetween = (currentParticle.GetComponent<Rigidbody>().position - attemptedSpawnPoint).magnitude;
                    if (distanceBetween < xScale)
                    {
                        isSpotFree = false;
                        break;
                    }
                }

                if (isSpotFree)
                {
                    int maxTimeAlive = maxTime - time - 1;
                    int wantedTimeAlive = particleLiveTime;
                    int timeAlive = maxTimeAlive < wantedTimeAlive ? maxTimeAlive : wantedTimeAlive;

                    GameObject newQuantumParticle = Instantiate(quantumParticleGO, attemptedSpawnPoint, Quaternion.identity);
                    newQuantumParticle.GetComponent<scrQuantumParticle>().QuickStart(timeAlive);
                }
            }
        }
       
        
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            showBest = !showBest;
            print(showBest?"Showing Best":"Running Simulation");
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            virtualStop = !virtualStop;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void QuitGame()
    {
        // save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }


    private void Create()
    {
        particleCount = 0;
        solidParticleList = new List<GameObject>();
        time = 0;

        solidGrid = new GameObject[xLength,yLength,zLength];
        triedToCreate = new bool[xLength, yLength, zLength];

        //clear all data
        for (int x = 0; x < xLength; x++)
            for (int y = 0; y < yLength; y++)
                for (int z = 0; z < zLength; z++)
                {
                    solidGrid[x, y, z] = null;
                    triedToCreate[x, y, z] = false;
                }


        touchingOffsets = new Vector3Int[2, 2, 12];
        //x: any y:odd z:odd
        touchingOffsets[1, 1 ,0] = new Vector3Int(1, 0, -1);
        touchingOffsets[1, 1, 1] = new Vector3Int(1, -1, 1);
        touchingOffsets[1, 1, 2] = new Vector3Int(1, -1, 0);
        touchingOffsets[1, 1, 3] = new Vector3Int(-1, 0, 0);
        touchingOffsets[1, 1, 4] = new Vector3Int(0, -1, 0);
        touchingOffsets[1, 1, 5] = new Vector3Int(0, 0, -1);
        touchingOffsets[1, 1, 6] = new Vector3Int(0, 0, 1);
        touchingOffsets[1, 1, 7] = new Vector3Int(0, 1, 0);
        touchingOffsets[1, 1, 8] = new Vector3Int(1, 0, 0);
        touchingOffsets[1, 1, 9] = new Vector3Int(1, 0, 1);
        touchingOffsets[1, 1, 10] = new Vector3Int(1, 1, 0);
        touchingOffsets[1, 1, 11] = new Vector3Int(1, 1, 1);

        //x: any y:even z:odd
        touchingOffsets[0,1,0] = new Vector3Int(1, 0, -1);
        touchingOffsets[0, 1, 1] = new Vector3Int(0, -1, 0);
        touchingOffsets[0, 1, 2] = new Vector3Int(0, -1, -1);
        touchingOffsets[0, 1, 3] = new Vector3Int(-1, 0, 0);
        touchingOffsets[0, 1, 4] = new Vector3Int(-1, -1, 0);
        touchingOffsets[0, 1, 5] = new Vector3Int(0, 0, -1);
        touchingOffsets[0, 1, 6] = new Vector3Int(0, 0, 1);
        touchingOffsets[0, 1, 7] = new Vector3Int(-1, 1, 0);
        touchingOffsets[0, 1, 8] = new Vector3Int(1, 0, 0);
        touchingOffsets[0, 1, 9] = new Vector3Int(1, 0, 1);
        touchingOffsets[0, 1, 10] = new Vector3Int(0, 1, -1);
        touchingOffsets[0, 1, 11] = new Vector3Int(0, 1, 0);

        //x: any y:odd z:even
        touchingOffsets[1,0,0] = new Vector3Int(-1, 0, -1);
        touchingOffsets[1, 0, 1] = new Vector3Int(0, -1, 1);
        touchingOffsets[1, 0, 2] = new Vector3Int(1, -1, 0);
        touchingOffsets[1, 0, 3] = new Vector3Int(-1, 0, 0);
        touchingOffsets[1, 0, 4] = new Vector3Int(0, -1, 0);
        touchingOffsets[1, 0, 5] = new Vector3Int(0, 0, -1);
        touchingOffsets[1, 0, 6] = new Vector3Int(-1, 0, 1);
        touchingOffsets[1, 0, 7] = new Vector3Int(0, 1, 0);
        touchingOffsets[1, 0, 8] = new Vector3Int(1, 0, 0);
        touchingOffsets[1, 0, 9] = new Vector3Int(0, 0, 1);
        touchingOffsets[1, 0, 10] = new Vector3Int(1, 1, 0);
        touchingOffsets[1, 0, 11] = new Vector3Int(0, 1, 1);

        //x: any y:even z:even
        touchingOffsets[0,0,0] = new Vector3Int(-1, 0, -1);
        touchingOffsets[0, 0, 1] = new Vector3Int(-1, -1, 0);
        touchingOffsets[0, 0, 2] = new Vector3Int(0, -1, 0);
        touchingOffsets[0, 0, 3] = new Vector3Int(-1, 0, 0);
        touchingOffsets[0, 0, 4] = new Vector3Int(-1, -1, -1);
        touchingOffsets[0, 0, 5] = new Vector3Int(0, 0, -1);
        touchingOffsets[0, 0, 6] = new Vector3Int(-1, 0, 1);
        touchingOffsets[0, 0, 7] = new Vector3Int(-1, 1, -1);
        touchingOffsets[0, 0, 8] = new Vector3Int(1, 0, 0);
        touchingOffsets[0, 0, 9] = new Vector3Int(0, 0, 1);
        touchingOffsets[0, 0, 10] = new Vector3Int(0, 1, 0);
        touchingOffsets[0, 0, 11] = new Vector3Int(-1, 1, 0);




        if (!showBest)
        {
            CreateSeed();
            ExpandSeed();
        }
        else
        {
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    for (int z = 0; z < zLength; z++)
                    {
                        if(bestStructure[x, y, z])
                        {
                            CreateSolidParticle(new Vector3Int(x,y,z));
                        }
                    }
                }
            }
        }


        BindParticles();
    }



    void BindParticles()
    {
        for (int x = 1; x < xLength-1; x++)
        {
            for (int y = 1; y < yLength - 1; y++)
            {
                int spotStateY = y % 2;
                for (int z = 1; z < zLength - 1; z++)
                {
                    GameObject currentObject = solidGrid[x, y, z];
                    if (currentObject != null)
                    {
                        int spotStateZ = z % 2;
                        //int count = 0;

                        for (int i = 0; i < 12; i++)
                        {
                            Vector3Int checkPosition = (new Vector3Int(x, y, z)) + touchingOffsets[spotStateY, spotStateZ, i];
                            GameObject checkObject = solidGrid[checkPosition.x, checkPosition.y, checkPosition.z];
                            if (checkObject != null)
                            {
                                currentObject.GetComponent<scrSolidParticle>().BindToParticle(checkObject);
                                //count++;
                            }
                        }


                        //if (count > 0)
                        //    print(count);
                    }
                }
            }
        }
    }




    void CreateSeed()
    {
        Vector3Int testSpot = new Vector3Int(xLength/2, yLength/2, zLength/2);
        int spotStateY = testSpot.y % 2;
        int spotStateZ = testSpot.z % 2;
        
        for (int i = 0; i < 12; i++)
            CreateSolidParticle(testSpot + touchingOffsets[spotStateY, spotStateZ, i]);
    }
    

    private void ExpandSeed()
    {
        
        bool newParticleAdded = true;
        while ((newParticleAdded) && (particleCount < maxSize))
        {
            newParticleAdded = false;

            for (int x = xLength/3; x < xLength *2/ 3; x++) 
            { 
                for (int y = yLength/3; y < yLength * 2 / 3; y++) 
                {
                    int spotStateY = y % 2;
                    for (int z = zLength/3; z < zLength * 2 / 3; z++)
                    {
                        if (!triedToCreate[x,y,z])
                        {
                            int spotStateZ = z % 2;
                            //we need atleast 3 other particles in range to have a chance at making another
                            int nextToCount = 0;
                            for(int i = 0; i < 12; i++)
                            {
                                Vector3Int checkPosition = (new Vector3Int(x, y, z)) + touchingOffsets[spotStateY, spotStateZ, i];
                                if (solidGrid[checkPosition.x, checkPosition.y, checkPosition.z] != null)
                                {
                                    nextToCount++;
                                }
                            }

                            if (nextToCount >= 3)
                            {
                                bool newParticleJustAdded = RNGCreateSolidParticle(new Vector3Int(x, y, z));
                                if (newParticleJustAdded)
                                {
                                    newParticleAdded = true;                                    
                                }
                                    

                            }

                        }
                    }
                }
            }


            


        }
    }



    private bool RNGCreateSolidParticle(Vector3Int pos)
    {
        if ((!triedToCreate[pos.x, pos.y, pos.z]) && (Random.value <= RNGChange - ((float)particleCount / (float)maxSize) / 10f )  )
        {
            particleCount++;
            CreateSolidParticle(pos);
            triedToCreate[pos.x, pos.y, pos.z] = true;
            return true;
        }
        triedToCreate[pos.x, pos.y, pos.z] = true;
        return false;
    }

    private void CreateSolidParticle(Vector3Int pos)
    {
        GameObject newSolidParticle = Instantiate(solidParticleGO, ArrayToInGamePosition(pos), Quaternion.identity);
        newSolidParticle.GetComponent<scrSolidParticle>().QuickStart();
        solidGrid[pos.x, pos.y, pos.z] = newSolidParticle;
        solidParticleList.Add(newSolidParticle);
        triedToCreate[pos.x, pos.y, pos.z] = true;
    }


    private Vector3 ArrayToInGamePosition(Vector3Int pos)
    {
        float edgeLength = 1.6329931618554520654648560498039f;

        float newX=pos.x;
        float newY= pos.y;
        float newZ= pos.z;

        newX *= edgeLength;
        newZ *= 1.4142135623730950488016887242097f;
        newY *= 1.333333333333333333333333333333f;

        if ((pos.z % 2f) == 1f)
        {
            newX += edgeLength/2;
        }

        if ((pos.y % 2f) == 1f)
        {
            newX += 0.81649658092772603273242802490196f;
            newZ += 0.47140452079103168293389624140323f;
        }



        return new Vector3(newX, newY, newZ);
    }



}
