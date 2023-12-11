using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : BehaviorNode
{
    private IAInfo characterIA;
    private IAController iaController;
    private int countRangeYellow;
    private int countRangeRed;

    public ActionNode(IAInfo characterIA, IAController iaController, int countRangeYellow, int countRangeRed)
    {
        this.characterIA = characterIA;
        this.iaController = iaController;
        this.countRangeYellow = countRangeYellow;
        this.countRangeRed = countRangeRed;
    }

    public override NodeStatus Execute()
    {
        // Lógica del arbol de comportamiento

        if (publicVariables.myTurn)
        {
            if (RandomMove())
            {
                iaController.RandomMove();
            }
            if (LookMove())
            {
                iaController.LookMove();
            }
            if (AttackMove())
            {
                iaController.AttackMove();
            }
            Debug.Log("Estados: " + RandomMove() + LookMove() + AttackMove());
            return NodeStatus.Success;

        }
        return NodeStatus.Failure;

    }
        
    private bool RandomMove()
    {
        bool checkMove = false;
        if (countRangeYellow == 0 && countRangeRed == 0)
        {
            checkMove = true;
        }
        return checkMove; // Cambia esto según tu lógica
    }
    private bool LookMove()
    {
        bool checkMove = false;
        if (countRangeYellow != 0 && countRangeRed == 0)
        {
            checkMove = true;
        }
        return checkMove; // Cambia esto según tu lógica
    }

    private bool AttackMove()
    {
        bool checkAttack = false;
        if (countRangeRed > 0)
        {
            checkAttack = true;
        }

        return checkAttack;
    }
}
