using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class human : MonoBehaviour
{
    public Vector3 Velocity;

    public List<human> Neighbors = new List<human>(); //liste des voisins du boid
    private List<human> boidlist = new List<human>(); //liste de tous les boids de la scène

    private parametersBoids param;
    public Vector3 mousePos;

    //paramètres
    public float maxVelocity;
    public float distanceVoisin;
    public float distanceRepousse;
    public float velociteVersVoisins;
    public float velociteRapprocher;
    public float distanceMain;

    //state machine
    public int state; // 1 = search, 2 = chase, 3 = flee

    void Start()
    {
        param = GameObject.Find("Main Camera").GetComponent<parametersBoids>();
        distanceVoisin = param.distanceVoisin;
        maxVelocity = param.maxVelocity;
        distanceRepousse = param.distanceRepousse;
        velociteRapprocher = param.velociteRapprocher;
        velociteVersVoisins = param.velociteVersVoisins;
        mousePos = Input.mousePosition;
        distanceMain = param.distanceMain;

        GameObject[] listboidobject = GameObject.FindGameObjectsWithTag("human");

        foreach (var b in listboidobject)
        {
            boidlist.Add(b.GetComponent<human>());
        }

        state = 1;  //default state : searching
    }

    float distance(human boid) //distance par rapport à un autre boid
    {
        float distX = transform.position.x - boid.transform.position.x;
        float distY = transform.position.y - boid.transform.position.y;
        return Mathf.Sqrt((distX * distX) + (distY * distY));
    }

    Vector3 moveCloser() //se rapprocher
    {
        if (Neighbors.Count < 1) { return new Vector3(0, 0, 0); }
        //distance moyenne ds autres boids
        Vector3 avg = new Vector3(0, 0, 0);
        foreach (var neighbor in Neighbors)
        {
            avg = avg + neighbor.transform.position;
        }
        avg /= Neighbors.Count;

        return (avg - transform.position) / velociteRapprocher;
    }

    Vector3 moveAway() //s'écarter
    {
        if (Neighbors.Count < 1) { return new Vector3(0, 0, 0); }

        Vector3 avg = new Vector3(0, 0, 0);
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
        if (Neighbors.Count < 1) { return new Vector3(0, 0, 0); ; }

        Vector3 avg = new Vector3(0, 0, 0);
        foreach (var neighbor in Neighbors)
        {
            avg = avg + neighbor.Velocity;
        }
        avg /= Neighbors.Count;
        return (avg - Velocity) / velociteVersVoisins;
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

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, distanceRepousse);
        Gizmos.DrawWireSphere(transform.position, distanceVoisin);
        Gizmos.DrawWireSphere(transform.position, distanceMain);
        Gizmos.DrawLine(transform.position, Velocity + transform.position);
    }

    Vector3 avoidWall() //s'écarter des murs
    {
        Vector3 avg = new Vector3(0, 0, 0);
        foreach (var obst in param.obstacles)
        {
            //if (Mathf.Abs((obst.transform.position - transform.position).magnitude) < distanceRepousse)
            if (obst.GetComponent<Collider2D>().OverlapPoint(transform.position))
            {
                //Debug.Log("obstacle " + obst.name);
                avg -= (obst.transform.position - transform.position);
            }
        }
        return avg * 20;
    }

    Vector3 moveRandom()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
    }

    void searchNeighbors()
    {
        Neighbors.Clear();
        foreach (var boid in boidlist)
        {
            if (boid.name == gameObject.name) { continue; }
            //Debug.Log(boid.name);
            float dist = distance(boid);
            if (dist < distanceVoisin)
            {
                Neighbors.Add(boid);
            }
        }
    }

    void updateboid()
    {
        searchNeighbors();

        Vector3 v1, v2, v3, v4, v5;

        v1 = moveCloser();
        v2 = moveWith();
        v3 = moveAway();
        v4 = moveRandom();
        v5 = avoidWall();
        //Debug.Log(v1 + "/with : " + v2 + "/away" + v3);

        Velocity = Velocity + v1 + v2 + v3 + v4 + v5;

    }

    void runToward()
    {
        searchNeighbors();

        Vector3 v1, v2, v3, v4, v5;

        v1 = moveCloser();
        v2 = moveWith();
        v3 = moveAway();
        v4 = moveToward();
        v5 = avoidWall();
        //Debug.Log(v1 + "/with : " + v2 + "/away" + v3);

        Velocity = Velocity + v1 + v2 + v3 + v4 + v5;
    }


    private void Update()
    {
        updateboid();

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
