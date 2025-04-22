using System.Text.Json.Serialization;

namespace ExamResultsCLI;

// THEORY IN MARKS
// THEORY I
// THEORY II
// THEORY TOTAL IN MARKS
// TOTAL THEORY IN %
// PRACTICAL/CLINICAL + VIVA IN MARKS
// PRACTICAL/CLINICAL + VIVA IN %
// TOTAL (THEORY+PRACTICAL/CLINICAL+VIVA) IN %
public static class PaperNames
{
    public const string theory = "THEORY IN MARKS";

    public const string theoryI = "THEORY I";

    public const string theoryII = "THEORY II";

    public const string theoryTotal = "THEORY TOTAL IN MARKS";

    public const string practical = "PRACTICAL/CLINICAL + VIVA IN MARKS";

    public const string total = "TOTAL (THEORY+PRACTICAL/CLINICAL+VIVA) IN %";
}

public class ResultResponse
{
    [JsonPropertyName("resultcode")]
    public string? ResultCode { get; set; }

    [JsonPropertyName("resultmessage")]
    public string? ResultMessage { get; set; }

    [JsonPropertyName("result")]
    public Result? Result { get; set; }
}

public class Result
{
    [JsonPropertyName("footer_message")]
    public string? FooterMessage { get; set; }

    [JsonPropertyName("student")]
    public Student[]? Student { get; set; }
}

public class Student
{
    [JsonPropertyName("registration_no")]
    public string? RegistrationNo { get; set; }

    [JsonPropertyName("student_name")]
    public string? StudentName { get; set; }

    [JsonPropertyName("course")]
    public string? Course { get; set; }

    [JsonPropertyName("institution_name")]
    public string? InstitutionName { get; set; }

    [JsonPropertyName("exam_session_name")]
    public string? ExamSessionName { get; set; }

    [JsonPropertyName("regulation")]
    public string? Regulation { get; set; }

    [JsonPropertyName("term_name")]
    public string? TermName { get; set; }

    [JsonPropertyName("result_publish_from_date")]
    public string? ResultPublishFromDate { get; set; }

    [JsonPropertyName("subject")]
    public Subject[]? Subject { get; set; }
}

public class Subject
{
    [JsonPropertyName("subject_code")]
    public string? SubjectCode { get; set; }

    [JsonPropertyName("subject_name")]
    public string? SubjectName { get; set; }

    [JsonPropertyName("paper")]
    public Paper[]? Paper { get; set; }

    [JsonPropertyName("result")]
    public string? Result { get; set; }
}

public class Paper
{
    [JsonPropertyName("paper_name")]
    public string? PaperName { get; set; }

    [JsonPropertyName("min_marks")]
    public string? MinMarks { get; set; }

    [JsonPropertyName("max_marks")]
    public string? MaxMarks { get; set; }

    [JsonPropertyName("obtained_mark")]
    public string? ObtainedMark { get; set; }

    [JsonPropertyName("words")]
    public string? Words { get; set; }
}
