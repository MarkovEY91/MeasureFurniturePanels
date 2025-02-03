using System;
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
            
            var listIdsToContinue = new List<int>()
            {
                264191
            };
            
            var ifcLink = (RevitLinkInstance)doc.GetElement(new ElementId(linkIfcId));
            var ifcDoc = ifcLink.GetLinkDocument();
            var directShapes = new FilteredElementCollector(ifcDoc).OfClass(typeof(DirectShape)).Cast<DirectShape>()
                .Where(x=> listIdsToContinue.Contains(x.Id.IntegerValue))
                .ToList();


            // foreach (DirectShape ds in directShapes)
            // {
            //     if (!listIdsToContinue.Contains(ds.Id.IntegerValue))
            //     {
            //         continue;
            //     }
            //
            //     List<Solid> dsSolids = SolidExtractor.GetSolidsRecursively(ds, null, null);
            //     TaskDialog.Show("dsSolids count", dsSolids.Count.ToString());
            // }
            // TaskDialog.Show("ds count", directShapes.Count.ToString());

            using (Transaction trans = new Transaction(doc, "Create DirectShapes"))
            {
                trans.Start();

                foreach (var directShape in directShapes)
                {
                    // Получаем геометрию элемента
                    Options options = new Options();
                    GeometryElement geometryElement = directShape.get_Geometry(options);

                    // Проверяем, является ли геометрия параллелепипедом
                    var isParallelepiped = IsParallelepiped(geometryElement, out double length, out double width,
                        out double height);
                    
                    if (isParallelepiped)
                    {
                        // Определяем ориентацию
                        bool isHorizontal = IsHorizontal(length, width, height);

                        // Создаем новый DirectShape в текущей модели
                        DirectShape newDirectShape = DirectShape.CreateElement(doc, directShape.Category.Id);
                        // newDirectShape.SetShape(new List<GeometryObject>(){(GeometryObject)geometryElement});
                        newDirectShape.SetShape(geometryElement.Select(x => x).ToList());

                        // Вычисляем габариты
                        string dimensions = GetDimensionsString(length, width, height, isHorizontal);

                        // Записываем габариты в параметр "Комментарии"
                        Parameter commentParam = newDirectShape.LookupParameter("Комментарии");
                        if (commentParam != null && !commentParam.IsReadOnly)
                        {
                            commentParam.Set(dimensions);
                        }
                    }
                }

                trans.Commit();
            }


            return directShapes;
        }

        private static bool IsParallelepiped(GeometryElement geometryElement, out double length, out double width,
            out double height)
        {
            length = width = height = 0;

            foreach (GeometryObject geomObj in geometryElement)
            {
                if (geomObj is Solid solid)
                {
                    if (solid.Faces.Size == 6)
                    {
                        // Получаем габариты параллелепипеда
                        BoundingBoxXYZ bbox = solid.GetBoundingBox();
                        length = Math.Round((bbox.Max.X - bbox.Min.X) * 304.8);
                        width = Math.Round((bbox.Max.Y - bbox.Min.Y)  * 304.8);
                        height = Math.Round((bbox.Max.Z - bbox.Min.Z) * 304.8);

                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsHorizontal(double length, double width, double height)
        {
            // Определяем ориентацию по наибольшей грани
            double maxArea = Math.Max(length * width, Math.Max(width * height, height * length));
            return maxArea == length * width;
        }

        private static string GetDimensionsString(double length, double width, double height, bool isHorizontal)
        {
            double[] dimensions = { length, width, height };
            Array.Sort(dimensions);

            double thickness = dimensions[0];
            double depthOrHeight = dimensions[1];
            double longest = dimensions[2];

            if (isHorizontal)
            {
                return $"{longest}(Д)_{depthOrHeight}(Г)_{thickness}(Т)";
            }
            else
            {
                return $"{longest}(Д)_{depthOrHeight}(В)_{thickness}(Т)";
            }
        }
    }
}