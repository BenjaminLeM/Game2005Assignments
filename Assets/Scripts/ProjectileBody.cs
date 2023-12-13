using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using System.Numerics;
using UnityEngine;


public class ProjectileBody : MonoBehaviour
{
    // Homework: Incorporate launch position, launch speed, and launch angle into our new custom-physics system!
    // Plot 3D projectile motion by specifying a pitch (launch-angle about X) and yaw (launch-angle about Y)
    public Vector3 launchPosition;
    public float dt;
    public Vector3 grav = new Vector3(0, -9.8f, 0);
    public List<Body> bodies = new List<Body>();
    void checkForNewBodies()
    {
        Body[] allObjects = FindObjectsOfType<Body>(false);
        foreach (Body BodyFound in allObjects)
        {
            if (!bodies.Contains(BodyFound))
            {
                bodies.Add(BodyFound);
            }
        }
    }
    bool checkSphereSphereCollision(Body bodyA, Body bodyB)
    {
        Vector3 displacement = bodyA.transform.position - bodyB.transform.position;
        float distance = displacement.magnitude;
        return distance < bodyA.radius;
    }
    bool checkSpherePlaneCollision(Body bodyA, Body bodyB)
    {
        Vector3 normal = bodyB.transform.rotation * new Vector3(0, 1, 0);
        Vector3 displacement = bodyA.transform.position - bodyB.transform.position;
        float projection = Vector3.Dot(displacement, normal);
        return Mathf.Abs(projection) < bodyA.radius;
    }
    bool checkSphereHalfPlaneCollision(Body bodyA, Body bodyB)
    { 
        Vector3 normal = bodyB.transform.rotation * new Vector3(0,1,0);
        Vector3 displacement = bodyA.transform.position - bodyB.transform.position;
        float projection = Vector3.Dot(displacement, normal);
        return projection < bodyA.radius;
    }

