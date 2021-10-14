using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boids : MonoBehaviour
{
    public Sprite dead;
    public Vector3 Velocity;

    public List<boids> Neighbors = new List<boids>(); //liste des voisins du boid
    private List<boids> boidlist = new List<boids>(); //liste de tous les boids de la scène

    private parametersBoids param;
    private GameObject humans;
    public Vector3 mousePos;

    //paramètres
    public float maxVelocity;
    public float distanceVoisin;
    public float distanceRepousse;
    public float velociteVersVoisins;
    public float velociteRapprocher;
    public float distanceMain;

    //state machine
    public int state; // 1 = search, 2 = chase hand, 3 = flee, 4 = chase human , 5 = dead

    void Start()
    {
        param = GameObject.Find("Main Camera").GetComponent<parametersBoids>();
        humans = GameObject.Find("humans");
        distanceVoisin = param.distanceVoisin;
        maxVelocity = param.maxVelocity;
        distanceRepousse = param.distanceRepousse;
        velociteRapprocher = param.velociteRapprocher;
        velociteVersVoisins = param.velociteVersVoisins;
        mousePos = Input.mousePosition;
        distanceMain = param.distanceMain;

        GameObject[] listboidobject = GameObject.FindGameObjectsWithTag("boid");

        foreach (var b in listboidobject)
		{
            boidlist.Add(b.GetComponent<boids>());
        }

        state = 1;  //default state : searching
    }

    float distance(boids boid) //distance par rapport à un autre boid
    {
        float distX = transform.position.x - boid.transform.position.x;
        float distY = transform.position.y - boid.transform.position.y;
        return Mathf.Sqrt((distX * distX) + (distY * distY));
    }

    Vector3 moveCloser() //se rapprocher
    {
        if (Neighbors.Count < 1) { return moveRandom(); }
        //distance moyenne ds autres boids
        Vector3 avg = new Vector3(0,0,0);
		foreach (var neighbor in Neighbors)
		{
            avg = avg + neighbor.transform.position;
        }
        avg /= Neighbors.Count;

        return (avg - transform.position) / velociteRapprocher;
    }

    Vector3 moveAway() //s'écarter
    {
        if (Neighbors.Count < 1) { return moveRandom(); }

        Vector3 avg = new Vector3(0,0,0);
		foreach (var neighbor in Neighbors)
		{
            if (Mathf.Abs((neighbor.transform.position - transform.position).magnitude) < distanceRepousse)
            {
                avg = avg - (neighbor.transform.position - transform.position);
            }
        }
        return avg;
    }

    Vector3 moveWith() // bouger en bande
    {
        if (Neighbors.Count < 1) { return moveRandom(); }

        Vector3 avg = new Vector3(0, 0, 0);
        foreach (var neighbor in Neighbors)
        {
            avg = avg + neighbor.Velocity;
        }
        avg /= Neighbors.Count;
        return (avg-Velocity)/velociteVersVoisins;
    }

    bool isSeen() //la souris est-elle vue ?
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        float distX = transform.position.x - mousePos.x;
        float distY = transform.position.y - mousePos.y;
        if (Mathf.Sqrt((distX * distX) + (distY * distY)) < distanceMain)
        {
            return true;
        }
        return false;
        
    }
    Vector3 moveToward() //bouger vers la souris
    {
        return (mousePos - transform.position) * 4 / velociteVersVoisins; //la main compte autant que 4 zombies
    }

    bool humanIsSeen() //human en vuuuue (de loin)
    {
		for (int i = 0; i < humans.transform.childCount; i++)
		{
            if (humans.transform.GetChild(i).name == "humans") continue;
            if (humans.transform.GetChild(i).GetType() == GetType()) continue;
            //Debug.Log("aah " + humans.transform.GetChild(i).name);
            Vector3 dist = transform.position - humans.transform.GetChild(i).transform.position;
            dist.z = 0;
            if (dist.magnitude < distanceMain)
            {
                if (dist.magnitude < 1f)
                {
                    humans.transform.GetChild(i).GetComponent<human>().toZombie();
                    humans.transform.GetChild(i).transform.parent = transform.parent;
                    return false;
                }
                return true;
            }
		}
        return false;

    }
    public void isDead()
    {
        Debug.Log("headshot " + name + " !");
        GetComponent<SpriteRenderer>().sprite = dead;
        tag = "Untagged";
        state = 5;
        GetComponent<boids>().enabled = false;
        this.transform.parent = GameObject.Find("dead").transform;
    }
    Vector3 moveToHuman() //attaquer les humains
    {
        //distance moyenne ds autres boids
        Vector3 avg = new Vector3(0, 0, 0);
        int compte = 0;
        Transform[] allChildren = humans.GetComponentsInChildren<Transform>();
        foreach (var child in allChildren)
        {
            float distX = transform.position.x - child.transform.position.x;
            float distY = transform.position.y - child.transform.position.y;
            if (Mathf.Sqrt((distX * distX) + (distY * distY)) < distanceMain)
            {
                avg = avg + child.transform.position;
                compte++;
            }
        }
        if (compte > 0)
        {
            avg /= compte;
            return (avg - transform.position) / velociteRapprocher;
        }
        return avg;
    }

    
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, distanceRepousse);
        Gizmos.DrawWireSphere(transform.position, distanceVoisin);
        Gizmos.DrawWireSphere(transform.position, distanceMain);
        Gizmos.DrawLine(transform.position, Velocity+transform.position);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
    

    Vector3 avoidWall() //s'écarter des murs
    {
        Vector3 avg = new Vector3(0, 0, 0);
        foreach (var obst in param.obstacles)
        {
            //if (Mathf.Abs((obst.transform.position - transform.position).magnitude) < distanceRepousse)
            if(obst.GetComponent<Collider2D>().OverlapPoint(transform.position))
            {
                //Debug.Log("obstacle " + obst.name);
                avg -= (obst.transform.position - transform.position);
            }
        }

        //Debug.Log(Vector3.Distance(avg, new Vector3(0, 0, 0)));
        if (Vector3.Distance(avg, new Vector3(0,0,0)) < 0.1)
            return avg;

        return avg*20;
    }

    Vector3 moveRandom()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0) ;
    }

    void searchNeighbors()
    {
        Neighbors.Clear();
        foreach (var boid in boidlist)
        {
            if (boid.name == gameObject.name) { continue; }
            if (boid.state == 5) continue;
            //Debug.Log(boid.name);
            float dist = distance(boid);
            if (dist < distanceVoisin)
            {
                Neighbors.Add(boid);
            }
        }
    }


	private void Update()
	{
        //Debug.Log(name + " : state "+state);
        Vector3 v1=new Vector3(0,0,0), 
            v2 = new Vector3(0, 0, 0), 
            v3 = new Vector3(0, 0, 0), 
            v4= new Vector3(0, 0, 0), 
            v5 = new Vector3(0, 0, 0);

        switch (state)
        {
            case 1: //search
                if (humanIsSeen()) //state 4
                {
                    state = 4;
                    break;
                }
                if (isSeen()) //state 2
                {
                    state = 2;
                    break;
                }
                //else state 1
                searchNeighbors();
                v1 = moveCloser();
                v2 = moveWith();
                v3 = moveAway();
                v4 = moveRandom();
                v5 = avoidWall();
                break;

            case 2: //follow hand
                if (humanIsSeen()) //state 4
                {
                    state = 4;
                    break;
                }
                if (isSeen()) //state 2
                {
                    searchNeighbors();
                    v1 = moveCloser();
                    v2 = moveWith();
                    v3 = moveAway();
                    v4 = moveToward();
                    v5 = avoidWall();
                    break;
                }
                else //state 1
                {
                    state = 1;
                    break;
                }

            case 3: //flee
                break;

            case 4: //follow human
                if (humanIsSeen()) //state 4
                {

                    searchNeighbors();
                    v1 = moveCloser();
                    v2 = moveWith();
                    v3 = moveAway();
                    v4 = moveToHuman();
                    v5 = avoidWall();
                    break;
                }
                if (isSeen()) //state 2
                {
                    state = 2;
                    break;
                }
                //else state 1
                state = 1;
                break;

            case 5: //is ded, do nothin'
                break;

            default:
                state = 1;
                break;
        }

        Velocity = Velocity + v1 + v2 + v3 + v4 + v5;

        Velocity.z = 0;


        //scale the velocity
        if (Mathf.Abs(Velocity.x) > maxVelocity || Mathf.Abs(Velocity.y) > maxVelocity || Mathf.Abs(Velocity.z) > maxVelocity)
        {
            float scaleFactor = maxVelocity / Mathf.Max(Mathf.Abs(Velocity.x), Mathf.Abs(Velocity.y), Mathf.Abs(Velocity.z));
            Velocity.x *= scaleFactor;
            Velocity.y *= scaleFactor;
            Velocity.z *= scaleFactor;
        }

        //avoid borders
        int border = 2;
        int width = 52;
        int height = 30;
        if (transform.position.x < border && Velocity.x < 0)
            Velocity.x = -Velocity.x * Random.Range(0f, 1f);
        if (transform.position.x > width - border && Velocity.x > 0)
            Velocity.x = -Velocity.x * Random.Range(0f, 1f);
        if (transform.position.y < border && Velocity.y < 0)
            Velocity.y = -Velocity.y * Random.Range(0f, 1f);
        if (transform.position.y > height - border && Velocity.y > 0)
            Velocity.y = -Velocity.y * Random.Range(0f, 1f);

        Velocity = new Vector3(Velocity.x, Velocity.y, 0);

        transform.position = Vector3.Lerp(transform.position, transform.position + Velocity, 1f * Time.deltaTime);
    }
}
