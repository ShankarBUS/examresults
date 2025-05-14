# Unofficial TNMGRMU Exam Results Web Interface and .NET Command-Line Interface

## Overview

This project provides an **unofficial platform** to access Tamil Nadu Dr. M.G.R. Medical University (TNMGRMU) Exam Results. It includes:

- A **web-based interface** for students to view their results. [GitHub Pages Link](https://shankarbus.github.io/examresults/)
- A **.NET command-line interface (CLI)** for batch processing and exporting multiple results into a single CSV file. It is currently hardcoded for **final year** results (i.e., `THIRD PROFESSIONAL PART-II`).
- A [**Guide for Automating TNGMRMU Exam Result Distribution Using Google Forms and Apps Script**](/docs/automation_with_google_forms_and_apps_script.md)

## Disclaimer

> [!CAUTION]
>
> - This is an **unofficial tool** only for educational purposes and is **not affiliated with TNMGRMU**. Use at your own discretion.
> - There is no guarantee that this will work forever as they might change the API anytime in the future.

## Features

- **Web Interface**: Allows users to enter their registration number and fetch results interactively.
- **.NET CLI Tool**: Automates the process of fetching results for a range of registration numbers and exporting them to a CSV file.

## Installation and Setup

### Prerequisites

- **Web Interface**: A modern web browser.
- **.NET CLI Tool**: [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0).

### Steps

1. Clone the repository:
   ```bash
   git clone <repository-url>
   ```
2. Navigate to the project directory:
   ```bash
   cd examresults
   ```
3. For the CLI tool:
   - Navigate to the CLI folder:
     ```bash
     cd src/ExamResultsCLI
     ```
   - Build the project:
     ```bash
     dotnet build
     ```
   - Run the CLI:
     ```bash
     dotnet run
     ```
4. For the web interface:
   - Open `web/index.html` in a browser.

## Usage

### Web Interface

1. Open `index.html` in a browser.
2. Enter your **Registration Number** in the input field.
3. Click the **Show Results** button to fetch and display your exam results.
4. Use the **Back** button to reset the interface.
5. Alternatively, visit the hosted website: [https://shankarbus.github.io/examresults/](https://shankarbus.github.io/examresults/)

### Command-Line Interface (CLI)

1. Run the CLI tool:
   ```bash
   dotnet run
   ```
2. Enter the starting and ending registration numbers when prompted.
3. The tool will fetch results for the specified range.
4. Optionally, save the results to a CSV file by typing `Y` when prompted.

## Project Structure

- **web/**: Contains the web interface files (`index.html`, `script.js`, `styles.css`).
- **src/ExamResultsCLI/**: Contains the CLI tool source code.

## APIs Used

This project interacts with the TNMGRMU API to fetch exam results. Below are the APIs used and their functionality:

### 1. Login API

**URL:**

```plaintext
https://cms2api.tnmgrmu.ac.in/Api/index.php/Login/appLogin?registration_no={0}&login_type=result
```

**Description:**

- This API is used to authenticate and retrieve the `exam_session` for a given registration number.
- Replace `{0}` with the student's registration number.

**Sample Response:**

```json
{
  "resultcode": "200",
  "resultmessage": "Success",
  "result": {
    "registration_no": "",
    "student_name": "",
    "course": "M.B.B.S.",
    "exam_session": "Feb 2024",
    "source": "CMS 1.0"
  }
}
```

### 2. Load Course Term API

**URL:**

```plaintext
https://cms2api.tnmgrmu.ac.in/Api/index.php/Login/loadCourseTerm?registration_no={0}&exam_session={1}
```

**Description:**

- This API retrieves the `course_term` for the next API call.
- Note: it returns an array as there could be multiple terms if the student wrote more than one exams recently.
- Replace `{0}` with the registration number and `{1}` with the `exam_session` obtained from the Login API.

**Sample Response:** (for final year)

```json
{
  "resultcode": "200",
  "resultmessage": "Get data",
  "result": [
    {
      "course_term": "THIRD PROFESSIONAL PART-II"
    }
  ]
}
```

### 3. Result API

**Base URL:**

```plaintext
https://cms2api.tnmgrmu.ac.in/Api/index.php/StudentPreview/previewGradeMarkAllCourse?registration_no={0}&term_code={1}
```

**Description:**

- This API fetches the exam results in JSON format.
- Replace `{0}` with the registration number and `{1}` with the `course_term` obtained from the Load Course Term API.

**Example Specific URL:** (for final year)

```plaintext
https://cms2api.tnmgrmu.ac.in/Api/index.php/StudentPreview/previewGradeMarkAllCourse?registration_no={0}&term_code=THIRD%20PROFESSIONAL%20PART-II
```

**Functionality:**

The JSON response is parsed and displayed in the web interface or formatted into a CSV file for batch processing.

These APIs are integral to the functionality of both the web and CLI tools, enabling seamless retrieval and processing of exam results.
