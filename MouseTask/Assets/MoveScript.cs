using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public struct Dot {
    public double x;
    public double y;

    public Dot(double xx, double yy)
    {
        x = xx;
        y = yy;
    }

    public Dot(Dot d)
    {
        x = d.x;
        y = d.y;
    }

    public static Dot operator +(Dot a, Dot b)
    {
        Dot res = new Dot(a);
        res.x += b.x;
        res.y += b.y;
        return res;
    }
}

public struct Circle {
    public Dot ctr;
    public double r;

    public double dist(Dot d)
    {
        double x = d.x - ctr.x;
        double y = d.y - ctr.y;
        double res = r - Math.Sqrt(x * x + y * y);
        Debug.Log(res.ToString());
        if(res < 0)
        {
            res = 0;
        }
        return res / r;
    }
}

public struct Square {
    public Dot lc;
    public Dot rc;
}

public class MoveScript : MonoBehaviour
{

    bool IsSegm(Dot d1, Dot d2, Dot pt)
    {
        bool flag = true;
        if(d1.x > d2.x && (pt.x > d1.x || pt.x < d2.x))
        {
            flag = false;
        }
        if (d1.y > d2.y && (pt.y > d1.y || pt.y < d2.y))
        {
            flag = false;
        }
        if (d1.x <= d2.x && (pt.x > d2.x || pt.x < d1.x))
        {
            flag = false;
        }
        if (d1.y <= d2.y && (pt.y > d2.y || pt.y < d1.y))
        {
            flag = false;
        }
        return flag;
    }

    bool IsIntersect(Dot d1, Dot d2, Dot d3, Dot d4)
    {
        Dot iDot;
        if (d1.x == d2.x)
        {
            iDot.x = d1.x;
            iDot.y = d3.y + (d4.y - d3.y) / (d4.x - d3.x) * (iDot.x - d3.x);
        }
        else
        {
            iDot.y = d1.y;
            iDot.x = d3.x + (d4.x - d3.x) / (d4.y - d3.y) * (iDot.y - d3.y);
        }
        return IsSegm(d1, d2, iDot) && IsSegm(d3, d4, iDot);
    }

    public Dot speed;
    public int speedVal;
    public int distVal;

    public Dot pos;
    public Dot dir;

    public bool swX = false;
    public bool swY = false;

    void CheckCollide(Square sq, Dot st, Dot fin)
    {
        Dot tmp;
        if (st.x > fin.x)
        {
            tmp = fin;
            fin = st;
            st = fin;
        }
        if ((IsIntersect(sq.lc, new Dot(sq.lc.x, sq.rc.y), st, fin) || IsIntersect(new Dot(sq.rc.x, sq.lc.y), sq.rc, st, fin)) && !swX)
        {
            swX = true;
            speed.x = -speed.x;
        }
        if ((IsIntersect(sq.lc, new Dot(sq.rc.x, sq.lc.y), st, fin) || IsIntersect(new Dot(sq.lc.x, sq.rc.y), sq.rc, st, fin)) && !swY)
        {
            swY = true;
            speed.y = -speed.y;
        }
    }

    List<Circle> areas;
    List<Square> barriers;
    List<List<int>> neuroConn;

    public int steps = 1000;
    public int noiseNeurons = 1000;

    System.Random rdGen = new System.Random();

    void Move()
    {
        Dot newPos = pos + speed;
        swX = false;
        swY = false;

        foreach (Square barrier in barriers) {
            CheckCollide(barrier, pos, newPos);
        }
        pos = newPos;
    }

    void MoveSegment()
    {
        if(dir.x < 0 && dir.y < 0)
        {
            dir.x = rdGen.NextDouble() * 2 * distVal - distVal;
            dir.y = Math.Sqrt(distVal * distVal - dir.x * dir.x);
            if(rdGen.Next(2) > 0)
            {
                dir.y = -dir.y;
            }
            speed.x = (speedVal / distVal) * dir.x;
            speed.y = (speedVal / distVal) * dir.y;
        }
        Move();
    }



    public void Simulate()
    {
        //Preprocess all Unity data
        GameObject[] GOs = GameObject.FindGameObjectsWithTag("Barrier");
        barriers = new List<Square>();

        foreach (GameObject curGO in GOs)
        {
            Square curBar;
            curBar.lc.x = curGO.transform.position.x - curGO.transform.localScale.x * 5;
            curBar.lc.y = curGO.transform.position.z - curGO.transform.localScale.z * 5;
            curBar.rc.x = curGO.transform.position.x + curGO.transform.localScale.x * 5;
            curBar.rc.y = curGO.transform.position.z + curGO.transform.localScale.z * 5;
            barriers.Add(curBar);
            
        }

        GOs = GameObject.FindGameObjectsWithTag("Area");
        areas = new List<Circle>();
        neuroConn = new List<List<int>>();

        foreach (GameObject curGO in GOs)
        {
            Circle curArea;
            curArea.ctr.x = curGO.transform.position.x;
            curArea.ctr.y = curGO.transform.position.z;
            curArea.r = curGO.transform.localScale.x / 2;
            areas.Add(curArea);
            neuroConn.Add(new List<int>());
            neuroConn[neuroConn.Count - 1].Add(neuroConn.Count - 1);
        }

        GameObject mouseGO = GameObject.FindWithTag("Mouse");
        pos.x = mouseGO.transform.position.x;
        pos.y = mouseGO.transform.position.z;


        //Starting Simulating process
        speed.x = 10;
        speed.y = 0;

        StreamWriter file = new StreamWriter("data.csv");
        file.Write("Step");
        for(int i = 0; i < neuroConn.Count; ++i)
        {
                file.Write(",Neuron " + (i + 1).ToString());
        }
        for (int i = 0; i < noiseNeurons; ++i)
        {
            file.Write(",Noise neuron " + (i + 1).ToString());
        }
        file.Write("\n");
        for (int i = 0; i < steps; ++i)
        {
            MoveSegment();
            file.Write((i + 1).ToString());

            foreach (List<int> listConn in neuroConn)
            {
                double curVal = 0;
                foreach (int curArea in listConn)
                {
                    curVal = Math.Max(curVal, areas[curArea].dist(pos));
                }
                file.Write("," + curVal.ToString().Replace(',', '.'));
            }

            for(int j = 0; j < noiseNeurons; ++j)
            {
                file.Write("," + rdGen.NextDouble().ToString().Replace(',', '.'));
            }

            file.Write("\n");
        }
        file.Close();
    }


    // Start is called before the first frame update
    void Start()
    {
        dir.x = 0;
        dir.y = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChSt(String s)
    {
        steps = Convert.ToInt32(s);
    }

    public void ChNN(String s)
    {
        noiseNeurons = Convert.ToInt32(s);
    }
}
