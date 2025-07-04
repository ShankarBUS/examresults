using System.Net;
using System.Text.Json.Nodes;

namespace ExamResultsCLI;

public class Program
{
    private static readonly string loginUrl = "https://cms2api.tnmgrmu.ac.in/Api/index.php/Login/appLogin?registration_no={0}&login_type=result";

    private static readonly string loadCourseUrl = "https://cms2api.tnmgrmu.ac.in/Api/index.php/Login/loadCourseTerm?registration_no={0}&exam_session={1}";

    private static readonly string resultUrlBase = "https://cms2api.tnmgrmu.ac.in/Api/index.php/StudentPreview/previewGradeMarkAllCourse?registration_no={0}&term_code={1}";

    private static string commonCourseTerm = string.Empty;

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
        Console.WriteLine("Welcome to the Unofficial TNMGRMU Exam Results CLI by ShankarBUS.");
        Console.ResetColor();
        Console.WriteLine("This tool fetches exam results for a range of registration numbers from TNMGRMU and saves them as JSON files. These files can later be processed and merged into a CSV file.");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Note: This tool is NOT AFFILIATED WITH TNMGRMU and is intended for educational purposes only.");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Use this tool responsibly. Do not abuse the service.");
        Console.ResetColor();
        Console.WriteLine("For more information, visit: https://github.com/ShankarBUS/examresults");

        List<long> regNumbers = [];
        Console.WriteLine("How would you like to provide the registration numbers?");
        Console.WriteLine("1. Load registration numbers from a file.");
        Console.WriteLine("2. Enter a starting and ending registration number.");
        Console.Write("Enter your choice (1 or 2): ");
        string? choice = Console.ReadLine().Trim();
        if (choice == "1")
        {
            Console.Write("Enter the path to the file containing the registration numbers (one per line): ");
            string? filePath = Console.ReadLine().Trim();
            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
            {
                foreach (var line in File.ReadAllLines(filePath))
                {
                    if (long.TryParse(line.Trim(), out long regNo))
                    {
                        regNumbers.Add(regNo);
                    }
                }
                if (regNumbers.Count == 0)
                {
                    Console.WriteLine("No valid registration numbers were found in the file. The program will now exit.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("The specified file was not found, or the path is invalid. The program will now exit.");
                return;
            }
        }
        else
        {
            Console.Write("Enter the starting registration number: ");
            long startRegNo = 0;
            long endRegNo = 0;
            if (long.TryParse(Console.ReadLine(), out long s))
            {
                startRegNo = s;
            }
            Console.Write("Enter the ending registration number: ");
            if (long.TryParse(Console.ReadLine(), out long e))
            {
                endRegNo = e;
            }
            if (endRegNo < startRegNo)
            {
                Console.WriteLine("The ending registration number cannot be less than the starting registration number. The program will now exit.");
                return;
            }
            for (long regNo = startRegNo; regNo <= endRegNo; regNo++)
            {
                regNumbers.Add(regNo);
            }
        }

        FetchResultsAsync(regNumbers).Wait();
        Console.WriteLine("All results have been fetched successfully.");
        Console.Write("Enter 'Y' to save the results to a CSV file, or any other key to exit: ");
        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            BatchResultProcessor.FormatResultAsync(workingDirectory, jsonFolder, resultsFile).Wait();
            Console.WriteLine($"Results have been saved to '{resultsFile}' successfully.");
        }
        else
        {
            Console.WriteLine("Exiting without saving the results.");
        }
    }
    private static async Task FetchResultsAsync(List<long> regNumbers)
    {
        using HttpClient client = new();
        foreach (long regNo in regNumbers)
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