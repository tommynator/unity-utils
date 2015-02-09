using UnityEngine;
using System.Collections.Generic;

namespace Utils
{
    public class Voronoi
    {
        Vector3[]   places;
        List<Edge>      edges;
        float           width, height;
        VParabola       root;
        float           ly;
        HashSet<VEvent> deleted = new HashSet<VEvent>();
        List<Vector3>   points = new List<Vector3>();
        PriorityQueue <float, VEvent>  queue = new PriorityQueue<float, VEvent>();

        //std::priority_queue<VEvent , std::vector<VEvent >, VEvent::CompareEvent> queue;
        //Dictionary<VEvent, List<VEvent>, VEvent::CompareEvent> queue;

        public List<Edge> GetEdges(Vector3[] v, int w, int h)
        {
            places = v;
            width = w;
            height = h;
            root = null;
    
            if (edges == null)
            {
                edges = new List<Edge>();
            } else
            {
                points.Clear();
                edges.Clear();
            }
    
            foreach (Vector3 vert in places)
            {
                queue.Enqueue(vert.y, new VEvent(vert, true));
            }
    
            VEvent e;
            while (queue.Count > 0)
            {
                e = queue.Dequeue().Value;
                ly = e.point.y;

                if (deleted.Contains(e))
                {
                    deleted.Remove(e);
                    continue;
                }

                if (e.pe)
                    AddParabola(e.point);
                else
                    RemoveParabola(e);
            }
    
            FinishEdge(root);
    
            foreach (var edge in edges)
            {
                if (edge.neighbour != null)
                {
                    edge.start = edge.neighbour.end;
                    edge.neighbour = null;
                    //delete (i).neighbour;
                }
            }
    
            return edges;
        }

        void    AddParabola(Vector3  p)
        {
            if (root == null)
            {
                root = new VParabola(p);
                return;
            }
    
            if (root.isLeaf && root.site.y - p.y < 1) // degenerovan˝ p¯Ìpad - obÏ spodnÌ mÌsta ve stejnÈ v˝öce
            {
                Vector3 fp = root.site;
                root.isLeaf = false;
                root.SetLeft(new VParabola(fp));
                root.SetRight(new VParabola(p));
                Vector3 s = new Vector3((p.x + fp.x) / 2, height); // zaË·tek hrany uprost¯ed mÌst
                points.Add(s);
                if (p.x > fp.x)
                    root.edge = new Edge(s, fp, p); // rozhodnu, kter˝ vlevo, kter˝ vpravo
        else
                    root.edge = new Edge(s, p, fp);
                edges.Add(root.edge);
                return;
            }
    
            VParabola par = GetParabolaByX(p.x);
    
            if (par.cEvent != null)
            {
                deleted.Add(par.cEvent);
                par.cEvent = null;
            }
    
            Vector3 start = new Vector3(p.x, GetY(par.site, p.x));
            points.Add(start);
    
            Edge el = new Edge(start, par.site, p);
            Edge er = new Edge(start, p, par.site);
    
            el.neighbour = er;
            edges.Add(el);
    
            // p¯estavuju strom .. vkl·d·m novou parabolu
            par.edge = er;
            par.isLeaf = false;
    
            VParabola p0 = new VParabola(par.site);
            VParabola p1 = new VParabola(p);
            VParabola p2 = new VParabola(par.site);
    
            par.SetRight(p2);
            par.SetLeft(new VParabola());
            par.Left().edge = el;
    
            par.Left().SetLeft(p0);
            par.Left().SetRight(p1);
    
            CheckCircle(p0);
            CheckCircle(p2);
        }

