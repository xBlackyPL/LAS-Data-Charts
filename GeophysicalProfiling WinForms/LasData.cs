using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Profilowanie_Geofizyczne_WForms
{
    internal class LasData
    {
        public readonly Dictionary<string, List<double>> charts;
        private string path { get; }

        public LasData(string pathToLasFile)
        {
            path = pathToLasFile;
            var name = pathToLasFile.Split('\\');
            filename = name.Last();

            using (var file = new StreamReader(pathToLasFile))
            {
                string line;
                var keyWordBeginningIndex = 0;
                var keyWordsBeginning = new[] { "STRT.M", "STOP.M", "STEP.M", "NULL." };
                var chartDataHeader = new[] { "~A", "#" };
                var delimiters = new[] { ':' };
                var startValues = new double[keyWordsBeginning.Length];
                var headerInfo = true;
                var numberOfColumns = 0;
                var chartsTitleList = new List<string>();

                charts = new Dictionary<string, List<double>>();

                while ((line = file.ReadLine()) != null)
                {
                    if (line == "") continue;
                    var parts = Regex.Split(line, @"\s+");
                    parts = parts.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                    if (headerInfo)
                    {
                        for (var i = 0; i < parts.Length; i++)
                        {
                            if (keyWordBeginningIndex == keyWordsBeginning.Length)
                            {
                                keyWordBeginningIndex = 0;
                                headerInfo = false;
                                startValue = startValues[0];
                                stopValue = startValues[1];
                                stepValue = startValues[2];
                                nullValue = startValues[3];
                                break;
                            }

                            if (parts[i] == keyWordsBeginning[keyWordBeginningIndex])
                            {
                                var data = parts[i + 1].Split(delimiters)[0];

                                startValues[keyWordBeginningIndex++] =
                                    double.Parse(data, CultureInfo.InvariantCulture);
                            }
                        }
                    }

                    else
                    {
                        if (parts[0] == chartDataHeader[0])
                        {
                            numberOfColumns = parts.Length - 1;
                            foreach (var word in parts.Skip(1))
                            {
                                charts.Add(word, new List<double>());
                                chartsTitleList.Add(word);
                            }
                        }

                        if (parts[0] != chartDataHeader[0] && parts[0] != chartDataHeader[1])
                            for (var i = 0; i < numberOfColumns; i++)
                            {
                                var dataString = parts[i].Split(delimiters)[0];

                                var fmt = new NumberFormatInfo
                                {
                                    NegativeSign = "-"
                                };

                                var dataDouble = double.Parse(dataString, fmt);
                                charts[chartsTitleList[i]].Add(dataDouble);
                            }
                    }
                }
            }
        }

        public double startValue { get; private set; }

        public double stopValue { get; private set; }

        public double stepValue { get; private set; }

        public double nullValue { get; private set; }

        public string filename { get; private set; }
    }
}