    bool checkSphereAABBCollision(Body bodyA, Body bodyB) 
    {
        //gets all of the aabb's face positions
        Vector3 aabbFrontFacePos = bodyB.transform.position + bodyB.transform.forward * bodyB.transform.lossyScale.x * 0.5f;
        Vector3 aabbBackFacePos = bodyB.transform.position - bodyB.transform.forward * bodyB.transform.lossyScale.x * 0.5f;
        Vector3 aabbRightFacePos = bodyB.transform.position + bodyB.transform.right * bodyB.transform.lossyScale.z * 0.5f;
        Vector3 aabbLeftFacePos = bodyB.transform.position - bodyB.transform.right * bodyB.transform.lossyScale.z * 0.5f;
        Vector3 aabbTopFacePos = bodyB.transform.position + bodyB.transform.up * bodyB.transform.lossyScale.y * 0.5f;
        Vector3 aabbBottomFacePos = bodyB.transform.position - bodyB.transform.up * bodyB.transform.lossyScale.y * 0.5f;


        float aabbFrontFacePosProjection = Vector3.Dot(bodyA.transform.position - aabbFrontFacePos, bodyB.transform.forward);
        float aabbBackFacePosProjection = Vector3.Dot(bodyA.transform.position - aabbBackFacePos, -bodyB.transform.forward);
        float aabbRightFacePosProjection = Vector3.Dot(bodyA.transform.position - aabbRightFacePos, bodyB.transform.right);
        float aabbLeftFacePosProjection = Vector3.Dot(bodyA.transform.position - aabbLeftFacePos, -bodyB.transform.right);
        float aabbTopFacePosProjection = Vector3.Dot(bodyA.transform.position - aabbTopFacePos, bodyB.transform.up);
        float aabbBottomFacePosProjection = Vector3.Dot(bodyA.transform.position - aabbBottomFacePos, -bodyB.transform.up);


        if (aabbFrontFacePosProjection < bodyA.radius) 
        {
            if (aabbBackFacePosProjection < bodyA.radius) 
            {
                if (aabbRightFacePosProjection < bodyA.radius) 
                {
                    if (aabbLeftFacePosProjection < bodyA.radius)
                    {
                        if (aabbTopFacePosProjection < bodyA.radius)
                        {
                            if (aabbBottomFacePosProjection < bodyA.radius)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    bool checkAABBAABBCollision(Body bodyA, Body bodyB)
    {
        //get all first aabb's direction vectors for the vector positions
        Vector3 firstFowardVec = bodyA.transform.forward * bodyA.transform.lossyScale.x * 0.5f;
        Vector3 firstRightVec = bodyA.transform.right * bodyA.transform.lossyScale.z * 0.5f;
        Vector3 firstTopVec = bodyA.transform.up * bodyA.transform.lossyScale.y * 0.5f;
        //get all of the first aabb's front vector postions
        Vector3 firstAABBFrontTopRightVecPos = bodyA.transform.position +
                ((firstFowardVec) + (firstRightVec) + (firstTopVec));

        Vector3 firstAABBFrontTopLeftVecPos = bodyA.transform.position +
                ((firstFowardVec) + (-firstRightVec) + (firstTopVec));

        Vector3 firstAABBFrontBottomRightVecPos = bodyA.transform.position +
                ((firstFowardVec) + (firstRightVec) + (-firstTopVec));

        Vector3 firstAABBFrontBottomLeftVecPos = bodyA.transform.position +
                ((firstFowardVec) + (-firstRightVec) + (-firstTopVec));

        //first aabb back faces
        Vector3 firstAABBBackTopRightVecPos = bodyA.transform.position +
                ((-firstFowardVec) + (firstRightVec) + (firstTopVec));

        Vector3 firstAABBBackTopLeftVecPos = bodyA.transform.position +
                ((-firstFowardVec) + (-firstRightVec) + (firstTopVec));

        Vector3 firstAABBBackBottomRightVecPos = bodyA.transform.position +
                ((-firstFowardVec) + (firstRightVec) + (-firstTopVec));

        Vector3 firstAABBBackBottomLeftVecPos = bodyA.transform.position +
                ((-firstFowardVec) + (-firstRightVec) + (-firstTopVec));


        Vector3 firstMin = firstAABBFrontTopRightVecPos;
        Vector3 firstMax = firstAABBFrontTopRightVecPos;

        for (int i = 0; i < 7; i++) 
        {
            if (i == 0) 
            {
                if (firstMin.x > firstAABBFrontTopLeftVecPos.x) 
                {
                    firstMin.x = firstAABBFrontTopLeftVecPos.x;
                }
                if (firstMin.y > firstAABBFrontTopLeftVecPos.y)
                {
                    firstMin.y = firstAABBFrontTopLeftVecPos.y;
                }
                if (firstMin.z > firstAABBFrontTopLeftVecPos.z)
                {
                    firstMin.z = firstAABBFrontTopLeftVecPos.z;
                }
            }
            if (i == 1) 
            {
                if (firstMin.x > firstAABBFrontBottomRightVecPos.x)
                {
                    firstMin.x = firstAABBFrontBottomRightVecPos.x;
                }
                if (firstMin.y > firstAABBFrontBottomRightVecPos.y)
                {
                    firstMin.y = firstAABBFrontBottomRightVecPos.y;
                }
                if (firstMin.z > firstAABBFrontBottomRightVecPos.z)
                {
                    firstMin.z = firstAABBFrontBottomRightVecPos.z;
                }
            }
            if (i == 2)
            {
                if (firstMin.x > firstAABBFrontBottomLeftVecPos.x)
                {
                    firstMin.x = firstAABBFrontBottomLeftVecPos.x;
                }
                if (firstMin.y > firstAABBFrontBottomLeftVecPos.y)
                {
                    firstMin.y = firstAABBFrontBottomLeftVecPos.y;
                }
                if (firstMin.z > firstAABBFrontBottomLeftVecPos.z)
                {
                    firstMin.z = firstAABBFrontBottomLeftVecPos.z;
                }
            }
            if (i == 3)
            {
                if (firstMin.x > firstAABBBackTopRightVecPos.x)
                {
                    firstMin.x = firstAABBBackTopRightVecPos.x;
                }
                if (firstMin.y > firstAABBBackTopRightVecPos.y)
                {
                    firstMin.y = firstAABBBackTopRightVecPos.y;
                }
                if (firstMin.z > firstAABBBackTopRightVecPos.z)
                {
                    firstMin.z = firstAABBBackTopRightVecPos.z;
                }
            }
            if (i == 4)
            {
                if (firstMin.x > firstAABBBackTopLeftVecPos.x)
                {
                    firstMin.x = firstAABBBackTopLeftVecPos.x;
                }
                if (firstMin.y > firstAABBBackTopLeftVecPos.y)
                {
                    firstMin.y = firstAABBBackTopLeftVecPos.y;
                }
                if (firstMin.z > firstAABBBackTopLeftVecPos.z)
                {
                    firstMin.z = firstAABBBackTopLeftVecPos.z;
                }
            }
            if (i == 5)
            {
                if (firstMin.x > firstAABBBackBottomRightVecPos.x)
                {
                    firstMin.x = firstAABBBackBottomRightVecPos.x;
                }
                if (firstMin.y > firstAABBBackBottomRightVecPos.y)
                {
                    firstMin.y = firstAABBBackBottomRightVecPos.y;
                }
                if (firstMin.z > firstAABBBackBottomRightVecPos.z)
                {
                    firstMin.z = firstAABBBackBottomRightVecPos.z;
                }
            }
            if (i == 6) 
            {
                if (firstMin.x > firstAABBBackBottomLeftVecPos.x)
                {
                    firstMin.x = firstAABBBackBottomLeftVecPos.x;
                }
                if (firstMin.y > firstAABBBackBottomLeftVecPos.y)
                {
                    firstMin.y = firstAABBBackBottomLeftVecPos.y;
                }
                if (firstMin.z > firstAABBBackBottomLeftVecPos.z)
                {
                    firstMin.z = firstAABBBackBottomLeftVecPos.z;
                }
            }
        }

        for (int i = 0; i < 7; i++)
        {
            if (i == 0)
            {
                if (firstMax.x < firstAABBFrontTopLeftVecPos.x)
                {
                    firstMax.x = firstAABBFrontTopLeftVecPos.x;
                }
                if (firstMax.y < firstAABBFrontTopLeftVecPos.y)
                {
                    firstMax.y = firstAABBFrontTopLeftVecPos.y;
                }
                if (firstMax.z < firstAABBFrontTopLeftVecPos.z)
                {
                    firstMax.z = firstAABBFrontTopLeftVecPos.z;
                }
            }
            if (i == 1)
            {
                if (firstMax.x < firstAABBFrontBottomRightVecPos.x)
                {
                    firstMax.x = firstAABBFrontBottomRightVecPos.x;
                }
                if (firstMax.y < firstAABBFrontBottomRightVecPos.y)
                {
                    firstMax.y = firstAABBFrontBottomRightVecPos.y;
                }
                if (firstMax.z < firstAABBFrontBottomRightVecPos.z)
                {
                    firstMax.z = firstAABBFrontBottomRightVecPos.z;
                }
            }
            if (i == 2)
            {
                if (firstMax.x < firstAABBFrontBottomLeftVecPos.x)
                {
                    firstMax.x = firstAABBFrontBottomLeftVecPos.x;
                }
                if (firstMax.y < firstAABBFrontBottomLeftVecPos.y)
                {
                    firstMax.y = firstAABBFrontBottomLeftVecPos.y;
                }
                if (firstMax.z < firstAABBFrontBottomLeftVecPos.z)
                {
                    firstMax.z = firstAABBFrontBottomLeftVecPos.z;
                }
            }
            if (i == 3)
            {
                if (firstMax.x < firstAABBBackTopRightVecPos.x)
                {
                    firstMax.x = firstAABBBackTopRightVecPos.x;
                }
                if (firstMax.y < firstAABBBackTopRightVecPos.y)
                {
                    firstMax.y = firstAABBBackTopRightVecPos.y;
                }
                if (firstMax.z < firstAABBBackTopRightVecPos.z)
                {
                    firstMax.z = firstAABBBackTopRightVecPos.z;
                }
            }
            if (i == 4)
            {
                if (firstMax.x < firstAABBBackTopLeftVecPos.x)
                {
                    firstMax.x = firstAABBBackTopLeftVecPos.x;
                }
                if (firstMax.y < firstAABBBackTopLeftVecPos.y)
                {
                    firstMax.y = firstAABBBackTopLeftVecPos.y;
                }
                if (firstMax.z < firstAABBBackTopLeftVecPos.z)
                {
                    firstMax.z = firstAABBBackTopLeftVecPos.z;
                }
            }
            if (i == 5)
            {
                if (firstMax.x < firstAABBBackBottomRightVecPos.x)
                {
                    firstMax.x = firstAABBBackBottomRightVecPos.x;
                }
                if (firstMax.y < firstAABBBackBottomRightVecPos.y)
                {
                    firstMax.y = firstAABBBackBottomRightVecPos.y;
                }
                if (firstMax.z < firstAABBBackBottomRightVecPos.z)
                {
                    firstMax.z = firstAABBBackBottomRightVecPos.z;
                }
            }
            if (i == 6)
            {
                if (firstMax.x < firstAABBBackBottomLeftVecPos.x)
                {
                    firstMax.x = firstAABBBackBottomLeftVecPos.x;
                }
                if (firstMax.y < firstAABBBackBottomLeftVecPos.y)
                {
                    firstMax.y = firstAABBBackBottomLeftVecPos.y;
                }
                if (firstMax.z < firstAABBBackBottomLeftVecPos.z)
                {
                    firstMax.z = firstAABBBackBottomLeftVecPos.z;
                }
            }
        }

        //get all second aabb's direction vectors for the vector positions
        Vector3 secondFowardVec = bodyB.transform.forward * bodyB.transform.lossyScale.x;
        Vector3 secondRightVec = bodyB.transform.right * bodyB.transform.lossyScale.z;
        Vector3 secondTopVec = bodyB.transform.up * bodyB.transform.lossyScale.y;
        //get all of the second aabb's front vector postions
        Vector3 secondAABBFrontTopRightVecPos = bodyB.transform.position +
                ((secondFowardVec) + (secondRightVec) + (secondTopVec));

        Vector3 secondAABBFrontTopLeftVecPos = bodyB.transform.position +
                ((secondFowardVec) + (-secondRightVec) + (secondTopVec));

        Vector3 secondAABBFrontBottomRightVecPos = bodyB.transform.position +
                ((secondFowardVec) + (secondRightVec) + (-secondTopVec));

        Vector3 secondAABBFrontBottomLeftVecPos = bodyB.transform.position +
                ((secondFowardVec) + (-secondRightVec) + (-secondTopVec));

        //second aabb back faces
        Vector3 secondAABBBackTopRightVecPos = bodyB.transform.position +
                ((-secondFowardVec) + (secondRightVec) + (secondTopVec));

        Vector3 secondAABBBackTopLeftVecPos = bodyB.transform.position +
                ((-secondFowardVec) + (-secondRightVec) + (secondTopVec));

        Vector3 secondAABBBackBottomRightVecPos = bodyB.transform.position +
                ((-secondFowardVec) + (secondRightVec) + (-secondTopVec));

        Vector3 secondAABBBackBottomLeftVecPos = bodyB.transform.position +
                ((-secondFowardVec) + (-secondRightVec) + (-secondTopVec));

        Vector3 secondMin = secondAABBFrontTopRightVecPos;
        Vector3 secondMax = secondAABBFrontTopRightVecPos;

        for (int i = 0; i < 7; i++)
        {
            if (i == 0)
            {
                if (secondMin.x > secondAABBFrontTopLeftVecPos.x)
                {
                    secondMin.x = secondAABBFrontTopLeftVecPos.x;
                }
                if (secondMin.y > secondAABBFrontTopLeftVecPos.y)
                {
                    secondMin.y = secondAABBFrontTopLeftVecPos.y;
                }
                if (secondMin.z > secondAABBFrontTopLeftVecPos.z)
                {
                    secondMin.z = secondAABBFrontTopLeftVecPos.z;
                }
            }
            if (i == 1)
            {
                if (secondMin.x > secondAABBFrontBottomRightVecPos.x)
                {
                    secondMin.x = secondAABBFrontBottomRightVecPos.x;
                }
                if (secondMin.y > secondAABBFrontBottomRightVecPos.y)
                {
                    secondMin.y = secondAABBFrontBottomRightVecPos.y;
                }
                if (secondMin.z > secondAABBFrontBottomRightVecPos.z)
                {
                    secondMin.z = secondAABBFrontBottomRightVecPos.z;
                }
            }
            if (i == 2)
            {
                if (secondMin.x > secondAABBFrontBottomLeftVecPos.x)
                {
                    secondMin.x = secondAABBFrontBottomLeftVecPos.x;
                }
                if (secondMin.y > secondAABBFrontBottomLeftVecPos.y)
                {
                    secondMin.y = secondAABBFrontBottomLeftVecPos.y;
                }
                if (secondMin.z > secondAABBFrontBottomLeftVecPos.z)
                {
                    secondMin.z = secondAABBFrontBottomLeftVecPos.z;
                }
            }
            if (i == 3)
            {
                if (secondMin.x > secondAABBBackTopRightVecPos.x)
                {
                    secondMin.x = secondAABBBackTopRightVecPos.x;
                }
                if (secondMin.y > secondAABBBackTopRightVecPos.y)
                {
                    secondMin.y = secondAABBBackTopRightVecPos.y;
                }
                if (secondMin.z > secondAABBBackTopRightVecPos.z)
                {
                    secondMin.z = secondAABBBackTopRightVecPos.z;
                }
            }
            if (i == 4)
            {
                if (secondMin.x > secondAABBBackTopLeftVecPos.x)
                {
                    secondMin.x = secondAABBBackTopLeftVecPos.x;
                }
                if (secondMin.y > secondAABBBackTopLeftVecPos.y)
                {
                    secondMin.y = secondAABBBackTopLeftVecPos.y;
                }
                if (secondMin.z > secondAABBBackTopLeftVecPos.z)
                {
                    secondMin.z = secondAABBBackTopLeftVecPos.z;
                }
            }
            if (i == 5)
            {
                if (secondMin.x > secondAABBBackBottomRightVecPos.x)
                {
                    secondMin.x = secondAABBBackBottomRightVecPos.x;
                }
                if (secondMin.y > secondAABBBackBottomRightVecPos.y)
                {
                    secondMin.y = secondAABBBackBottomRightVecPos.y;
                }
                if (secondMin.z > secondAABBBackBottomRightVecPos.z)
                {
                    secondMin.z = secondAABBBackBottomRightVecPos.z;
                }
            }
            if (i == 6)
            {
                if (secondMin.x > secondAABBBackBottomLeftVecPos.x)
                {
                    secondMin.x = secondAABBBackBottomLeftVecPos.x;
                }
                if (secondMin.y > secondAABBBackBottomLeftVecPos.y)
                {
                    secondMin.y = secondAABBBackBottomLeftVecPos.y;
                }
                if (secondMin.z > secondAABBBackBottomLeftVecPos.z)
                {
                    secondMin.z = secondAABBBackBottomLeftVecPos.z;
                }
            }
        }

        for (int i = 0; i < 7; i++)
        {
            if (i == 0)
            {
                if (secondMax.x < secondAABBFrontTopLeftVecPos.x)
                {
                    secondMax.x = secondAABBFrontTopLeftVecPos.x;
                }
                if (secondMax.y < secondAABBFrontTopLeftVecPos.y)
                {
                    secondMax.y = secondAABBFrontTopLeftVecPos.y;
                }
                if (secondMax.z < secondAABBFrontTopLeftVecPos.z)
                {
                    secondMax.z = secondAABBFrontTopLeftVecPos.z;
                }
            }
            if (i == 1)
            {
                if (secondMax.x < secondAABBFrontBottomRightVecPos.x)
                {
                    secondMax.x = secondAABBFrontBottomRightVecPos.x;
                }
                if (secondMax.y < secondAABBFrontBottomRightVecPos.y)
                {
                    secondMax.y = secondAABBFrontBottomRightVecPos.y;
                }
                if (secondMax.z < secondAABBFrontBottomRightVecPos.z)
                {
                    secondMax.z = secondAABBFrontBottomRightVecPos.z;
                }
            }
            if (i == 2)
            {
                if (secondMax.x < secondAABBFrontBottomLeftVecPos.x)
                {
                    secondMax.x = secondAABBFrontBottomLeftVecPos.x;
                }
                if (secondMax.y < secondAABBFrontBottomLeftVecPos.y)
                {
                    secondMax.y = secondAABBFrontBottomLeftVecPos.y;
                }
                if (secondMax.z < secondAABBFrontBottomLeftVecPos.z)
                {
                    secondMax.z = secondAABBFrontBottomLeftVecPos.z;
                }
            }
            if (i == 3)
            {
                if (secondMax.x < secondAABBBackTopRightVecPos.x)
                {
                    secondMax.x = secondAABBBackTopRightVecPos.x;
                }
                if (secondMax.y < secondAABBBackTopRightVecPos.y)
                {
                    secondMax.y = secondAABBBackTopRightVecPos.y;
                }
                if (secondMax.z < secondAABBBackTopRightVecPos.z)
                {
                    secondMax.z = secondAABBBackTopRightVecPos.z;
                }
            }
            if (i == 4)
            {
                if (secondMax.x < secondAABBBackTopLeftVecPos.x)
                {
                    secondMax.x = secondAABBBackTopLeftVecPos.x;
                }
                if (secondMax.y < secondAABBBackTopLeftVecPos.y)
                {
                    secondMax.y = secondAABBBackTopLeftVecPos.y;
                }
                if (secondMax.z < secondAABBBackTopLeftVecPos.z)
                {
                    secondMax.z = secondAABBBackTopLeftVecPos.z;
                }
            }
            if (i == 5)
            {
                if (secondMax.x < secondAABBBackBottomRightVecPos.x)
                {
                    secondMax.x = secondAABBBackBottomRightVecPos.x;
                }
                if (secondMax.y < secondAABBBackBottomRightVecPos.y)
                {
                    secondMax.y = secondAABBBackBottomRightVecPos.y;
                }
                if (secondMax.z < secondAABBBackBottomRightVecPos.z)
                {
                    secondMax.z = secondAABBBackBottomRightVecPos.z;
                }
            }
            if (i == 6)
            {
                if (secondMax.x < secondAABBBackBottomLeftVecPos.x)
                {
                    secondMax.x = secondAABBBackBottomLeftVecPos.x;
                }
                if (secondMax.y < secondAABBBackBottomLeftVecPos.y)
                {
                    secondMax.y = secondAABBBackBottomLeftVecPos.y;
                }
                if (secondMax.z < secondAABBBackBottomLeftVecPos.z)
                {
                    secondMax.z = secondAABBBackBottomLeftVecPos.z;
                }
            }
        }

        if (firstMin.x <= secondMax.x && firstMax.x >= secondMin.x) 
        {
            if (firstMin.y <= secondMax.y && firstMax.y >= secondMin.y)
            {
                if (firstMin.z <= secondMax.z && firstMax.z >= secondMin.z)
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool checkAABBPlaneCollision(Body bodyA, Body bodyB) 
    {
        //get all  aabb's direction vectors for the vector positions
        Vector3 FowardVec = bodyA.transform.forward * bodyA.transform.localScale.x * 0.5f;
        Vector3 RightVec = bodyA.transform.right * bodyA.transform.localScale.z * 0.5f;
        Vector3 TopVec = bodyA.transform.up * bodyA.transform.localScale.y * 0.5f;
        //get all of the  aabb's front vector postions
        Vector3 AABBFrontTopRightVecPos = bodyA.transform.position +
                ((FowardVec) + (RightVec) + (TopVec));

        Vector3 AABBFrontTopLeftVecPos = bodyA.transform.position +
                ((FowardVec) + (-RightVec) + (TopVec));

        Vector3 AABBFrontBottomRightVecPos = bodyA.transform.position +
                ((FowardVec) + (RightVec) + (-TopVec));

        Vector3 AABBFrontBottomLeftVecPos = bodyA.transform.position +
                ((FowardVec) + (-RightVec) + (-TopVec));

        // aabb back faces
        Vector3 AABBBackTopRightVecPos = bodyA.transform.position +
                ((-FowardVec) + (RightVec) + (TopVec));

        Vector3 AABBBackTopLeftVecPos = bodyA.transform.position +
                ((-FowardVec) + (-RightVec) + (TopVec));

        Vector3 AABBBackBottomRightVecPos = bodyA.transform.position +
                ((-FowardVec) + (RightVec) + (-TopVec));

        Vector3 AABBBackBottomLeftVecPos = bodyA.transform.position +
                ((-FowardVec) + (-RightVec) + (-TopVec));


        Vector3 Min = AABBFrontTopRightVecPos;
        Vector3 Max = AABBFrontTopRightVecPos;

        for (int i = 0; i < 7; i++)
        {
            if (i == 0)
            {
                if (Min.x > AABBFrontTopLeftVecPos.x)
                {
                    Min.x = AABBFrontTopLeftVecPos.x;
                }
                if (Min.y > AABBFrontTopLeftVecPos.y)
                {
                    Min.y = AABBFrontTopLeftVecPos.y;
                }
                if (Min.z > AABBFrontTopLeftVecPos.z)
                {
                    Min.z = AABBFrontTopLeftVecPos.z;
                }
            }
            if (i == 1)
            {
                if (Min.x > AABBFrontBottomRightVecPos.x)
                {
                    Min.x = AABBFrontBottomRightVecPos.x;
                }
                if (Min.y > AABBFrontBottomRightVecPos.y)
                {
                    Min.y = AABBFrontBottomRightVecPos.y;
                }
                if (Min.z > AABBFrontBottomRightVecPos.z)
                {
                    Min.z = AABBFrontBottomRightVecPos.z;
                }
            }
            if (i == 2)
            {
                if (Min.x > AABBFrontBottomLeftVecPos.x)
                {
                    Min.x = AABBFrontBottomLeftVecPos.x;
                }
                if (Min.y > AABBFrontBottomLeftVecPos.y)
                {
                    Min.y = AABBFrontBottomLeftVecPos.y;
                }
                if (Min.z > AABBFrontBottomLeftVecPos.z)
                {
                    Min.z = AABBFrontBottomLeftVecPos.z;
                }
            }
            if (i == 3)
            {
                if (Min.x > AABBBackTopRightVecPos.x)
                {
                    Min.x = AABBBackTopRightVecPos.x;
                }
                if (Min.y > AABBBackTopRightVecPos.y)
                {
                    Min.y = AABBBackTopRightVecPos.y;
                }
                if (Min.z > AABBBackTopRightVecPos.z)
                {
                    Min.z = AABBBackTopRightVecPos.z;
                }
            }
            if (i == 4)
            {
                if (Min.x > AABBBackTopLeftVecPos.x)
                {
                    Min.x = AABBBackTopLeftVecPos.x;
                }
                if (Min.y > AABBBackTopLeftVecPos.y)
                {
                    Min.y = AABBBackTopLeftVecPos.y;
                }
                if (Min.z > AABBBackTopLeftVecPos.z)
                {
                    Min.z = AABBBackTopLeftVecPos.z;
                }
            }
            if (i == 5)
            {
                if (Min.x > AABBBackBottomRightVecPos.x)
                {
                    Min.x = AABBBackBottomRightVecPos.x;
                }
                if (Min.y > AABBBackBottomRightVecPos.y)
                {
                    Min.y = AABBBackBottomRightVecPos.y;
                }
                if (Min.z > AABBBackBottomRightVecPos.z)
                {
                    Min.z = AABBBackBottomRightVecPos.z;
                }
            }
            if (i == 6)
            {
                if (Min.x > AABBBackBottomLeftVecPos.x)
                {
                    Min.x = AABBBackBottomLeftVecPos.x;
                }
                if (Min.y > AABBBackBottomLeftVecPos.y)
                {
                    Min.y = AABBBackBottomLeftVecPos.y;
                }
                if (Min.z > AABBBackBottomLeftVecPos.z)
                {
                    Min.z = AABBBackBottomLeftVecPos.z;
                }
            }
        }

        for (int i = 0; i < 7; i++)
        {
            if (i == 0)
            {
                if (Max.x < AABBFrontTopLeftVecPos.x)
                {
                    Max.x = AABBFrontTopLeftVecPos.x;
                }
                if (Max.y < AABBFrontTopLeftVecPos.y)
                {
                    Max.y = AABBFrontTopLeftVecPos.y;
                }
                if (Max.z < AABBFrontTopLeftVecPos.z)
                {
                    Max.z = AABBFrontTopLeftVecPos.z;
                }
            }
            if (i == 1)
            {
                if (Max.x < AABBFrontBottomRightVecPos.x)
                {
                    Max.x = AABBFrontBottomRightVecPos.x;
                }
                if (Max.y < AABBFrontBottomRightVecPos.y)
                {
                    Max.y = AABBFrontBottomRightVecPos.y;
                }
                if (Max.z < AABBFrontBottomRightVecPos.z)
                {
                    Max.z = AABBFrontBottomRightVecPos.z;
                }
            }
            if (i == 2)
            {
                if (Max.x < AABBFrontBottomLeftVecPos.x)
                {
                    Max.x = AABBFrontBottomLeftVecPos.x;
                }
                if (Max.y < AABBFrontBottomLeftVecPos.y)
                {
                    Max.y = AABBFrontBottomLeftVecPos.y;
                }
                if (Max.z < AABBFrontBottomLeftVecPos.z)
                {
                    Max.z = AABBFrontBottomLeftVecPos.z;
                }
            }
            if (i == 3)
            {
                if (Max.x < AABBBackTopRightVecPos.x)
                {
                    Max.x = AABBBackTopRightVecPos.x;
                }
                if (Max.y < AABBBackTopRightVecPos.y)
                {
                    Max.y = AABBBackTopRightVecPos.y;
                }
                if (Max.z < AABBBackTopRightVecPos.z)
                {
                    Max.z = AABBBackTopRightVecPos.z;
                }
            }
            if (i == 4)
            {
                if (Max.x < AABBBackTopLeftVecPos.x)
                {
                    Max.x = AABBBackTopLeftVecPos.x;
                }
                if (Max.y < AABBBackTopLeftVecPos.y)
                {
                    Max.y = AABBBackTopLeftVecPos.y;
                }
                if (Max.z < AABBBackTopLeftVecPos.z)
                {
                    Max.z = AABBBackTopLeftVecPos.z;
                }
            }
            if (i == 5)
            {
                if (Max.x < AABBBackBottomRightVecPos.x)
                {
                    Max.x = AABBBackBottomRightVecPos.x;
                }
                if (Max.y < AABBBackBottomRightVecPos.y)
                {
                    Max.y = AABBBackBottomRightVecPos.y;
                }
                if (Max.z < AABBBackBottomRightVecPos.z)
                {
                    Max.z = AABBBackBottomRightVecPos.z;
                }
            }
            if (i == 6)
            {
                if (Max.x < AABBBackBottomLeftVecPos.x)
                {
                    Max.x = AABBBackBottomLeftVecPos.x;
                }
                if (Max.y < AABBBackBottomLeftVecPos.y)
                {
                    Max.y = AABBBackBottomLeftVecPos.y;
                }
                if (Max.z < AABBBackBottomLeftVecPos.z)
                {
                    Max.z = AABBBackBottomLeftVecPos.z;
                }
            }
        }

        Vector3 positiveExtents = Max - bodyB.transform.position;

        Vector3 NegativeExtents = Min - bodyB.transform.position;

        Vector3 PlaneNormal = bodyB.transform.rotation * new Vector3(0, 1, 0);

        float Projection = positiveExtents.x * Mathf.Abs(PlaneNormal.x)
                            + positiveExtents.y * Mathf.Abs(PlaneNormal.y)  
                            + positiveExtents.z * Mathf.Abs(PlaneNormal.z);
        //still haven't got dist correct 
        //float distance = Mathf.Abs(PlaneNormal, bodyA.transform.position) - bodyB.transform.position.magnitude;
        float distance = bodyA.transform.position.magnitude - bodyB.transform.position.magnitude;
        return Mathf.Abs(distance) >= Projection;
    }


    Body Fix(Body bodyA, Body bodyB) 
    {
        Vector3 normal = bodyB.transform.rotation * new Vector3(0, 1, 0);
        Vector3 displacement = bodyA.transform.position - bodyB.transform.position;
        float projection = Vector3.Dot(displacement, normal);
        Vector3 gravityForce = grav * bodyA.mass;

        //normal forces
        Vector3 gravityForcePerpendicular = Vector3.Dot(gravityForce, normal) * normal;
        Vector3 normalForce = -gravityForcePerpendicular;
        bodyA.AddForce(normalForce);

        //find net force in parallel direction to surface
        Vector3 fNetParallel = -(gravityForce - gravityForcePerpendicular);
        float fNetParallelMagnitude = fNetParallel.magnitude;
        Vector3 FrictionDirection = fNetParallel.normalized;

        //find velocity in perpendicular and parallel direction to the surface 
        Vector3 Velocity = bodyA.vel;
        Vector3 VelocityPerpendicular = Vector3.Dot(Velocity, normal) * normal;
        Vector3 VelocityParallel = Velocity - VelocityPerpendicular;

        float FrictionMagnitude = normalForce.magnitude*bodyA.coefficientOfFriction;
        Vector3 FrictionForce = -VelocityParallel.normalized * FrictionMagnitude;
        if (Vector3.Dot(Velocity, normal) < 0)
        {
            bodyA.AddForce(FrictionForce);
            bodyA.transform.position += normal * (bodyA.radius - projection);
        }
        Debug.DrawLine(bodyA.transform.position, bodyA.transform.position + normalForce, Color.green);
        Debug.DrawLine(bodyA.transform.position, bodyA.transform.position + gravityForce, Color.magenta);
        Debug.DrawLine(bodyA.transform.position, bodyA.transform.position + FrictionForce, Color.yellow);
        Debug.DrawLine(bodyA.transform.position, bodyA.transform.position + Velocity, Color.red);


        //bodyA.vel += (FrictionForce * 0.3f) * dt;

        //bodyA.vel = Vector3.zero;
        //draw forces


        return bodyA;
    }

    Body FixSphereAABBCol(Body bodyA, Body bodyB)
    {
        //gets all of the aabb's face positions
        Vector3 aabbFrontFacePos = bodyB.transform.position + (bodyB.transform.forward * bodyB.transform.lossyScale.x * 0.5f);
        Vector3 aabbBackFacePos = bodyB.transform.position - (bodyB.transform.forward * bodyB.transform.lossyScale.x * 0.5f);
        Vector3 aabbRightFacePos = bodyB.transform.position + (bodyB.transform.right * bodyB.transform.lossyScale.z * 0.5f);
        Vector3 aabbLeftFacePos = bodyB.transform.position - (bodyB.transform.right * bodyB.transform.lossyScale.x * 0.5f);
        Vector3 aabbTopFacePos = bodyB.transform.position + (bodyB.transform.up * bodyB.transform.lossyScale.x * 0.5f);
        Vector3 aabbBottomFacePos = bodyB.transform.position - (bodyB.transform.up * bodyB.transform.lossyScale.x * 0.5f);

        Vector3 Min = aabbFrontFacePos;
        Vector3 Max = aabbFrontFacePos;

        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                if (Min.x > aabbBackFacePos.x)
                {
                    Min.x = aabbBackFacePos.x;
                }
                if (Min.y > aabbBackFacePos.y)
                {
                    Min.y = aabbBackFacePos.y;
                }
                if (Min.z > aabbBackFacePos.z)
                {
                    Min.z = aabbBackFacePos.z;
                }
            }
            if (i == 1)
            {
                if (Min.x > aabbRightFacePos.x)
                {
                    Min.x = aabbRightFacePos.x;
                }
                if (Min.y > aabbRightFacePos.y)
                {
                    Min.y = aabbRightFacePos.y;
                }
                if (Min.z > aabbRightFacePos.z)
                {
                    Min.z = aabbRightFacePos.z;
                }
            }
            if (i == 2)
            {
                if (Min.x > aabbLeftFacePos.x)
                {
                    Min.x = aabbLeftFacePos.x;
                }
                if (Min.y > aabbLeftFacePos.y)
                {
                    Min.y = aabbLeftFacePos.y;
                }
                if (Min.z > aabbLeftFacePos.z)
                {
                    Min.z = aabbLeftFacePos.z;
                }
            }
            if (i == 3)
            {
                if (Min.x > aabbTopFacePos.x)
                {
                    Min.x = aabbTopFacePos.x;
                }
                if (Min.y > aabbTopFacePos.y)
                {
                    Min.y = aabbTopFacePos.y;
                }
                if (Min.z > aabbTopFacePos.z)
                {
                    Min.z = aabbTopFacePos.z;
                }
            }
            if (i == 4)
            {
                if (Min.x > aabbBottomFacePos.x)
                {
                    Min.x = aabbBottomFacePos.x;
                }
                if (Min.y > aabbBottomFacePos.y)
                {
                    Min.y = aabbBottomFacePos.y;
                }
                if (Min.z > aabbBottomFacePos.z)
                {
                    Min.z = aabbBottomFacePos.z;
                }
            }
        }

        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                if (Max.x < aabbBackFacePos.x)
                {
                    Max.x = aabbBackFacePos.x;
                }
                if (Max.y < aabbBackFacePos.y)
                {
                    Max.y = aabbBackFacePos.y;
                }
                if (Max.z < aabbBackFacePos.z)
                {
                    Max.z = aabbBackFacePos.z;
                }
            }
            if (i == 1)
            {
                if (Max.x < aabbRightFacePos.x)
                {
                    Max.x = aabbRightFacePos.x;
                }
                if (Max.y < aabbRightFacePos.y)
                {
                    Max.y = aabbRightFacePos.y;
                }
                if (Max.z < aabbRightFacePos.z)
                {
                    Max.z = aabbRightFacePos.z;
                }
            }
            if (i == 2)
            {
                if (Max.x < aabbLeftFacePos.x)
                {
                    Max.x = aabbLeftFacePos.x;
                }
                if (Max.y < aabbLeftFacePos.y)
                {
                    Max.y = aabbLeftFacePos.y;
                }
                if (Max.z < aabbLeftFacePos.z)
                {
                    Max.z = aabbLeftFacePos.z;
                }
            }
            if (i == 3)
            {
                if (Max.x < aabbTopFacePos.x)
                {
                    Max.x = aabbTopFacePos.x;
                }
                if (Max.y < aabbTopFacePos.y)
                {
                    Max.y = aabbTopFacePos.y;
                }
                if (Max.z < aabbTopFacePos.z)
                {
                    Max.z = aabbTopFacePos.z;
                }
            }
            if (i == 4)
            {
                if (Max.x < aabbBottomFacePos.x)
                {
                    Max.x = aabbBottomFacePos.x;
                }
                if (Max.y < aabbBottomFacePos.y)
                {
                    Max.y = aabbBottomFacePos.y;
                }
                if (Max.z < aabbBottomFacePos.z)
                {
                    Max.z = aabbBottomFacePos.z;
                }
            }
        }

        Vector3 Nearest = bodyA.transform.position;
        Nearest.x = Mathf.Clamp(bodyA.transform.position.x, bodyB.transform.position.x - Min.x, bodyB.transform.position.x - Max.x);
        Nearest.y = Mathf.Clamp(bodyA.transform.position.y, bodyB.transform.position.y - Min.x, bodyB.transform.position.x - Max.y);
        Nearest.z = Mathf.Clamp(bodyA.transform.position.z, bodyB.transform.position.z - Min.x, bodyB.transform.position.x - Max.z);

        Quaternion prevRotation = bodyB.transform.rotation;
        bodyB.transform.LookAt(Nearest + bodyB.transform.position);
        Vector3 normal = bodyB.transform.rotation * new Vector3(1, 0, 0);
        Vector3 displacement = bodyA.transform.position - (bodyB.transform.position + Nearest);
        float projection = Vector3.Dot(displacement, normal);
        bodyA.transform.position -= normal * (bodyA.radius - projection) * 0.25f;
        bodyB.transform.rotation = prevRotation;
        return bodyA;
    }

    void MomentumConservationCollision(Body bodyA, Body bodyB) 
    {
        Vector3 normal = (bodyB.transform.position - bodyA.transform.position).normalized;
        Vector3 displacement = bodyA.transform.position - bodyB.transform.position;
        float projection = Vector3.Dot(displacement, normal);

        Vector3 RelativeVelocity = bodyB.vel - bodyA.vel;

        Vector3 ClosingVelocity = Vector3.Dot(RelativeVelocity, normal) * normal;

        //average of the bounciness
        float Restitution = (bodyA.Bounciness + bodyB.Bounciness) / 2;

        Vector3 ClosingVelocityRetained = Restitution * ClosingVelocity;

        Vector3 ClosingVelocityFinal = -ClosingVelocityRetained;
        
        float Impulse = -(1 + Restitution) * Vector3.Dot(RelativeVelocity, normal) * bodyA.mass * bodyB.mass / (bodyA.mass + bodyB.mass);

        Vector3 VelocityChangeA = Impulse/bodyA.mass * -normal;

        Vector3 VelocityChangeB = Impulse / bodyB.mass * normal;

        if (Vector3.Dot(RelativeVelocity, normal) >= 0) 
        {}
        else
        {
            //bodyA.vel += ((bodyB.mass / bodyA.mass) * ClosingVelocity);
            bodyA.vel += (VelocityChangeA);
            //bodyA.transform.position += normal * (bodyA.radius - projection);
            bodyB.vel += (VelocityChangeB);
        }
    }

    void MomentumConservationCollisionAsymmtrical(Body bodyA, Body bodyB, Vector3 Normal)
    {
        Vector3 normal = (bodyB.transform.position - bodyA.transform.position).normalized;
        Vector3 displacement = bodyA.transform.position - bodyB.transform.position;
        float projection = Vector3.Dot(displacement, normal);

        Vector3 RelativeVelocity = bodyB.vel - bodyA.vel;

        Vector3 ClosingVelocity = Vector3.Dot(RelativeVelocity, normal) * normal;

        //average of the bounciness
        float Restitution = (bodyA.Bounciness + bodyB.Bounciness) / 2;

        Vector3 ClosingVelocityRetained = Restitution * ClosingVelocity;

        Vector3 ClosingVelocityFinal = -ClosingVelocityRetained;

        float ImpulsehasNonKinematic = -(1 + Restitution) * Vector3.Dot(RelativeVelocity, Normal);

        Vector3 VelocityChangeNonKine = ImpulsehasNonKinematic * Normal;

        if (Vector3.Dot(RelativeVelocity, normal) >= 0)
        { }
        else
        {
            //bodyA.vel += ((bodyB.mass / bodyA.mass) * ClosingVelocity);
            if (bodyA.isKinematic && !bodyB.isKinematic)
            {
                bodyA.vel += (-VelocityChangeNonKine);
            }
            else if (!bodyA.isKinematic && bodyB.isKinematic)
            {
                bodyB.vel += (-VelocityChangeNonKine);
            }
            else if (!bodyA.isKinematic && !bodyB.isKinematic) { }
            else
            { 
            }
        }
    }

    private void checkCollision()
    {
        for (int i = 0; i < bodies.Count; i++)
        {
            Body bodyA = bodies[i];
            for (int j = 0; j < bodies.Count; j++)
            {
                Body bodyB = bodies[j];
                //checks for collision detection type
                if (bodyA.GetShape() == 0 && bodyB.GetShape() == 0)
                {
                    if (checkSphereSphereCollision(bodyA, bodyB))
                    {
                        if (bodyA.isKinematic && bodyB.isKinematic)
                        {
                            MomentumConservationCollision(bodyA, bodyB);
                        }
                        else if (bodyA.isKinematic && !bodyB.isKinematic)
                        {
                            Vector3 Normal = bodyB.transform.rotation * new Vector3(0, 1, 0);
                            MomentumConservationCollisionAsymmtrical(bodyA, bodyB, Normal);
                        }
                        else if (!bodyA.isKinematic && bodyB.isKinematic)
                        {
                            Vector3 Normal = bodyA.transform.rotation * new Vector3(0, 1, 0);
                            MomentumConservationCollisionAsymmtrical(bodyA, bodyB, Normal);
                        }
                    }
                    else
                    {

                    }
                }
                else if (bodyA.GetShape() == 0 && bodyB.GetShape() == 1)
                {
                    if (checkSphereAABBCollision(bodyA, bodyB))
                    {
                        Debug.Log("Hit");
                        bodyA = FixSphereAABBCol(bodyA, bodyB);
                    }
                }
                else if (bodyB.GetShape() == 0 && bodyA.GetShape() == 1)
                {

                }
                else if (bodyA.GetShape() == 0 && bodyB.GetShape() == 2)
                {
                    if (checkSpherePlaneCollision(bodyA, bodyB))
                    {
                        if (bodyA.isKinematic && bodyB.isKinematic)
                        {
                            MomentumConservationCollision(bodyA, bodyB);
                        }
                        else if (bodyA.isKinematic && !bodyB.isKinematic)
                        {
                            Vector3 Normal = bodyB.transform.rotation * new Vector3(0, 1, 0);
                            MomentumConservationCollisionAsymmtrical(bodyA, bodyB, Normal);
                        }
                        else if (!bodyA.isKinematic && bodyB.isKinematic)
                        {
                            Vector3 Normal = bodyA.transform.rotation * new Vector3(0, 1, 0);
                            MomentumConservationCollisionAsymmtrical(bodyA, bodyB, Normal);
                        }


                        if (bodyA.isKinematic)
                        {
                            bodyA = Fix(bodyA, bodyB);
                        }
                    }
                    else
                    {

                    }
                }
                else if (bodyB.GetShape() == 0 && bodyA.GetShape() == 2)
                {
                    if (checkSpherePlaneCollision(bodyB, bodyA))
                    {

                    }
                    else
                    {

                    }
                }
                else if (bodyA.GetShape() == 0 && bodyB.GetShape() == 3)
                {
                    if (checkSphereHalfPlaneCollision(bodyA, bodyB))
                    {
                        if (bodyA.isKinematic && bodyB.isKinematic)
                        {
                            MomentumConservationCollision(bodyA, bodyB);
                        }
                        else if (bodyA.isKinematic && !bodyB.isKinematic)
                        {
                            Vector3 Normal = bodyB.transform.rotation * new Vector3(0, 1, 0);
                            MomentumConservationCollisionAsymmtrical(bodyA, bodyB, Normal);
                        }
                        else if (!bodyA.isKinematic && bodyB.isKinematic)
                        {
                            Vector3 Normal = bodyA.transform.rotation * new Vector3(0, 1, 0);
                            MomentumConservationCollisionAsymmtrical(bodyA, bodyB, Normal);
                        }
                    }
                    else
                    {

                    }
                }
                else if (bodyB.GetShape() == 0 && bodyA.GetShape() == 3)
                {
                    if (checkSphereHalfPlaneCollision(bodyB, bodyA))
                    {

                    }
                    else
                    {

                    }
                }
                else if (bodyA.GetShape() == 1 && bodyB.GetShape() == 1)
                {
                    if (bodyA != bodyB) 
                    {
                        if (checkAABBAABBCollision(bodyA, bodyB)) 
                        {
                            Debug.Log("CubeHit");
                        }
                    }
                }
                else if (bodyA.GetShape() == 1 && bodyB.GetShape() == 2)
                {
                    //if (checkAABBPlaneCollision(bodyA, bodyB))
                    //{
                    //    Debug.Log("Bruh");
                    //}
                }
                else if (bodyB.GetShape() == 1 && bodyA.GetShape() == 2)
                {

                }
            }
        }
    }
    private void Start()
    {
        dt = Time.fixedDeltaTime;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        checkForNewBodies();
        foreach (Body body in bodies)
        {
            if (body.isKinematic)
            {
                body.Simulate(grav, dt);
                body.transform.localPosition += new Vector3(
                    (body.vel.x * dt) * body.drag,
                    (body.vel.y * dt) * body.drag,
                    (body.vel.z * dt) * body.drag);
            }
        }
        checkCollision();
        //Simulate(Physics.gravity, Time.fixedDeltaTime);
        //transform.position = new Vector3(
        //    transform.position.x + (vel.x * dt) * drag,
        //    transform.position.y + (vel.y * dt) * drag,
        //    transform.position.z + (vel.z * dt) * drag
        //);
    }
}