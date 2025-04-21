namespace ExamResultsCLI;

public class Program
{
    private static string loginurl = "https://cms2api.tnmgrmu.ac.in/Api/index.php/Login/appLogin?registration_no={0}&login_type=result";

    private static string loadcourseurl = "https://cms2api.tnmgrmu.ac.in/Api/index.php/Login/loadCourseTerm?registration_no={0}&exam_session={1}";

    private static string resulturlbase = "https://cms2api.tnmgrmu.ac.in/Api/index.php/StudentPreview/previewGradeMarkAllCourse?registration_no={0}&term_code={1}";

    private static string resulturl = "https://cms2api.tnmgrmu.ac.in/Api/index.php/StudentPreview/previewGradeMarkAllCourse?registration_no={0}&term_code=THIRD%20PROFESSIONAL%20PART-II";

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

        Console.WriteLine("Welcome to the [Unofficial] TNMGRMU Exam Results CLI.");
        Console.WriteLine("This tool fetches multiple exam results from TNMGRMU and saves them in JSON format.");

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
        for (long s = startRegNo; s <= endRegNo; s++)
        {
            string link = string.Format(resulturl, s);
            HttpResponseMessage message = await client.GetAsync(link);
            if (message?.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string json = await message.Content.ReadAsStringAsync();
                var writer = File.CreateText(Path.Combine(jsonFolder ?? workingDirectory, s + ".json"));
                writer.Write(json);
                writer.Flush();
                writer.Close();
                Console.WriteLine($"Result for {s} fetched successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to fetch result for {s}. Status code: {message?.StatusCode}");
            }
        }
    }
}