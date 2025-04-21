using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExamResultsCLI;

public class MockData
{
    const string theoryI = "THEORY I";

    const string theoryII = "THEORY II";

    const string theory = "THEORY IN MARKS";

    const string theoryTotal = "THEORY TOTAL IN MARKS";

    const string practical = "PRACTICAL/CLINICAL + VIVA IN MARKS";

    const string total = "TOTAL (THEORY+PRACTICAL/CLINICAL+VIVA) IN %";

    // Just for testing purposes, not used in the actual code

    #region Mock Data

    // A mock result of a student who just passed the exams.
    private static ResultResponse dummy1 = new()
    {
        ResultCode = "200",
        ResultMessage = "Success",
        Result = new Result
        {
            FooterMessage = "The Statement of Marks bears no correction",
            Student =
            [
                new Student
                {
                    RegistrationNo = "52002010xxxx",
                    StudentName = "Normal Shungus",
                    Course = "BACHELOR OF MEDICINE & BACHELOR OF SURGERY",
                    InstitutionName = "Government Medical College",
                    ExamSessionName = "Mar 2025",
                    Regulation = "2019-2020 (NON-SEMESTER)",
                    TermName = "THIRD PROFESSIONAL PART-II",
                    ResultPublishFromDate = "01-05-2025",
                    Subject =
                    [
                        new Subject
                        {
                            SubjectCode = "526081,\n526082",
                            SubjectName = "GENERAL MEDICINE (PAPER I & II)",
                            Paper =
                            [
                                new Paper { PaperName = theoryI, ObtainedMark = "50" },
                                new Paper { PaperName = theoryII, ObtainedMark = "50" },
                                new Paper { PaperName = theoryTotal, ObtainedMark = "100" },
                                new Paper { PaperName = practical, ObtainedMark = "150" },
                                new Paper { PaperName = total, ObtainedMark = "62.5" }
                            ],
                            Result = "PASS"
                        },
                        new Subject
                        {
                            SubjectCode = "526083,\n526084",
                            SubjectName = "GENERAL SURGERY (PAPER I & II)",
                            Paper =
                            [
                                new Paper { PaperName = theoryI, ObtainedMark = "48" },
                                new Paper { PaperName = theoryII, ObtainedMark = "52" },
                                new Paper { PaperName = theoryTotal, ObtainedMark = "100" },
                                new Paper { PaperName = practical, ObtainedMark = "100" },
                                new Paper { PaperName = total, ObtainedMark = "50" }
                            ],
                            Result = "PASS"
                        },
                        new Subject
                        {
                            SubjectCode = "526085,\n526086",
                            SubjectName = "OBSTETRICS & GYNAECOLOGY (PAPER I & II)",
                            Paper =
                            [
                                new Paper { PaperName = theoryI, ObtainedMark = "56" },
                                new Paper { PaperName = theoryII, ObtainedMark = "46" },
                                new Paper { PaperName = theoryTotal, ObtainedMark = "102" },
                                new Paper { PaperName = practical, ObtainedMark = "120" },
                                new Paper { PaperName = total, ObtainedMark = "55.5" }
                            ],
                            Result = "PASS"
                        },
                        new Subject
                        {
                            SubjectCode = "526087",
                            SubjectName = "PEDIATRICS",
                            Paper =
                            [
                                new Paper { PaperName = theory, ObtainedMark = "44" },
                                new Paper { PaperName = practical, ObtainedMark = "60" },
                                new Paper { PaperName = total, ObtainedMark = "52" }
                            ],
                            Result = "PASS"
                        }
                    ]
                }
            ]
        }
    };

    // A mock result of a student who failed the exams.
    private static ResultResponse dummy2 = new()
    {
        ResultCode = "200",
        ResultMessage = "Success",
        Result = new Result
        {
            FooterMessage = "The Statement of Marks bears no correction",
            Student =
            [
                new Student
                {
                    RegistrationNo = "52002010xxxx",
                    StudentName = "Not Shungus",
                    Course = "BACHELOR OF MEDICINE & BACHELOR OF SURGERY",
                    InstitutionName = "Government Medical College",
                    ExamSessionName = "Mar 2025",
                    Regulation = "2019-2020 (NON-SEMESTER)",
                    TermName = "THIRD PROFESSIONAL PART-II",
                    ResultPublishFromDate = "01-05-2025",
                    Subject =
                    [
                        new Subject
                        {
                            SubjectCode = "526081,\n526082",
                            SubjectName = "GENERAL MEDICINE (PAPER I & II)",
                            Paper =
                            [
                                new Paper { PaperName = theoryI, ObtainedMark = "40" },
                                new Paper { PaperName = theoryII, ObtainedMark = "40" },
                                new Paper { PaperName = theoryTotal, ObtainedMark = "80" },
                                new Paper { PaperName = practical, ObtainedMark = "100" },
                                new Paper { PaperName = total, ObtainedMark = "45" }
                            ],
                            Result = "FAIL"
                        },
                        new Subject
                        {
                            SubjectCode = "526083,\n526084",
                            SubjectName = "GENERAL SURGERY (PAPER I & II)",
                            Paper =
                            [
                                new Paper { PaperName = theoryI, ObtainedMark = "36" },
                                new Paper { PaperName = theoryII, ObtainedMark = "40" },
                                new Paper { PaperName = theoryTotal, ObtainedMark = "76" },
                                new Paper { PaperName = practical, ObtainedMark = "80" },
                                new Paper { PaperName = total, ObtainedMark = "39" }
                            ],
                            Result = "FAIL"
                        },
                        new Subject
                        {
                            SubjectCode = "526085,\n526086",
                            SubjectName = "OBSTETRICS & GYNAECOLOGY (PAPER I & II)",
                            Paper =
                            [
                                new Paper { PaperName = theoryI, ObtainedMark = "50" },
                                new Paper { PaperName = theoryII, ObtainedMark = "46" },
                                new Paper { PaperName = theoryTotal, ObtainedMark = "96" },
                                new Paper { PaperName = practical, ObtainedMark = "120" },
                                new Paper { PaperName = total, ObtainedMark = "54" }
                            ],
                            Result = "PASS"
                        },
                        new Subject
                        {
                            SubjectCode = "526087",
                            SubjectName = "PEDIATRICS",
                            Paper =
                            [
                                new Paper { PaperName = theory, ObtainedMark = "40" },
                                new Paper { PaperName = practical, ObtainedMark = "60" },
                                new Paper { PaperName = total, ObtainedMark = "50" }
                            ],
                            Result = "PASS"
                        }
                    ]
                }
            ]
        }
    };

