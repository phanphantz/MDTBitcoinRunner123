using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class SpawnData
{
    public float vSpeed {get;}
    public float vDist {get;}
    public float hSpeed {get;}

    public SpawnData(float vSpeed, float vDist, float hSpeed)
    {
        this.vSpeed = vSpeed;
        this.vDist = vDist;
        this.hSpeed = hSpeed;
    }

}
