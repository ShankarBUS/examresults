import { createExpander, createKeyValueTable, setupMessagePopup, showMessagePopup } from 'https://shankarbus.github.io/kaadu-ui/kaadu-ui.js';

setupMessagePopup();

async function fetchAvailableResults() {
  try {
    const dataUrl = 'https://cms2api.tnmgrmu.ac.in/Api/index.php/Login/loadResultData';
    const response = await fetch(dataUrl);
    if (!response.ok) {
      throw new Error('FAILED TO FETCH DATA.');
    }
    const data = await response.json();
    if (data.resultcode !== '200') {
      throw new Error(data.resultmessage);
    }
    return data.result;
  } catch (error) {
    showMessagePopup('ERROR IN FETCHING AVAILABLE RESULTS: ' + error.message);
    return [];
  }
}

async function displayAvailableResults() {
  const data = await fetchAvailableResults();
  const list = document.getElementById('availableResultsList');
  list.innerHTML = '';

  if (!data || data.length === 0) {
    return;
  }

  data.forEach(item => {
    const listItem = document.createElement('li');
    listItem.className = 'available-result-item';

    const publishedDate = document.createElement('span');
    publishedDate.textContent = item.publised_date;

    const text = document.createElement('span');
    text.textContent = `${item.course_name} (${item.term_name})`;

    if (item.is_new === 'Yes') {
      text.className = 'new-result-text';
    }

    listItem.appendChild(publishedDate);
    listItem.appendChild(text);
    list.appendChild(listItem);
  });
}

// Fetching and displaying the results based on the entered registration number.
async function fetchResultsFromRegNo(regNo) {
  const loginUrl = `https://cms2api.tnmgrmu.ac.in/Api/index.php/Login/appLogin?registration_no=${regNo}&login_type=result`;
  const loadCourseUrlBase = `https://cms2api.tnmgrmu.ac.in/Api/index.php/Login/loadCourseTerm?registration_no=${regNo}&exam_session=`;
  const resultUrlBase = `https://cms2api.tnmgrmu.ac.in/Api/index.php/StudentPreview/previewGradeMarkAllCourse?registration_no=${regNo}&term_code=`;

  // Step 1: Get exam_session
  const loginResponse = await fetch(loginUrl);
  if (!loginResponse.ok) {
    throw new Error('FAILED TO FETCH EXAM SESSION.');
  }
  const loginData = await loginResponse.json();
  if (loginData.resultcode !== '200') {
    throw new Error(loginData.resultmessage || 'INVALID REGISTRATION NUMBER.');
  }
  const examSession = loginData.result.exam_session;
  if (!examSession) {
    throw new Error('EXAM SESSION NOT FOUND.');
  }

  // Step 2: Get course_term
  const loadCourseUrl = `${loadCourseUrlBase}${encodeURIComponent(examSession)}`;
  const courseResponse = await fetch(loadCourseUrl);
  if (!courseResponse.ok) {
    throw new Error('FAILED TO FETCH COURSE TERM.');
  }
  const courseData = await courseResponse.json();
  if (courseData.resultcode !== '200' || courseData.result.length === 0) {
    throw new Error(courseData.resultmessage || 'INVALID REGISTRATION NUMBER.');
  }
  // The student may have written one or more exams recently, but we take only the first one.
  const termCode = courseData.result[0].course_term;
  if (!termCode) {
    throw new Error('COURSE TERM NOT FOUND.');
  }

  // Step 3: Get result data
  const resultUrl = `${resultUrlBase}${encodeURIComponent(termCode)}`;
  const resultResponse = await fetch(resultUrl);
  if (!resultResponse.ok) {
    throw new Error('FAILED TO FETCH RESULTS.');
  }
  const resultData = await resultResponse.json();
  if (resultData.resultcode !== '200') {
    throw new Error(resultData.resultmessage || 'NO RESULTS FOUND.');
  }
  return resultData;
}

