using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalObject : MonoBehaviour
{
    [Header("Gravity")]
    [SerializeField] private GameObject gravityPoint;
    [field: SerializeField] public GravityType GravityType{ get; private set; } = GravityType.SinglePoint;
    [field: SerializeField] public float GravityScale{ get; private set;} = 9.81f;
    [Header("Shape")]
    [SerializeField] private Collider gravTriggerCollider;
    [field: SerializeField] public ObjectShape ObjectShape{ get; private set; } = ObjectShape.Sphere;

    
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