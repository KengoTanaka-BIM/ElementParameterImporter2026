using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ElementParameterImporter2026
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // 固定パス（デスクトップ）
            string csvPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "SelectedElements_Parameters.csv"
            );

            if (!File.Exists(csvPath))
            {
                TaskDialog.Show("インポート", $"CSVファイルが見つかりません:\n{csvPath}");
                return Result.Cancelled;
            }

            StringBuilder changeLog = new StringBuilder();

            using (Transaction trans = new Transaction(doc, "Import CSV Parameters"))
            {
                trans.Start();

                var lines = File.ReadAllLines(csvPath, Encoding.UTF8);

                // 1行目はヘッダー行なのでスキップ
                foreach (var line in lines.Skip(1))
                {
                    var cols = ParseCsvLine(line);
                    if (cols.Length < 3) continue;

                    if (!int.TryParse(cols[0], out int elementIdInt)) continue;

                    string paramName = cols[1];
                    string newValue = cols[2];

                    Element elem = doc.GetElement(new ElementId(elementIdInt));
                    if (elem == null) continue;

                    Parameter param = elem.LookupParameter(paramName);
                    if (param == null || param.IsReadOnly) continue;

                    string oldValue = GetParameterValue(param);

                    // 値が変わっていたら更新
                    if ((oldValue ?? "") != (newValue ?? ""))
                    {
                        try
                        {
                            SetParameterValue(param, newValue);
                            changeLog.AppendLine(
                                $"ElementId: {elem.Id} | {paramName} | \"{oldValue}\" → \"{newValue}\""
                            );
                        }
                        catch (Exception ex)
                        {
                            changeLog.AppendLine(
                                $"[ERROR] ElementId: {elem.Id} | {paramName} | {ex.Message}"
                            );
                        }
                    }
                }

                trans.Commit();
            }

            // 最後にまとめて表示
            string logResult = changeLog.Length > 0
                ? changeLog.ToString()
                : "変更はありませんでした。";

            TaskDialog.Show("インポート結果", logResult);

            return Result.Succeeded;
        }

        private string GetParameterValue(Parameter param)
        {
            switch (param.StorageType)
            {
                case StorageType.Double:
                    return param.AsDouble().ToString();
                case StorageType.Integer:
                    return param.AsInteger().ToString();
                case StorageType.ElementId:
                    return param.AsElementId().Value.ToString();
                case StorageType.String:
                    return param.AsString();
                default:
                    return "";
            }
        }

        private void SetParameterValue(Parameter param, string value)
        {
            switch (param.StorageType)
            {
                case StorageType.String:
                    param.Set(value);
                    break;
                case StorageType.Double:
                    if (double.TryParse(value, out double dVal))
                        param.Set(dVal);
                    break;
                case StorageType.Integer:
                    if (int.TryParse(value, out int iVal))
                        param.Set(iVal);
                    break;
                case StorageType.ElementId:
                    if (int.TryParse(value, out int eIdVal))
                        param.Set(new ElementId(eIdVal));
                    break;
            }
        }

        private string[] ParseCsvLine(string line)
        {
            List<string> result = new List<string>();
            bool inQuotes = false;
            StringBuilder value = new StringBuilder();

            foreach (char c in line)
            {
                if (c == '\"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(value.ToString());
                    value.Clear();
                }
                else
                {
                    value.Append(c);
                }
            }
            result.Add(value.ToString());

            return result.ToArray();
        }
    }
}
