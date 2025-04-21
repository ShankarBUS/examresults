# Collecting Emails and Registration Numbers in Google Forms and Distributing Exam Results with Apps Script

This demo explains how to create a Google Form to collect email addresses and registration numbers, and use Google Apps Script to fetch exam results from the API and distribute them via email.

## Step 1: Create a Google Form

1. Open [Google Forms](https://forms.google.com).
2. Create a new form and add a question:
    - **Registration Number**: Use the "Short Answer" question type. Make it a **"Required"**.
3. Go to the settings tab, then to the "Responses" section, and ensure **"Collect email addresses"** is set to `Verified`.
4. Make sure to enable **"Limit to 1 response"**.

## Step 2: Write the Apps Script

1. Click the three dots next to your profile picture to open the menu.
2. Click the **"Apps Script"** menu.
3. Replace the default code with the following script:

```javascript
var formId = ''; // Eg: https://docs.google.com/forms/d/{id}/edit
var dummyFileId = ''; // Drive id of the dummy json file if needed.
var debug = false; // Enable only when debugging as it runs the script only once.
var useDummyJSON = false; // Uses the provided dummy json instead of fetching the actual result.
var sendEmail = true;

function main() {
  var form = FormApp.openById(formId);
  if (form == null) return;
  var formResponses = form.getResponses();
  for (var i = 0; i < formResponses.length; i++)
  {
    var formResponse = formResponses[i];
    var itemResponses = formResponse.getItemResponses();
    var regNo = itemResponses[0].getResponse();
    var email = formResponse.getRespondentEmail();
    
    if (debug) console.log(email);
    else
    {
      console.log(`${regNo}: ${email}`)
    }
    result = getResult(regNo);
    if (debug) console.log(result);

    if (sendEmail & email != null) MailApp.sendEmail(email, 'TNMGRMU Final Year Results (Unofficial)', result, { name: 'Automated Script' });
    
    if (debug) return;
  }
}

function getResult(regNo)
{
  var json = useDummyJSON ? getDummyJson() : fetchResult(regNo);
  var data = JSON.parse(json);
  var str = parseStudentResult(data);
  return str;
}

function fetchResult(regNo)
{
  var url = `https://cms2api.tnmgrmu.ac.in/Api/index.php/StudentPreview/previewGradeMarkAllCourse?registration_no=${regNo}&term_code=THIRD%20PROFESSIONAL%20PART-II`
  if (debug) console.log(url);
  var json = UrlFetchApp.fetch(url).getContentText();
  if (debug) console.log(json);
  return json;
}

function parseStudentResult(data) {
  if (!data.result || !data.result.student) {
    return data.resultmessage;
  }
  var allPass = isAllPass(data) ? ' (All Pass)' : ' :(';
  var resultString = `Final Year Exam Results${allPass}\n\n`;

  data.result.student.forEach(student => {
    resultString += `Registration No: ${student.registration_no}\n`;
    resultString += `Name: ${student.student_name}\n`;

    student.subject.forEach(subject => {
      resultString += `${subject.subject_name}: ${subject.result}\n`;
      subject.paper.forEach(paper => {
        resultString += `    • ${paper.paper_name}: ${paper.obtained_mark}\n`;
      });
    });
  });

  return resultString;
}

function isAllPass(data)
{
  if (!data.result || !data.result.student) {
    return false;
  }

  var allPass = true;

  data.result.student.forEach(student => {
    student.subject.forEach(subject => {
      allPass &= subject.result.toLowerCase() == 'pass';
    });
  });

  return allPass;
}

function getDummyJson()
{
  var file = DriveApp.getFileById(dummyFileId);
  return file.getBlob().getDataAsString();
}
```

## Step 4: Test the Workflow

1. Submit a test response in the Google Form.
2. Hit `Debug` to see the `Execution log`.
3. If `sendEmail` is set to true, verify that the email with the exam result is sent to the provided email address.
