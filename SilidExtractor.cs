    using Autodesk.Revit.DB;
using System.Collections.Generic;
    
namespace MeasureFurniturePanels
{

public class SolidExtractor
{
    public static List<Solid> GetSolidsRecursively(Element element, Options gOptions = null, List<Solid> solids = null)
    {
        // Если список solids не передан, создаем новый список
        if (solids == null)
        {
            solids = new List<Solid>();
        }

        if (gOptions == null)
        {
            gOptions = new Options();
        }

        // Проверка на вложенные семейства
        if (element is FamilyInstance familyInstance)
        {
            foreach (ElementId subElementId in familyInstance.GetSubComponentIds())
            {
                Element subElement = element.Document.GetElement(subElementId);
                if (subElement != null)
                {
                    GetSolidsRecursively(subElement, gOptions, solids);
                }
            }
        }

        // Получаем геометрию элемента
        GeometryElement geomElement = element.get_Geometry(gOptions);
        if (geomElement != null)
        {
            foreach (GeometryObject geomObj in geomElement)
            {
                // Если это GeometryInstance, обрабатываем его отдельно
                if (geomObj is GeometryInstance geomInstance)
                {
                    GeometryElement instanceGeometry = geomInstance.GetInstanceGeometry();
                    if (instanceGeometry != null)
                    {
                        foreach (GeometryObject instanceObj in instanceGeometry)
                        {
                            GetSolidsRecursively(instanceObj, gOptions, solids);
                        }
                    }
                }
                // Если это Solid, добавляем его в список
                else if (geomObj is Solid solid && solid.Volume > 0)
                {
                    solids.Add(solid);
                }
            }
        }

        return solids;
    }

    private static void GetSolidsRecursively(GeometryObject geomObj, Options gOptions, List<Solid> solids)
    {
        if (geomObj is GeometryInstance geomInstance)
        {
            GeometryElement instanceGeometry = geomInstance.GetInstanceGeometry();
            if (instanceGeometry != null)
            {
                foreach (GeometryObject instanceObj in instanceGeometry)
                {
                    GetSolidsRecursively(instanceObj, gOptions, solids);
                }
            }
        }
        else if (geomObj is Solid solid && solid.Volume > 0)
        {
            solids.Add(solid);
        }
    }
}

}