async function showResultsAsync(regNo) {
  const loadingProgressBar = document.getElementById('loadingProgressBar');
  loadingProgressBar.style.display = 'block';

  try {
    const resultData = await fetchResultsFromRegNo(regNo);
    displayResults(resultData);
  } catch (error) {
    showMessagePopup(error.message);
    if (regNo > 3) return; // For testing purposes, we can use dummy data for regNo 1, 2, and 3 only.
    // If the API call fails, we can use dummy data for regNo 1, 2, and 3.
    const useDummyData = confirm('API CALL FAILED. DO YOU WANT TO USE DUMMY DATA?');
    if (useDummyData) {
      fetch(`./mockdata/dummy${regNo}.json`)
        .then(response => response.json())
        .then(data => {
          displayResults(data);
        })
        .catch((e) => {
          showMessagePopup(e.message);
        });
    }
  } finally {
    loadingProgressBar.style.display = 'none';

    urlParams.set('regNo', regNo);
    window.history.replaceState(null, '',
      `${window.location.origin}${window.location.pathname}?${urlParams.toString()}`);
  }
}

async function onShowResults() {
  const regNo = document.getElementById('regNo').value;
  if (!regNo) {
    showMessagePopup('PLEASE ENTER A REGISTRATION NUMBER.');
    return;
  }
  await showResultsAsync(regNo);
}

document.getElementById('showResults').addEventListener('click', onShowResults);

document.getElementById('regNo').addEventListener('keypress', (event) => {
  if (event.key === 'Enter') onShowResults();
});

document.getElementById('backButton').addEventListener('click', () => {
  document.getElementById('resultsContainer').innerHTML = '';
  document.getElementById('regNo').value = '';
  document.body.classList.remove('results-fetched');

  urlParams.set('regNo', null);
  window.history.replaceState(null, '',
    `${window.location.origin}${window.location.pathname}`);
});

// New: TOTAL (THEORY+PRACTICAL/CLINICAL + VIVA) IN %
// Prev: TOTAL (THEORY+PRACTICAL/CLINICAL+VIVA) IN %
// So it is safe to check for both by checking the keys values' presence.
let totalPaperKey = ['TOTAL', 'THEORY', 'PRACTICAL'];
function isTotalPaper(paper) {
  if (!paper || !paper.paper_name) return false;
  const paperName = paper.paper_name.toUpperCase();
  return totalPaperKey.every(key => paperName.includes(key));
}

function isPass(result) {
  return result.toLowerCase() === 'pass';
}

let resultText = '';

