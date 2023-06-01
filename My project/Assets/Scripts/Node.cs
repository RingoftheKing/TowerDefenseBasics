using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private BuildManager builder;
    private Color onHover;
    public Color notEnoughMoneyColor;
    private Renderer rend;
    private Color STARTCOLOR;
    private Vector3 positionOffset;
    //internal bool nodeOccupied; // Might be redundant since there's turretOnNode

    [Header("Do not touch")]
    internal GameObject turretOnNode; // Is there already a turret here?
    

    void Start()
    {
        builder = BuildManager.Builder;
        rend = GetComponent<Renderer>();
        STARTCOLOR = rend.material.color;
        onHover.r = STARTCOLOR.r + 0.1f;
        onHover.g = STARTCOLOR.g + 0.1f;
        onHover.b = STARTCOLOR.b + 0.1f;
        //nodeOccupied = false;
    }

    void OnMouseEnter()
    {
        if (!builder.canBuild) { return; } // Nothing is selected
        
        if (turretOnNode != null) // Something is built
        {
            rend.material.color = onHover;
        } 
        
       
        if (builder.hasMoney)
        {
            rend.material.color = onHover;
        } else
        {
            rend.material.color = notEnoughMoneyColor;
        }
    }

    void OnMouseExit()
    {
        rend.material.color = STARTCOLOR;
    }

    private void OnMouseDown()
    {
        if (turretOnNode != null) // Something is built
        {
            builder.SelectNode(this);
            return; 
        }

        if (!builder.canBuild) { return; } // Nothing is selected
        Debug.Log("Built");
        builder.BuildTurret(this);
    }

    public Vector3 PositionToBuild()
    {
        return transform.position + positionOffset;
    }
}
