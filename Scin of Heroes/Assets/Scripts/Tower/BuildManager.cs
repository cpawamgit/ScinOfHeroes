using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour {

    public static BuildManager Instance;
    public GameObject buildEffect;
    public GameObject sellEffect;

    #region Singleton
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one Buildmanager in scene!");
            return;
        }
        Instance = this;
    }
    #endregion


    private TurretBlueprint turretToBuild;
    private Node selectedNode;

    public bool CanBuild { get { return turretToBuild != null; } }
    public bool HasMoney { get { return PlayerStats.Instance.Money >= turretToBuild.cost; } }

    public NodeUI nodeUI;



    public void SelectNode (Node node)
    {
        if (selectedNode == node)
        {
            DeselectNode();
            return;
        }

        //if (node.buildingDisable)
        //{
        //    nodeUI.SetTarget(node);
        //    return;
        //}

        selectedNode = node;
        turretToBuild = null;

        nodeUI.SetTarget(node);
    }

    public void DeselectNode()
    {
        selectedNode = null;
        nodeUI.Hide();
    }

    public void SelectTurretToBuild(TurretBlueprint turret)
    {
        turretToBuild = turret;
        DeselectNode();
    }

    public TurretBlueprint GetTurretToBuild()
    {
        return turretToBuild;
    }
}
