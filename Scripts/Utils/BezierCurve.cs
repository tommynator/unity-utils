using System;
using UnityEngine;

public static class BezierCurve
{
    private static float[] FactorialLookup;

    static BezierCurve()
    {
        CreateFactorialTable();
    }

    // just check if n is appropriate, then return the result
    private static float factorial(int n)
    {
        if (n < 0) { throw new Exception("n is less than 0"); }
        if (n > 32) { throw new Exception("n is greater than 32"); }

        return FactorialLookup[n]; /* returns the value n! as a SUMORealing point number */
    }

    // create lookup table for fast factorial calculation
    private static void CreateFactorialTable()
    {
        // fill untill n=32. The rest is too high to represent
        FactorialLookup = new float[33]; 
        FactorialLookup[0] = 1f;
        FactorialLookup[1] = 1f;
        FactorialLookup[2] = 2f;
        FactorialLookup[3] = 6f;
        FactorialLookup[4] = 24f;
        FactorialLookup[5] = 120f;
        FactorialLookup[6] = 720f;
        FactorialLookup[7] = 5040f;
        FactorialLookup[8] = 40320f;
        FactorialLookup[9] = 362880f;
        FactorialLookup[10] = 3628800f;
        FactorialLookup[11] = 39916800f;
        FactorialLookup[12] = 479001600f;
        FactorialLookup[13] = 6227020800f;
        FactorialLookup[14] = 87178291200f;
        FactorialLookup[15] = 1307674368000f;
        FactorialLookup[16] = 20922789888000f;
        FactorialLookup[17] = 355687428096000f;
        FactorialLookup[18] = 6402373705728000f;
        FactorialLookup[19] = 121645100408832000f;
        FactorialLookup[20] = 2432902008176640000f;
        FactorialLookup[21] = 51090942171709440000f;
        FactorialLookup[22] = 1124000727777607680000f;
        FactorialLookup[23] = 25852016738884976640000f;
        FactorialLookup[24] = 620448401733239439360000f;
        FactorialLookup[25] = 15511210043330985984000000f;
        FactorialLookup[26] = 403291461126605635584000000f;
        FactorialLookup[27] = 10888869450418352160768000000f;
        FactorialLookup[28] = 304888344611713860501504000000f;
        FactorialLookup[29] = 8841761993739701954543616000000f;
        FactorialLookup[30] = 265252859812191058636308480000000f;
        FactorialLookup[31] = 8222838654177922817725562880000000f;
        FactorialLookup[32] = 263130836933693530167218012160000000f;
    }

    private static float Ni(int n, int i)
    {
        float ni;
        float a1 = factorial(n);
        float a2 = factorial(i);
        float a3 = factorial(n - i);
        ni =  a1 / (a2 * a3);
        return ni;
    }

    // Calculate Bernstein basis
    private static float Bernstein(int n, int i, float t)
    {
        float basis;
        float ti; /* t^i */
        float tni; /* (1 - t)^i */

        /* Prevent problems with pow */

        if (t == 0f && i == 0) 
            ti = 1f; 
        else 
            ti = Mathf.Pow(t, i);

        if (n == i && t == 1f) 
            tni = 1f; 
        else 
            tni = Mathf.Pow((1 - t), (n - i));

        //Bernstein basis
        basis = Ni(n, i) * ti * tni; 
        return basis;
    }

    public static Vector3[] Bezier3D(Vector3[] b)
    {
        int cpts = (b.Length + 2) / 2; 
        Vector3[] p = new Vector3[cpts * 2];
       
        return Bezier3D(b, cpts, p);
    }

    public static Vector3[] Bezier3D(Vector3[] b, int cpts, Vector3[] p)
    {
        if(p.Length % 2 != 0)
            throw new System.ArgumentException();

        if(cpts * 2 > p.Length)
            throw new System.ArgumentException();

        int npts = b.Length / 2;
        int icount, jcount;
        float step, t;

        // Calculate points on curve

        icount = 0;
        t = 0;
        step = 1f / (cpts - 1);

        for (int i1 = 0; i1 != cpts; i1++)
        { 
            if ((1f - t) < 5e-6) 
                t = 1f;

            jcount = 0;
            p[icount] = Vector3.zero;
            p[icount + 1] = Vector3.zero;
            for (int i = 0; i != npts; i++)
            {
                float basis = Bernstein(npts - 1, i, t);
                p[icount] += basis * b[jcount];
                p[icount + 1] += basis * b[jcount + 1];
                jcount = jcount + 2;
            }

            icount += 2;
            t += step;
        }

        return p;
    }
}