    // A mock result of a student who passed with distinction and honors.
    private static ResultResponse dummy3 = new()
    {
        ResultCode = "200",
        ResultMessage = "Success",
        Result = new Result
        {
            FooterMessage = "The Statement of Marks bears no correction",
            Student =
             [
                 new Student
                {
                    RegistrationNo = "52002010xxxx",
                    StudentName = "Humungus Shungus",
                    Course = "BACHELOR OF MEDICINE & BACHELOR OF SURGERY",
                    InstitutionName = "Government Medical College",
                    ExamSessionName = "Mar 2025",
                    Regulation = "2019-2020 (NON-SEMESTER)",
                    TermName = "THIRD PROFESSIONAL PART-II",
                    ResultPublishFromDate = "01-05-2025",
                    Subject =
                    [
                        new Subject
                        {
                            SubjectCode = "526081,\n526082",
                            SubjectName = "GENERAL MEDICINE (PAPER I & II)",
                            Paper =
                            [
                                new Paper { PaperName = theoryI, ObtainedMark = "75" },
                                new Paper { PaperName = theoryII, ObtainedMark = "80" },
                                new Paper { PaperName = theoryTotal, ObtainedMark = "155" },
                                new Paper { PaperName = practical, ObtainedMark = "170" },
                                new Paper { PaperName = total, ObtainedMark = "81.25" }
                            ],
                            Result = "PASS"
                        },
                        new Subject
                        {
                            SubjectCode = "526083,\n526084",
                            SubjectName = "GENERAL SURGERY (PAPER I & II)",
                            Paper =
                            [
                                new Paper { PaperName = theoryI, ObtainedMark = "70" },
                                new Paper { PaperName = theoryII, ObtainedMark = "75" },
                                new Paper { PaperName = theoryTotal, ObtainedMark = "145" },
                                new Paper { PaperName = practical, ObtainedMark = "160" },
                                new Paper { PaperName = total, ObtainedMark = "76.25" }
                            ],
                            Result = "PASS"
                        },
                        new Subject
                        {
                            SubjectCode = "526085,\n526086",
                            SubjectName = "OBSTETRICS & GYNAECOLOGY (PAPER I & II)",
                            Paper =
                            [
                                new Paper { PaperName = theoryI, ObtainedMark = "84" },
                                new Paper { PaperName = theoryII, ObtainedMark = "78" },
                                new Paper { PaperName = theoryTotal, ObtainedMark = "162" },
                                new Paper { PaperName = practical, ObtainedMark = "170" },
                                new Paper { PaperName = total, ObtainedMark = "83" }
                            ],
                            Result = "PASS"
                        },
                        new Subject
                        {
                            SubjectCode = "526087",
                            SubjectName = "PEDIATRICS",
                            Paper =
                            [
                                new Paper { PaperName = theory, ObtainedMark = "82" },
                                new Paper { PaperName = practical, ObtainedMark = "86" },
                                new Paper { PaperName = total, ObtainedMark = "84" }
                            ],
                            Result = "PASS"
                        }
                    ]
                }
             ]
        }
    };

    #endregion

    public static void SaveMockDataJSONs(string jsonFolder)
    {
        string dummy1Path = Path.Combine(jsonFolder, "dummy1.json");
        string dummy2Path = Path.Combine(jsonFolder, "dummy2.json");
        string dummy3Path = Path.Combine(jsonFolder, "dummy3.json");

        JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        File.WriteAllText(dummy1Path, JsonSerializer.Serialize(dummy1, options));
        File.WriteAllText(dummy2Path, JsonSerializer.Serialize(dummy2, options));
        File.WriteAllText(dummy3Path, JsonSerializer.Serialize(dummy3, options));
    }
}