        void    RemoveParabola(VEvent  e)
        {
            VParabola p1 = e.arch;
    
            VParabola xl = VParabola.GetLeftParent(p1);
            VParabola xr = VParabola.GetRightParent(p1);
    
            VParabola p0 = VParabola.GetLeftChild(xl);
            VParabola p2 = VParabola.GetRightChild(xr);
    
            if (p0 == p2)
                Debug.LogWarning("chyba - prav· a lev· parabola m· stejnÈ ohnisko!");
    
            if (p0.cEvent != null)
            {
                deleted.Add(p0.cEvent);
                p0.cEvent = null;
            }
            if (p2.cEvent != null)
            {
                deleted.Add(p2.cEvent);
                p2.cEvent = null;
            }
    
            Vector3 p = new Vector3(e.point.x, GetY(p1.site, e.point.x));
            points.Add(p);
    
            xl.edge.end = p;
            xr.edge.end = p;
    
            VParabola higher = null;
            VParabola par = p1;
            while (par != root)
            {
                par = par.parent;
                if (par == xl)
                    higher = xl;
                if (par == xr)
                    higher = xr;
            }
            higher.edge = new Edge(p, p0.site, p2.site);
            edges.Add(higher.edge);
    
            VParabola gparent = p1.parent.parent;
            if (p1.parent.Left() == p1)
            {
                if (gparent.Left() == p1.parent)
                    gparent.SetLeft(p1.parent.Right());
                if (gparent.Right() == p1.parent)
                    gparent.SetRight(p1.parent.Right());
            } else
            {
                if (gparent.Left() == p1.parent)
                    gparent.SetLeft(p1.parent.Left());
                if (gparent.Right() == p1.parent)
                    gparent.SetRight(p1.parent.Left());
            }
    
            p1.parent = null;
            //delete p1;
    
            CheckCircle(p0);
            CheckCircle(p2);
        }

        void FinishEdge(VParabola  n)
        {
            if (n.isLeaf)
            {
                // delete n; 
                return;
            }

            float mx;
            if (n.edge.direction.x > 0.0f)
                mx = Mathf.Max(width, n.edge.start.x + 10);
            else
                mx = Mathf.Min(0.0f, n.edge.start.x - 10);
    
            Vector3 end = new Vector3(mx, mx * n.edge.f + n.edge.g); 
            n.edge.end = end;
            points.Add(end);
    
            FinishEdge(n.Left());
            FinishEdge(n.Right());
            //delete n;
        }

        float  GetXOfEdge(VParabola  par, float y)
        {
            VParabola left = VParabola.GetLeftChild(par);
            VParabola right = VParabola.GetRightChild(par);
    
            Vector3 p = left.site;
            Vector3 r = right.site;
    
            float dp = 2.0f * (p.y - y);
            float a1 = 1.0f / dp;
            float b1 = -2.0f * p.x / dp;
            float c1 = y + dp / 4 + p.x * p.x / dp;
    
            dp = 2.0f * (r.y - y);
            float a2 = 1.0f / dp;
            float b2 = -2.0f * r.x / dp;
            float c2 = ly + dp / 4 + r.x * r.x / dp;
    
            float a = a1 - a2;
            float b = b1 - b2;
            float c = c1 - c2;
    
            float disc = b * b - 4 * a * c;
            float x1 = (-b + Mathf.Sqrt(disc)) / (2 * a);
            float x2 = (-b - Mathf.Sqrt(disc)) / (2 * a);
    
            float ry;
            if (p.y < r.y)
                ry = Mathf.Max(x1, x2);
            else
                ry = Mathf.Min(x1, x2);
    
            return ry;
        }

        VParabola GetParabolaByX(float xx)
        {
            VParabola par = root;
            float x = 0.0f;
    
            while (!par.isLeaf) // projdu stromem dokud nenarazÌm na vhodn˝ list
            {
                x = GetXOfEdge(par, ly);
                if (x > xx)
                    par = par.Left();
                else
                    par = par.Right();                
            }
            return par;
        }

        float GetY(Vector3  p, float x) // ohnisko, x-sou¯adnice
        {
            float dp = 2 * (p.y - ly);
            float a1 = 1 / dp;
            float b1 = -2 * p.x / dp;
            float c1 = ly + dp / 4 + p.x * p.x / dp;
    
            return(a1 * x * x + b1 * x + c1);
        }

