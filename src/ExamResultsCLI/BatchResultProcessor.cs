using System.Globalization;
using System.Text.Json;
using CsvHelper;

namespace ExamResultsCLI;

public class BatchResultProcessor
{
    const string theoryI = "THEORY I";

    const string theoryII = "THEORY II";

    const string theory = "THEORY IN MARKS";

    const string theoryTotal = "THEORY TOTAL IN MARKS";

    const string practical = "PRACTICAL/CLINICAL + VIVA IN MARKS";

    const string total = "TOTAL (THEORY+PRACTICAL/CLINICAL+VIVA) IN %";

    public static async Task FormatResultAsync(string workingDirectory, string? jsonFolder, string resultsFile)
    {
        string[] jsonFiles = Directory.GetFiles(jsonFolder ?? workingDirectory, "*.json");
        using StreamWriter writer = new(Path.Combine(workingDirectory, resultsFile));
        CsvWriter csvWriter = new(writer, CultureInfo.InvariantCulture);

        List<StudentResultEntry> entries = [];

        foreach (string file in jsonFiles)
        {
            string json = await File.ReadAllTextAsync(file);
            ResultResponse? resultResponse = JsonSerializer.Deserialize<ResultResponse>(json);
            if (resultResponse != null && resultResponse.Result != null && resultResponse.Result.Student?.Length > 0)
            {
                Student student = resultResponse.Result.Student[0];

                StudentResultEntry entry = new()
                {
                    RegNo = student.RegistrationNo,
                    Name = student.StudentName,
                };
                entries.Add(entry);

                foreach (Subject subject in student.Subject)
                {
                    if (subject.SubjectCode == "526081,\n526082") // IM
                    {
                        foreach (Paper paper in subject.Paper)
                        {
                            switch (paper.PaperName)
                            {
                                case theoryI:
                                    entry.IMTheoryI = paper.ObtainedMark;
                                    break;
                                case theoryII:
                                    entry.IMTheoryII = paper.ObtainedMark;
                                    break;
                                case theoryTotal:
                                    entry.IMTheoryTotal = paper.ObtainedMark;
                                    break;
                                case practical:
                                    entry.IMPrac = paper.ObtainedMark;
                                    break;
                                case total:
                                    entry.IMTotal = paper.ObtainedMark;
                                    break;
                            }
                        }
                        entry.IMResult = subject.Result;
                    }
                    else if (subject.SubjectCode == "526083,\n526084") // SU
                    {
                        foreach (Paper paper in subject.Paper)
                        {
                            switch (paper.PaperName)
                            {
                                case theoryI:
                                    entry.SUTheoryI = paper.ObtainedMark;
                                    break;
                                case theoryII:
                                    entry.SUTheoryII = paper.ObtainedMark;
                                    break;
                                case theoryTotal:
                                    entry.SUTheoryTotal = paper.ObtainedMark;
                                    break;
                                case practical:
                                    entry.SUPrac = paper.ObtainedMark;
                                    break;
                                case total:
                                    entry.SUTotal = paper.ObtainedMark;
                                    break;
                            }
                        }
                        entry.SUResult = subject.Result;
                    }
                    else if (subject.SubjectCode == "526085,\n526086") // OG
                    {
                        foreach (Paper paper in subject.Paper)
                        {
                            switch (paper.PaperName)
                            {
                                case theoryI:
                                    entry.OGTheoryI = paper.ObtainedMark;
                                    break;
                                case theoryII:
                                    entry.OGTheoryII = paper.ObtainedMark;
                                    break;
                                case theoryTotal:
                                    entry.OGTheoryTotal = paper.ObtainedMark;
                                    break;
                                case practical:
                                    entry.OGPrac = paper.ObtainedMark;
                                    break;
                                case total:
                                    entry.OGTotal = paper.ObtainedMark;
                                    break;
                            }
                        }
                        entry.OGResult = subject.Result;
                    }
                    else if (subject.SubjectCode == "526087") // PE
                    {
                        foreach (Paper paper in subject.Paper)
                        {
                            switch (paper.PaperName)
                            {
                                case theory:
                                    entry.PETheory = paper.ObtainedMark;
                                    break;
                                case practical:
                                    entry.PEPrac = paper.ObtainedMark;
                                    break;
                                case total:
                                    entry.PETotal = paper.ObtainedMark;
                                    break;
                            }
                        }
                        entry.PEResult = subject.Result;
                    }
                }
            }
        }

        csvWriter.WriteHeader<StudentResultEntry>();
        csvWriter.NextRecord();

        foreach (StudentResultEntry e in entries)
        {
            csvWriter.WriteRecord(e);
            csvWriter.NextRecord();
        }

        csvWriter.Flush();
        csvWriter.Dispose();
    }
}
