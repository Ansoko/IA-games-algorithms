using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boids : MonoBehaviour
{
    public Vector2 Position;
    public Vector2 Velocity;

    private List<boids> Neighbors = new List<boids>(); //liste des voisins du boid
    private List<boids> boidlist = new List<boids>(); //liste de tous les boids de la scène

    private parametersBoids param;

    //paramètres
    public float distanceVoisin;
    public int maxVelocity;
    public float distanceRepousse;
    public float velociteRapprocher;
    public float velociteVersVoisins;
    public float velociteRepousse;

    void Start()
    {
        param = GameObject.Find("Main Camera").GetComponent<parametersBoids>();
        distanceVoisin = param.distanceVoisin;
        maxVelocity = param.maxVelocity;
        distanceRepousse = param.distanceRepousse;
        velociteRapprocher = param.velociteRapprocher;
        velociteVersVoisins = param.velociteVersVoisins;
        velociteRepousse = param.velociteRepousse;

        Position = transform.position;
        GameObject[] listboidobject = GameObject.FindGameObjectsWithTag("boid");

        foreach (var b in listboidobject)
		{
            boidlist.Add(b.GetComponent<boids>());
        }

        //StartCoroutine(updateboid());
    }

    float distance(boids boid)
    {
        float distX = transform.position.x - boid.transform.position.x;
        float distY = transform.position.y - boid.transform.position.y;
        return Mathf.Sqrt((distX * distX) + (distY * distY));
    }

    void moveCloser() //se rapprocher
    {
        if (Neighbors.Count < 1){return;}

        //distance moyenne ds autres boids
        float avgX = 0;
        float avgY = 0;
		foreach (var neighbor in Neighbors)
		{
            if (neighbor.Position.x == Position.x && neighbor.Position.y == Position.y){ continue; }
            avgX += (neighbor.Position.x - Position.x);
            avgY += (neighbor.Position.y - Position.y);
        }
        avgX /= Neighbors.Count;
        avgY /= Neighbors.Count;

        //vélocité vers les voisins
        Velocity.x -= (avgX / velociteRapprocher);
        Velocity.y -= (avgY / velociteRapprocher);
    }

    void moveWith() //bouger en bande
    {
        if (Neighbors.Count < 1) { return; }

        //vélocité moyenne ds autres boids
        float avgX = 0;
        float avgY = 0;
		foreach (var neighbor in Neighbors)
		{
            avgX += neighbor.Velocity.x;
            avgY += neighbor.Velocity.y;
        }
        avgX /= Neighbors.Count;
        avgY /= Neighbors.Count;

        //vélocité vers les voisins
        Velocity.x -= (avgX / velociteVersVoisins);
        Velocity.y -= (avgY / velociteVersVoisins);
    }

    void moveAway() // s'écarter
    {
        if (Neighbors.Count < 1) { return; }

        float distanceX = 0;
        float distanceY = 0;
        float numClose = 0;

		foreach (var neighbor in Neighbors)
		{
            
			float distance = this.distance(neighbor);
            if (distance < distanceRepousse)
            {
                numClose += 1;
                float xdiff = (Position.x - neighbor.transform.position.x);
                float ydiff = (Position.y - neighbor.transform.position.y);

                if (xdiff >= 0) { xdiff = Mathf.Sqrt(distanceRepousse) - xdiff; }
                else if (xdiff < 0) { xdiff = -Mathf.Sqrt(distanceRepousse) - xdiff; }

                if (ydiff >= 0) { ydiff = Mathf.Sqrt(distanceRepousse) - ydiff; }
                else if (ydiff < 0) { ydiff = -Mathf.Sqrt(distanceRepousse) - ydiff; }

                distanceX += xdiff;
                distanceY += ydiff;
            }
		}

        if (numClose == 0) { return; }

        Velocity.x -= distanceX / velociteRepousse;
        Velocity.y -= distanceY / velociteRepousse;
    }

    void move() //se déplacer
    {
        if (Mathf.Abs(Velocity.x) > maxVelocity || Mathf.Abs(Velocity.y) > maxVelocity)
        {
            float scaleFactor = maxVelocity / Mathf.Max(Mathf.Abs(Velocity.x), Mathf.Abs(Velocity.y));
            Velocity.x *= scaleFactor;
            Velocity.y *= scaleFactor;
        }


        float distanceX = 0;
        float distanceY = 0;

        foreach (var obst in param.obstacles)
        {

            float distance = (transform.position - obst.transform.position).magnitude;
            if (distance < distanceRepousse+transform.localScale.x+obst.transform.localScale.x)
            {
                float xdiff = (Position.x - obst.transform.position.x);
                float ydiff = (Position.y - obst.transform.position.y);

                if (xdiff >= 0) { xdiff = Mathf.Sqrt(distanceRepousse) - xdiff; }
                else if (xdiff < 0) { xdiff = -Mathf.Sqrt(distanceRepousse) - xdiff; }

                if (ydiff >= 0) { ydiff = Mathf.Sqrt(distanceRepousse) - ydiff; }
                else if (ydiff < 0) { ydiff = -Mathf.Sqrt(distanceRepousse) - ydiff; }

                distanceX += xdiff;
                distanceY += ydiff;
            }
        }

        Velocity.x -= distanceX / velociteRepousse;
        Velocity.y -= distanceY / velociteRepousse;

        Position.x += Velocity.x;
        Position.y += Velocity.y;
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, distanceRepousse);
        Gizmos.DrawWireSphere(transform.position, distanceVoisin);
    }

    //IEnumerator updateboid()
    void updateboid()
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

            moveCloser();
            moveWith();
            moveAway();

            int border = 0;
            int width = 28;
            int height = 15;
            if (Position.x < border && Velocity.x < 0)
                Velocity.x = -Velocity.x * Random.Range(0f, 1f);
            if (Position.x > width - border && Velocity.x > 0)
                Velocity.x = -Velocity.x * Random.Range(0f, 1f);
            if (Position.y < border && Velocity.y < 0)
                Velocity.y = -Velocity.y * Random.Range(0f, 1f);
            if (Position.y > height - border && Velocity.y > 0)
                Velocity.y = -Velocity.y * Random.Range(0f, 1f);

            move();
        
    }

	private void Update()
	{
        updateboid();
        //transform.position += (Vector3)Velocity*Time.deltaTime*10;
        transform.position = Vector3.MoveTowards(transform.position, Position, 50f*Time.deltaTime);
    }
}