        void CheckCircle(VParabola  b)
        {
            VParabola lp = VParabola.GetLeftParent(b);
            VParabola rp = VParabola.GetRightParent(b);
    
            VParabola a = VParabola.GetLeftChild(lp);
            VParabola c = VParabola.GetRightChild(rp);
    
            if (a == null || c == null || a.site == c.site)
                return;
    
            Vector3 s;
            if (!GetEdgeIntersection(lp.edge, rp.edge, out s))
                return;
    
            float dx = a.site.x - s.x;
            float dy = a.site.y - s.y;
    
            float d = Mathf.Sqrt((dx * dx) + (dy * dy));
    
            if (s.y - d >= ly)
            {
                return;
            }
    
            VEvent e = new VEvent(new Vector3(s.x, s.y - d), false, b);
            points.Add(e.point);
            b.cEvent = e;

            queue.Enqueue(e.point.y, e);
        }

        bool GetEdgeIntersection(Edge a, Edge b, out Vector3 intersection)
        {
            intersection = Vector3.zero;
            float x = (b.g - a.g) / (a.f - b.f);
            float y = a.f * x + a.g;
    
            if ((x - a.start.x) / a.direction.x < 0)
                return false;
            if ((y - a.start.y) / a.direction.y < 0)
                return false;
    
            if ((x - b.start.x) / b.direction.x < 0)
                return false;
            if ((y - b.start.y) / b.direction.y < 0)
                return false; 
    
            intersection = new Vector3(x, y);      
            points.Add(intersection);

            return true;
        }
    }

    public class Edge
    {
        public Vector3 start;
        public Vector3 end;
        public Vector3 direction;
        public Vector3 left;
        public Vector3 right;
        public float   f;
        public float   g;
        public Edge    neighbour;

        public Edge(Vector3 s, Vector3 a, Vector3 b)
        {
            start = s;
            left = a;
            right = b;
            neighbour = null;
            end = Vector3.zero;

            f = (b.x - a.x) / (a.y - b.y);
            g = s.y - f * s.x;
            direction = new Vector3(b.y - a.y, -(b.x - a.x));
        }
    }

    public class VParabola
    {
        public bool      isLeaf;
        public Vector3   site;
        public Edge      edge;
        public VEvent    cEvent;
        public VParabola parent;

        public void SetLeft(VParabola  p)
        {
            _left = p;
            p.parent = this;
        }

        public void SetRight(VParabola  p)
        {
            _right = p;
            p.parent = this;
        }
    
        public VParabola Left()
        {
            return _left;
        }

        public VParabola Right()
        {
            return _right;
        }
    
        VParabola _left;
        VParabola _right;

        public VParabola()
        {
            site = Vector3.zero;
            isLeaf = false;
        }
    
        public VParabola(Vector3 s)
        {
            site = s; 
            isLeaf = true;
        }

        public static VParabola GetLeft(VParabola  p)
        {
            return GetLeftChild(GetLeftParent(p));
        }
    
        public static VParabola GetRight(VParabola  p)
        {
            return GetRightChild(GetRightParent(p));
        }
    
        public static VParabola GetLeftParent(VParabola  p)
        {
            VParabola par = p.parent;
            VParabola pLast = p;
            while (par.Left() == pLast)
            { 
                if (par.parent == null)
                    return null;

                pLast = par; 
                par = par.parent; 
            }
            return par;
        }
    
        public static VParabola GetRightParent(VParabola  p)
        {
            VParabola par = p.parent;
            VParabola pLast = p;
            while (par.Right() == pLast)
            { 
                if (par.parent == null)
                    return null;

                pLast = par;
                par = par.parent; 
            }
            return par;
        }
    
        public static VParabola GetLeftChild(VParabola  p)
        {
            if (p == null)
                return null;
            VParabola par = p.Left();
            while (!par.isLeaf)
                par = par.Right();
            return par;
        }
    
        public static VParabola GetRightChild(VParabola  p)
        {
            if (p == null)
                return null;
            VParabola par = p.Right();
            while (!par.isLeaf)
                par = par.Left();
            return par;
        }
    }

    public class VEvent
    {
        public  Vector3   point;
        public  bool      pe;
        public  VParabola arch;
   
        public VEvent(Vector3 p, bool pev, VParabola parabola = null)
        {
            point = p;
            pe = pev;
            arch = parabola;
        }
    }
}