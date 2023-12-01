using UnityEngine;

public class Tama {
    public float Mass;
    public float Scale;
    public Vector3 Position;
    public Vector3 Velocity;
    public GameObject TamaGameObject;
    
    public Tama(float mass, float scale, Vector3 position, Vector3 velocity, GameObject sphere) {
        Mass = mass;
        Scale = scale;
        Position = position;
        Velocity = velocity;
        TamaGameObject = Object.Instantiate(sphere, Position, Quaternion.identity);
        TamaGameObject.tag = "Tama";
        TamaEmitter.SetWorldScale(sphere.transform, new Vector3(scale, scale, scale));
    }
}

public interface IForce {
    public abstract Vector3 GetForce(Tama p);
}

public class ConstantForce : IForce {
    private Vector3 _force;

    public ConstantForce(Vector3 force) {
        _force = force;
    }

    public Vector3 GetForce(Tama p) {
        return  _force;
    }

    public void SetForce(Vector3 force) {
        _force = force;
    }
}


// TODO: Implement viscous drag force (f = -k_d * v)
// Refer to ConstantForce above as an example
public class ViscousDragForce : IForce {

    private float _drag;
    
    public ViscousDragForce(float k_d) {
        _drag = k_d;
    }

    // return the drag force given a sphere with velocity v
    public Vector3 GetForce(Tama p) {
        Vector3 force = -_drag * p.Velocity; 

        return force;
    }

    // update the drag force such that subsequent GetForce invocations respect the new coefficient
    public void SetDragCoefficient(float k_d) {
        _drag = k_d;
    }
}
