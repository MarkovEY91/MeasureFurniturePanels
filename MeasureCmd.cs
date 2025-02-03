using System;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace MeasureFurniturePanels
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MeasureCmd : IExternalCommand
    {

        private UIApplication _uiApp;
        private Application _app;
        private string _docName;
        private UIDocument _uiDoc;
        private Document _doc;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            _uiApp = commandData.Application;
            _uiDoc = _uiApp.ActiveUIDocument;
            _app = _uiApp.Application;
            _doc = _uiDoc.Document;
            _docName = _doc.Title;

            try
            {
                // MarkFurniture(_doc);
                FurnitureUtils.ProcessDirectShapes(_doc, 1308511);

                TaskDialog.Show("Инфо", "Готово");
                return Result.Succeeded;
            }
            catch (CustomWarningException e)
            {
                TaskDialog.Show("Внимание", e.Message);
            }

            catch (Exception e)
            {
                
                TaskDialog.Show("Внимание", e.Message + "\n\n" + e.StackTrace);
            }

            return Result.Failed;

        }

        private void MarkFurniture(Document doc)
        {
            throw new NotImplementedException();
        }
    }
}