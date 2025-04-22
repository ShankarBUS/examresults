using System.Net;
using System.Text.Json.Nodes;

namespace ExamResultsCLI;

public class Program
{
    private static readonly string loginUrl = "https://cms2api.tnmgrmu.ac.in/Api/index.php/Login/appLogin?registration_no={0}&login_type=result";

    private static readonly string loadCourseUrl = "https://cms2api.tnmgrmu.ac.in/Api/index.php/Login/loadCourseTerm?registration_no={0}&exam_session={1}";

    private static readonly string resultUrlBase = "https://cms2api.tnmgrmu.ac.in/Api/index.php/StudentPreview/previewGradeMarkAllCourse?registration_no={0}&term_code={1}";

    private static string commonCourseTerm = string.Empty;

    private static long startRegNo = 0;

    private static long endRegNo = 0;

    private static string workingDirectory = Directory.GetCurrentDirectory();

    private static string resultsFile = "Results.csv";

    private static string? jsonFolder;

    public static void Main(string[] args)
    {
        if (Directory.Exists(workingDirectory))
        {
            jsonFolder = Path.Combine(workingDirectory, "JSONs");
            if (!Directory.Exists(jsonFolder))
            {
                Directory.CreateDirectory(jsonFolder);
            }
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Unofficial TNMGRMU Exam Results CLI by ShankarBUS");
        Console.ResetColor();
        Console.WriteLine("This tool fetches exam results for a range of registration numbers from TNMGRMU and saves them as JSON files, " +
            "which can later be processed and merged into a CSV.");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Please note that this tool is NOT AFFILIATED WITH TNMGRMU and is for educational purposes only.");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Please use it responsibly and do not abuse the service.");
        Console.ResetColor();
        Console.WriteLine("Visit https://github.com/ShankarBUS/examresults for further details.");

        Console.WriteLine("Type the starting registration number.");
        if (long.TryParse(Console.ReadLine(), out long s))
        {
            startRegNo = s;
        }
        Console.WriteLine("Type the ending registration number.");
        if (long.TryParse(Console.ReadLine(), out long e))
        {
            endRegNo = e;
        }

        FetchResultsAsync().Wait();
        Console.WriteLine("All results have been fetched successfully.");
        Console.WriteLine("Type (Y/y) to save the results to a CSV file or any other key to exit.");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            BatchResultProcessor.FormatResultAsync(workingDirectory, jsonFolder, resultsFile).Wait();
            Console.WriteLine($"Results saved to {resultsFile} successfully.");
        }
        else
        {
            Console.WriteLine("Exiting without saving results.");
        }
    }

    private static async Task FetchResultsAsync()
    {
        using HttpClient client = new();
        for (long regNo = startRegNo; regNo <= endRegNo; regNo++)
        {
            if (string.IsNullOrEmpty(commonCourseTerm))
            {
                await GetCourseTermAsync(client, regNo);
            }

            string resultUrl = string.Format(resultUrlBase, regNo, commonCourseTerm);
            HttpResponseMessage message = await client.GetAsync(resultUrl);
            if (message?.StatusCode == HttpStatusCode.OK)
            {
                string json = await message.Content.ReadAsStringAsync();
                var writer = File.CreateText(Path.Combine(jsonFolder ?? workingDirectory, regNo + ".json"));
                writer.Write(json);
                writer.Flush();
                writer.Close();
                Console.WriteLine($"Result for {regNo} fetched successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to fetch result for {regNo}. Status code: {message?.StatusCode}");
            }
        }
    }

    private static async Task GetCourseTermAsync(HttpClient client, long regNo)
    {
        HttpResponseMessage loginMessage = await client.GetAsync(string.Format(loginUrl, regNo));
        if (loginMessage?.StatusCode == HttpStatusCode.OK)
        {
            string json = await loginMessage.Content.ReadAsStringAsync();
            var loginResponse = JsonNode.Parse(json);
            if (loginResponse != null && loginResponse!["resultcode"]?.GetValue<string>() == "200")
            {
                var r1 = loginResponse!["result"];
                string? examSession = r1!["exam_session"]?.GetValue<string>();
                HttpResponseMessage loadCourseMessage = await client.GetAsync(string.Format(loadCourseUrl, regNo, examSession));
                if (loadCourseMessage?.StatusCode == HttpStatusCode.OK)
                {
                    string courseJson = await loadCourseMessage.Content.ReadAsStringAsync();
                    var courseResponse = JsonNode.Parse(courseJson);
                    if (courseResponse != null && courseResponse!["resultcode"]!.GetValue<string>() == "200")
                    {
                        var r2 = courseResponse!["result"]![0];
                        string? courseTerm = r2!["course_term"]?.GetValue<string>();
                        if (!string.IsNullOrEmpty(courseTerm))
                        {
                            commonCourseTerm = courseTerm;
                        }
                    }
                }
            }
        }
    }
}