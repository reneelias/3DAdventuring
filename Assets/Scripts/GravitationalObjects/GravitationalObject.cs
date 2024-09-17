using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalObject : MonoBehaviour
{
    [Tooltip("Gravity")]
    [field: SerializeField] public GravityType gravityType{ get; private set; } = GravityType.SinglePoint;
    [field: SerializeField] public float gravityScale{ get; private set;} = 9.81f;
    [SerializeField] private GameObject gravityPoint;
    [Tooltip("Shape")]
    [field: SerializeField] public ObjectShape objectShape{ get; private set; } = ObjectShape.Sphere;
    [SerializeField] private Collider gravTriggerCollider;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum GravityType{
    SinglePoint,
    SurfaceNormal
}

public enum ObjectShape{
    Sphere,
    Cuboid,
    Plane
}