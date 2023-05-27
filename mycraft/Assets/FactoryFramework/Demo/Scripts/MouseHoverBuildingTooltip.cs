using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using FactoryFramework;

public class MouseHoverBuildingTooltip : MouseHoverTooltip
{


    protected override string DisplayMessage()
    {

        if (TryGetComponent(out PowerGridComponent pgc))
        {
            if (pgc.basePowerDraw > 0)
            {
                if (pgc.grid == null || pgc.grid.Efficiency < 1f)
                {
                    return $"{gameObject.name} does not have enough power! Build more Energy buildings";
                }
            } else if (pgc.basePowerDraw < 0)
            {
                return $"Generating {Mathf.Abs(pgc.basePowerDraw)} energy";
            }else if (!TryGetComponent(out LogisticComponent _))
            {
                if (pgc.useConnectionRadius)
                {
                    return $"Area of Effect Power. Grid producing {pgc.grid.Production} energy and using {pgc.grid.Load} energy";
                } else {
                    return $"Direct Power. Grid producing {pgc.grid.Production} energy and using {pgc.grid.Load} energy";
                }
            }
        }

        if (TryGetComponent(out Driller producer))
        {
            //if (producer.resource.itemStack.item == null) return "";
            //return $"{gameObject.name} producing {producer.resource.itemStack.amount} {producer.resource.itemStack.item.name}";
            return $"{gameObject.name} producing ...아이템명 아이템개수 노출할것";
        }
        if (TryGetComponent(out Forge proc))
        {
            //HG[2023.06.09] Item -> MyCraft.ItemBase
            //return $"{gameObject.name} processing items to make {proc.recipe?.name ?? "nothing"}";
            return $"{gameObject.name} processing items to make recipe nothing";
        }
        if (TryGetComponent(out Splitter _))
        {
            return $"{gameObject.name} splits conveyor belts";
        }
        if (TryGetComponent(out Merger _))
        {
            return $"{gameObject.name} merges conveyor belts";
        }
        if (TryGetComponent(out Storage _))
        {
            return $"{gameObject.name} can store items";
        }
        return $"{gameObject.name}";
    }

   
}
