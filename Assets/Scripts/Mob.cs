using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mob : MonoBehaviour {

    #region Serialized variables
    [SerializeField]
    public int mobType = 0; //0=passive, 1=hostile, 2=neutral
    [SerializeField]
    private int currMode = 0; //0=default, 1=running, 2=slowed down

    [SerializeField]
    private float huntCooldown = 10f;
    [SerializeField]
    private float runCooldown = 10f;
    [SerializeField]
    public float sightRange = 20f;
    [SerializeField]
    public float runDistance = 25f;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private LayerMask layerMask1;

    [SerializeField]
    private float floatHeight = 3.5f;
    [SerializeField]
    private float floatForce = 65f;
    [SerializeField]
    private float maxMoveDist = 3f;
    [SerializeField]
    private float stopDistance = .5f;
    [SerializeField]
    private float moveSpeed = 1f;
    [SerializeField]
    private float rotateSpeed = 1f;
    [SerializeField]
    private float moveCancelCooldown = 10f;
    [SerializeField]
    public float viewAngle = 45f;

    [SerializeField]
    private float[] moveCooldown = new float[2] { 2f, 10f };

    [SerializeField]
    private int[] moveDistances = new int[2] { 3, 10 };

    #endregion

    #region Non-Serialized variables
    private int nextMode;
    private int rotDirection = 0;

    private float lastTriggerTime = -20f;

    private bool canChangeMode = true;
    private bool canMove = true;
    private bool hasPlayerInDirectSight = false;
    private bool moving = false;
    private bool rotating = true;
    private bool rotLook = false;
    private bool lockRot = false;

    private Vector3 LastPosSeen;
    private Vector3 targetPos;
    private Vector3 lastDir;
    private Vector3 targetRotPos;

    private GameObject huntTarget;
    private GameObject fledTarget;

    private float[][] speed = new float[][]
    {
        new float[]//passive mobs speed
        {
            1f,//default
            3f,//running
            0.7f,//slowed down
            2f,//hunt
        },
        new float[]//hostile mobs speed
        {
            1f,//default
            2f,//running
            0.7f,//slowed down
            2f,//hunt
        },
        new float[]//neutral mobs speed
        {
            1f,//default
            2f,//running
            0.7f,//slowed down
            2f,//hunt
        },
    };

    private EntitiesManager EM;
    #endregion

    #region Mobs editor HUD Variables
    public bool displayLookDirection = false;
    public bool displayFOV = false;
    public bool displayLineToPlayer = false;
    #endregion

    private void Start()
    {
        targetRotPos = transform.forward;
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        EM = GameObject.Find("MapGenV2").GetComponent<EntitiesManager>();
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        Vector3 localPlayerDirection = player.transform.position - transform.position;
        localPlayerDirection.Normalize();
        if (Physics.Raycast(transform.position, localPlayerDirection, out hit, sightRange, layerMask1))
        {
            if (hit.transform.gameObject == player.gameObject)//has player in direct sight
            {
                hasPlayerInDirectSight = true;
                if (displayLineToPlayer)
                {
                    Debug.DrawLine(transform.position, player.transform.position, Color.blue);
                }
            }
            else
            {
                hasPlayerInDirectSight = false;
            }
        }
        if (displayLookDirection)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * sightRange, Color.green);
        }
        if (displayFOV)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(VectorFromAngle(viewAngle)) * sightRange, Color.grey);
            Debug.DrawRay(transform.position, transform.TransformDirection(VectorFromAngle(-viewAngle)) * sightRange, Color.grey);
        }
    }

    private void Update()
    {
        /*if (hasPlayerInDirectSight && Vector3.Angle(new Vector3(player.transform.position.x, 0, player.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z), transform.forward) <= viewAngle)
        {
            if(mobType == 1)
            {
                if (currMode != 1)
                {
                    ChangeMode(1);
                    Hunt(player.gameObject);
                }
                lastTriggerTime = Time.time;//reset EndHunt cooldown
            }
        }*/

        #region hostileMob
        if(mobType == 1)
        {
            GameObject closestPrey = GetClosestEntity(0);
            if (closestPrey)
            {
                Hunt(closestPrey);
            }

            if (currMode == 3)
            {
                if (canMove)
                {
                    Vector3 dir = /*LastPosSeen - transform.position*/transform.forward;
                    dir.Normalize();
                    transform.position += dir * moveSpeed * speed[mobType][currMode] * Time.deltaTime;
                }

                try
                {
                    if (PlaneDistance(huntTarget.transform.position, transform.position) <= 1f)
                    {
                        Attack(huntTarget);
                    }
                }
                catch
                {
                    huntTarget = null;
                    ChangeMode(0);
                }
                
                if(lastTriggerTime + huntCooldown < Time.time)
                {
                    huntTarget = null;
                    ChangeMode(0);
                }
            }
        }
        #endregion

        #region passiveMob
        if (mobType == 0)
        {
            GameObject closestHunter = GetClosestEntity(1);
            if (closestHunter)
            {
                Run(closestHunter);
            }

            if (currMode == 1)
            {
                if (canMove)
                {
                    Vector3 dir = /*LastPosSeen - transform.position*/transform.forward;
                    dir.Normalize();
                    //dir *= -1;
                    transform.position += dir * moveSpeed * speed[mobType][currMode] * Time.deltaTime;
                }

                if (lastTriggerTime + runCooldown < Time.time)
                {
                    fledTarget = null;
                    ChangeMode(0);
                }
            }
        }
        
        #endregion

        //until flying mode isn't removed
        #region fly
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, floatHeight))
        {
            float proportionalHeight = (floatHeight - hit.distance) / floatHeight;
            Vector3 appliedFloatForce = Vector3.up * proportionalHeight * floatForce;
            GetComponent<Rigidbody>().AddForce(appliedFloatForce, ForceMode.Acceleration);
        }
        #endregion

        #region basic movements and rotations
        if (canMove && currMode == 0/* && !hasPlayerInDirectSight*/)
        {
            StartCoroutine(Move());
        }

        if(moving && currMode == 0)
        {
            Vector3 dir = /*targetPos - PlaneVector(transform.position)*/transform.forward;
            dir.Normalize();
            transform.position += dir * moveSpeed * speed[mobType][currMode] * Time.deltaTime;
        }

        /*if (rotLook)
        {
            if (Vector3.Angle(targetPos - new Vector3(transform.position.x, 0, transform.position.z), transform.forward) < 1f)
            {
                rotLook = false;
            }
            else
            {
                //Debug.Log(Vector3.Angle(targetPos - new Vector3(transform.position.x, 0, transform.position.z), transform.forward));
                transform.eulerAngles += Vector3.up * rotateSpeed * rotDirection * Time.deltaTime;
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
        }*/
        if(!lockRot)
        {
            if(currMode == 1 && fledTarget)
            {
                targetRotPos = -1 * (fledTarget.transform.position - PlaneVector(transform.position));// * -1 to look in the direction opposed to fled entity's one
            }else if (currMode == 3 && huntTarget)
            {
                targetRotPos = huntTarget.transform.position - PlaneVector(transform.position);
            }else if (currMode == 0)
            {
                targetRotPos = targetPos - PlaneVector(transform.position);
            }
            targetRotPos.y = 0;
            if(targetRotPos == Vector3.zero)
            {
                targetRotPos = transform.forward;
            }
            Quaternion targetRotation = Quaternion.LookRotation(targetRotPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
        #endregion
    }

    private GameObject GetClosestEntity(int entityType)
    {
        List<GameObject> entitiesList = new List<GameObject>();
        if(entityType == 0)//passive
        {
            entitiesList = EM.passiveMobs;
        }else if (entityType == 1)//hostile
        {
            entitiesList = EM.hostileMobs;
        }
        else if (entityType == 2)//neutral
        {
            entitiesList = EM.neutralMobs;
        }
        List<GameObject> closestEntity = new List<GameObject>();
        for (int i = 0; i < entitiesList.Count; i++)
        {
            if (PlaneDistance(entitiesList[i].transform.position, transform.position) <= runDistance)
            {
                if(entityType == 0)
                {
                    if (Vector3.Angle(PlaneVector(entitiesList[i].transform.position) - PlaneVector(transform.position), transform.forward) <= viewAngle) // if entity is within view angle
                    {
                        closestEntity.Add(entitiesList[i]);
                    }
                }
                else if(entityType == 1 || entityType == 2)
                {
                    closestEntity.Add(entitiesList[i]);
                }
            }
        }
        closestEntity.OrderBy(entity => PlaneDistance(entity.transform.position, transform.position)).ToList();//order by distance
        if (closestEntity.Count > 0)//check from the closest to the furthest hunter within mob's hear range if it isn't beind a structure (to avoid detection through walls)
        {
            float maxDist = 0;
            if(entityType == 0)
            {
                maxDist = sightRange;
            }else if(entityType == 1)
            {
                maxDist = runDistance;
            }
            for (int i = 0; i < closestEntity.Count; i++)
            {
                RaycastHit hit2;
                if (Physics.Raycast(transform.position, entitiesList[i].transform.position - transform.position, out hit2, maxDist))
                {
                    if (hit2.transform.name == entitiesList[i].transform.name)
                    {
                        return closestEntity[i];
                    }
                }
            }
        }
        return null;
    }

    private void Hunt(GameObject _huntTarget)
    {
        lastTriggerTime = Time.time;
        if(huntTarget != _huntTarget)
        {
            huntTarget = _huntTarget;
        }
        LastPosSeen = huntTarget.transform.position;
        if(currMode != 3)
        {
            ChangeMode(3);
        }
    }

    private void Run(GameObject _fledTarget)
    {
        lastTriggerTime = Time.time;
        if(fledTarget != _fledTarget)
        {
            fledTarget = _fledTarget;
        }
        LastPosSeen = fledTarget.transform.position;
        if (currMode != 1)
        {
            ChangeMode(1);
        }
    }

    private void Attack(GameObject entity)
    {
        Destroy(entity);
    }

    /*private IEnumerator MoveTowardTarget()
    {
        canMove = false;
        moving = true;
        yield return new WaitUntil(() => Vector3.Distance(PlaneVector(transform.position), LastPosSeen) <= stopDistance || currMode != 3);
        moving = false;
        canMove = true;
    }

    private IEnumerator MoveAwayFromTarget()
    {
        canMove = false;
        moving = true;
        yield return new WaitUntil(() => Vector3.Distance(PlaneVector(transform.position), LastPosSeen) >= runDistance || currMode != 1);
        moving = false;
        canMove = true;
    }*/

    private void ChangeDir ()
    {
        rotLook = true;
        if (Vector3.SignedAngle(targetPos - PlaneVector(transform.position), transform.forward, Vector3.up) < 0)
        {
            rotDirection = 1;
        }
        else if (Vector3.SignedAngle(targetPos - PlaneVector(transform.position), transform.forward, Vector3.up) > 0)
        {
            rotDirection = -1;
        }
        else
        {
            rotDirection = 0;
            rotLook = false;
        }
    }

    private IEnumerator Move()
    {
        canMove = false;

        Vector3 targetDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        float moveDist = Random.Range(moveDistances[0], moveDistances[1]);
        targetPos = new Vector3(transform.position.x + targetDir.x * moveDist, 0, transform.position.z + targetDir.z * moveDist);

        //ChangeDir();

        moving = true;
        float startMoveTime = Time.time;
        yield return new WaitUntil(() => Vector3.Distance(PlaneVector(transform.position), targetPos) <= stopDistance || currMode != 0 || startMoveTime + moveCancelCooldown < Time.time);
        moving = false;

        yield return new WaitForSeconds(Random.Range(moveCooldown[0], moveCooldown[1]));
        canMove = true;
    }

    public void ChangeMode(int mode)
    {
        if(canChangeMode)
        {
            int temp = currMode;
            currMode = mode;
            ModeChanged(temp, mode);
        }
        else
        {
            nextMode = mode;
            ChangeModeASAP();
        }
    }

    private IEnumerator ChangeModeASAP()
    {
        yield return new WaitUntil(() => canChangeMode);
        int temp = currMode;
        currMode = nextMode;
        ModeChanged(temp, nextMode);
        nextMode = -1;
    }

    private void ModeChanged(int previous, int curr)
    {
        //update values related to current mode value
        canMove = true;
    }

    private void OnDestroy()
    {
        EM.RefreshEntities();
    }

    private Vector3 VectorFromAngle(float angle)
    {
        return new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0, Mathf.Cos(Mathf.Deg2Rad * angle));
    }

    private Vector3 PlaneVector(Vector3 vec)
    {
        return new Vector3(vec.x, 0, vec.z);
    }

    private float PlaneDistance(Vector3 vec1, Vector3 vec2)
    {
        return Mathf.Sqrt(Mathf.Pow(vec2.x - vec1.x, 2) + Mathf.Pow(vec2.z - vec1.z, 2));
    }
}
