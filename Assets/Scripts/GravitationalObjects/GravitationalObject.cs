using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalObject : MonoBehaviour
{
    [Header("Gravity")]
    [SerializeField] private GameObject gravityPoint;
    [field: SerializeField] public GravityType GravityType{ get; private set; } = GravityType.SinglePoint;
    [field: SerializeField] public float GravityScale{ get; private set;} = 9.81f;
    public Transform GravityPointTransform => gravityPoint.transform;
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

    public Vector3 GetGravity(GameObject otherObj){
        switch(GravityType){
            case GravityType.SinglePoint:
                return (gravityPoint.transform.position - otherObj.transform.position) * GravityScale;
            case GravityType.SurfaceNormal:
            default:
                return Vector3.down * GravityScale;
        }
    }
}

public enum GravityType{
    SinglePoint,
    SurfaceNormal,
    Generic
}

public enum ObjectShape{
    Sphere,
    Cuboid,
    Plane
}