function displayResults(data) {
  const container = document.getElementById('resultsContainer');
  container.innerHTML = '';
  resultText = '';

  if (!data || !data.result || data.resultcode !== '200'
    || !data.result.student || data.result.student.length === 0) {
    container.innerHTML = '<p>NO RESULTS FOUND.</p>';
    return;
  }

  const student = data.result.student[0];

  const group = document.createElement('div');
  group.className = 'group result-group';

  const studentInfo = {
    'NAME': student.student_name,
    'REGISTRATION NUMBER': student.registration_no,
    'COURSE': student.course,
    'INSTITUTION': student.institution_name,
    'EXAM SESSION': student.exam_session_name,
    'REGULATION': student.regulation,
    'TERM': student.term_name,
    'RESULT PUBLISH DATE': student.result_publish_from_date
  };

  const infoTable = createKeyValueTable(studentInfo);
  group.appendChild(infoTable);

  const allpass = student.subject.every(subject => isPass(subject.result));
  const failedSubjects = student.subject.filter(subject => !isPass(subject.result));
  const failedSubjectsText = failedSubjects.map(subject => subject.subject_name).join(', ');

  if (allpass) {
    const passMessage = document.createElement('p');
    passMessage.textContent = 'CONGRATULATIONS! YOU HAVE PASSED ALL SUBJECTS.';
    passMessage.className = 'pass-message';
    group.appendChild(passMessage);
  } else {
    const failMessage = document.createElement('p');
    failMessage.textContent = `YOU HAVE FAILED IN THE FOLLOWING SUBJECTS: ${failedSubjectsText}`;
    failMessage.className = 'fail-message';
    group.appendChild(failMessage);
  }

  // For sharing
  resultText = `Name: ${student.student_name}\nReg. No: ${student.registration_no}\n\nSubjects:\n`;

  const expanderGrid = document.createElement('div');
  expanderGrid.className = 'cards-grid';

  student.subject.forEach((subject, index) => {
    const detailsPreview = document.createElement('div');
    detailsPreview.className = 'details-preview';

    const subjectText = document.createElement('span');
    subjectText.textContent = `${subject.subject_name}`;
    subjectText.className = 'subject-text';
    detailsPreview.appendChild(subjectText);

    const badgeArea = document.createElement('div');
    badgeArea.className = 'badge-area';

    detailsPreview.appendChild(badgeArea);

    const badge = document.createElement('span');
    badge.textContent = subject.result;
    badge.className = `badge ${isPass(subject.result) ? 'badge-pass' : 'badge-fail'}`;

    const totalText = document.createElement('span');

    const totalPaper = subject.paper.find(paper => isTotalPaper(paper));
    if (totalPaper) {

      const totalPercentage = parseFloat(totalPaper.obtained_mark);
      if (totalPercentage >= 75) {
        const specialBadge = document.createElement('span');
        if (totalPercentage >= 80) {
          specialBadge.textContent = 'H';
          specialBadge.className = 'badge badge-honors';
        } else if (totalPercentage >= 75) {
          specialBadge.textContent = 'D';
          specialBadge.className = 'badge badge-distinction';
        }
        badgeArea.appendChild(specialBadge);
      }
      totalText.textContent = `${totalPercentage.toFixed(1)}%`;
      const specialScore = totalPercentage >= 80 ? 'Honors' : totalPercentage >= 75 ? 'Distinction' : '';
      resultText += `${index + 1}. ${subject.subject_name}: ${subject.result} (${totalPercentage.toFixed(1)}%) ${specialScore ? `- ${specialScore}` : ''}\n`;
    }
    totalText.className = 'total-text';

    badgeArea.appendChild(badge);

    const marks = subject.paper.reduce((acc, paper) => {
      acc[paper.paper_name] = paper.obtained_mark;
      return acc;
    }, {});

    const markTable = createKeyValueTable(marks);

    const expander = createExpander([detailsPreview, totalText], markTable,
      `expander-button ${isPass(subject.result) ? '' : 'expander-button-fail'}`);
    expanderGrid.appendChild(expander);
  });

  group.appendChild(expanderGrid);
  container.appendChild(group);

  document.body.classList.add('results-fetched');

  // For sharing
  resultText += `\nView more details here: ${window.location.href}`;
}

const disclaimerButton = document.getElementById('disclaimerButton');

disclaimerButton.addEventListener('click', () => {
  showMessagePopup('DISCLAIMER: THIS WEBSITE IS NOT AFFILIATED WITH TNMGRMU. IT IS AN UNOFFICIAL PLATFORM FOR VIEWING EXAM RESULTS, PROVIDED FOR INFORMATIONAL PURPOSES ONLY. FOR OFFICIAL AND AUTHENTIC RESULTS, PLEASE VISIT THE TNMGRMU OFFICIAL WEBSITE.');
});

const urlParams = new URLSearchParams(window.location.search);

if (urlParams.has('regNo')) {
  const _regNo = urlParams.get('regNo');
  document.getElementById('regNo').value = _regNo;
  showResultsAsync(_regNo);
}

function shareResults() {
  console.log(resultText);
  if (navigator.share) {
    navigator.share({
      title: 'Exam Results',
      text: resultText
    }).catch(err => {
      showMessagePopup('SHARING FAILED.');
      console.error(err);
    });
  } else {
    navigator.clipboard.writeText(resultText).then(() => {
      showMessagePopup('RESULTS COPIED TO CLIPBOARD.');
    }).catch(err => {
      showMessagePopup('FAILED TO COPY RESULTS.');
      console.error(err);
    });
  }
}

document.getElementById('shareButton').addEventListener('click', shareResults);

displayAvailableResults();