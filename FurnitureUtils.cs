using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace MeasureFurniturePanels
{
    public static class FurnitureUtils
    {
        public static List<DirectShape> ProcessDirectShapes(Document doc, int linkIfcId)
        {
            var ifcLink = (RevitLinkInstance) doc.GetElement(new ElementId(linkIfcId));
            var ifcDoc = ifcLink.GetLinkDocument();
            var directShapes = new FilteredElementCollector(doc).OfClass(typeof(DirectShape)).Cast<DirectShape>()
                .ToList();

            
            var listIdsToContinue = new List<int>()
            {
                264236
            };
            foreach (DirectShape ds in directShapes)
            {
                if (!listIdsToContinue.Contains(ds.Id.IntegerValue))
                {
                    continue;
                }

                List<Solid> dsSolids = SolidExtractor.GetSolidsRecursively(ds, null, null);
                TaskDialog.Show("dsSolids count", dsSolids.Count.ToString());

                
            }
            
            
            return directShapes;
        }   
    }
}