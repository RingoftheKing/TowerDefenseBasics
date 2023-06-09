using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node : MonoBehaviour
{
    private BuildManager builder;
    private Color onHover;
    public Color notEnoughMoneyColor;
    //private Renderer rend;
    private Material rend;
    private Color STARTCOLOR;
    public Vector3 positionOffset;
    internal bool nodeOccupied; // Might be redundant since there's turretOnNode
    
    // this stores information about upgrade status.
    // May be updated to be an array when branching is implemented.
    public bool IsUpgraded; 

    [Header("Do not touch")]
    internal Buildings turretOnNode; // Is there already a turret here?
    internal TurretBlueprint turretBlueprint;
    
    

    void Start()
    {
        builder = BuildManager.Builder;
        //rend = GetComponent<Renderer>();
        rend = GetComponent<Renderer>().materials.ToList()[1];
        STARTCOLOR = rend.color;
        onHover.r = STARTCOLOR.r + 0.1f;
        onHover.g = STARTCOLOR.g + 0.1f;
        onHover.b = STARTCOLOR.b + 0.1f;
        nodeOccupied = false;
        
    }

    void OnMouseEnter()
    {
        // Selecting a node when shop is not selected should highlight the node
        if (nodeOccupied && !builder.canBuild)
        {
            rend.color = onHover;
        }

        // If shop is not selected, don't highlight node
        if (!builder.canBuild)  
        {
            return;
        }

        // If conditions met below, ready to build turret. Else do nothing.
        if (builder.hasMoney && builder.canBuild && !nodeOccupied)
        {
            rend.color = onHover;
        } 
        else if (!builder.hasMoney)
        {
            rend.color = notEnoughMoneyColor;
        }
    }

    void OnMouseExit()
    {
        rend.color = STARTCOLOR;
    }

    private void OnMouseDown()
    {
        // If turret is built, select node and bring up node UI
        if (nodeOccupied) 
        {
            builder.SelectNode(this);
            return; 
        }

        // If shop turret not selected, return
        if (!builder.canBuild) { return; } 
        
        this.BuildTurret(builder.turretSelected);
    }

    void BuildTurret(TurretBlueprint turretPrefab)
    {
        if (PlayerStats.Money < turretPrefab.cost)
        {
            Debug.Log("Not enough money");
            builder.Shop.deselectTurret();
            return;
        }

        GameObject turretToBuildIns = (GameObject) Instantiate(turretPrefab.prefab, this.PositionToBuild(), Quaternion.identity);

        if (turretPrefab.isOffensiveTurret)
        {
            Turret turretBuilt = turretToBuildIns.GetComponent<Turret>();
            this.turretOnNode = turretBuilt;
        } else
        {
            UtilityTurret turretBuilt = turretToBuildIns.GetComponent<UtilityTurret>();
            this.turretOnNode = turretBuilt;
        }
        
        GameObject buildEffectIns = (GameObject) Instantiate(builder.buildEffect, this.PositionToBuild(), Quaternion.identity);
        Destroy(buildEffectIns, 2f);

        this.turretBlueprint = turretPrefab;
        this.turretBlueprint.isUpgraded = false;
        this.nodeOccupied = true;

        PlayerStats.Money -= turretPrefab.cost;
        Debug.Log("$" + turretPrefab.cost + " spent to build turret");

        builder.Shop.deselectTurret();
    }

    public void UpgradeTurret()
    {
        if (PlayerStats.Money < turretBlueprint.upgradeCost)
        {
            Debug.Log("Not enough money");
            builder.Shop.deselectTurret();
            return;
        }
        Destroy(turretOnNode.gameObject);

        GameObject turretToBuildIns = (GameObject) Instantiate(turretBlueprint.upgradedPrefab, this.PositionToBuild(), Quaternion.identity);
        if (turretBlueprint.isOffensiveTurret)
        {
            Turret turretBuilt = turretToBuildIns.GetComponent<Turret>();
            turretBuilt.isUpgraded = true;
            this.turretOnNode = turretBuilt;
        }
        else
        {
            UtilityTurret turretBuilt = turretToBuildIns.GetComponent<UtilityTurret>();
            //turretBuilt.isUpgraded = true;
            this.turretOnNode = turretBuilt;
        }
        
        GameObject buildEffectIns = (GameObject) Instantiate(builder.buildEffect, this.PositionToBuild(), Quaternion.identity);
        Destroy(buildEffectIns, 2f);

        PlayerStats.Money -= turretBlueprint.upgradeCost;
        this.IsUpgraded = true;
        this.turretBlueprint.isUpgraded = true;
        Debug.Log("$" + turretBlueprint.upgradeCost + " spent to upgrade turret");

        builder.DeselectNode();
        //turretOnNode.isUpgraded = true;
    }

    public void SellTurret()
    {
        long returns = (long) turretBlueprint.sellAmount();
        PlayerStats.Money += returns;

        if (turretOnNode == null)
        {
            Debug.Log("null");
        }
        Destroy(turretOnNode.gameObject);
        Debug.Log("$" + returns + " received from selling turret");

        turretBlueprint = null;
        nodeOccupied = false;
        builder.DeselectNode();
    }

    public void EquipGear(Gear gear)
    {
        Debug.Log("Gear equipped!");

        if (turretOnNode is Turret)
        {
            Turret turret = turretOnNode.GetComponent<Turret>();
            turret.TriggerGearEffect(gear);
        } 
        
        builder.DeselectNode();
    }

    public Vector3 PositionToBuild()
    {
        return transform.position + positionOffset;
    }
}
